﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public class MultipartForm
    {
        private readonly Stream _stream;
        private readonly Regex boundaryRegex = new Regex(@"(?<=boundary=)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);
        private Regex _nameRegex = new Regex(@"(?<=name\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private Regex _filenameRegex = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private Regex _charsetRegex = new Regex(@"(?<=charset\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private Regex _contentTypeRegex = new Regex(@"(?<=Content\-Type:)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);
        private Regex _contentTransferEncoding = new Regex(@"(?<=Content\-Transfer\-Encoding:)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);

        public MultipartForm(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new Exception("Stream should support reading");
            _stream = stream;
        }

        public MultipartFormData Parse()
        {
            if (!_stream.CanRead) throw new Exception("Stream should support reading");
            var reader = new StreamLineReader(_stream);

            string boundary = null;
            bool isMultipart = false, isContentBegin = false;
            MultipartFormData data = null;
            reader.Read(bytes =>
            {
                string line = Encoding.ASCII.GetString(bytes);
                if (line.StartsWith("Content-Type:"))
                {
                    isMultipart = true;
                    if (!line.Contains("multipart/form-data")) throw new Exception();
                    var boundaryMatch = boundaryRegex.Match(line);
                    if (boundaryMatch.Success) boundary = boundaryMatch.Value;
                    return true;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    isContentBegin = true;
                    if (!isMultipart) throw new Exception();
                    if (!string.IsNullOrEmpty(boundary)) return ParseWithBoundary(boundary, r);
                    return true;
                }
                if (isMultipart && isContentBegin)
                {
                    boundary = line;
                    data = ParseWithBoundary(boundary, reader);
                    return false;
                }
                return true;
            });
            if (data == null) throw new Exception();
            return data;
        }

        private MultipartFormData ParseWithBoundary(string boundary, StreamLineReader reader)
        {
            throw new NotImplementedException();
        }

        private MultipartFormData ParseWithBoundary(string boundary, StreamReader reader)
        {
            string line = null;
            IList<MultipartFormDataItem> content = new List<MultipartFormDataItem>(10);
            MultipartFormDataItem item = new MultipartFormDataItem();
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("Content-Disposition:"))
                {
                    if (!line.Contains("form-data")) throw new Exception();
                    var nameMatch = _nameRegex.Match(line);
                    var filenameMatch = _filenameRegex.Match(line);
                    if (nameMatch.Success) item.Name = nameMatch.Value;
                    if (filenameMatch.Success) item.Filename = filenameMatch.Value;
                }
                else if (line.StartsWith("Content-Type:"))
                {
                    var contentTypeMatch = _contentTypeRegex.Match(line);
                    if (contentTypeMatch.Success) item.ContentType = contentTypeMatch.Value.Trim();
                    var charsetMatch = _charsetRegex.Match(line);
                    if (charsetMatch.Success) item.Charset = charsetMatch.Value.Trim();
                }
                else if (line.StartsWith("Content-Transfer-Encoding:"))
                {
                    var contentTransferEncodingMatch = _contentTransferEncoding.Match(line);
                    if (contentTransferEncodingMatch.Success)
                        item.ContentTransferEncoding = contentTransferEncodingMatch.Value.Trim();
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    item.Content = ReadToNextBoundary(boundary, reader);
                    content.Add(item);
                    item = new MultipartFormDataItem();
                }
            }
            return new MultipartFormData() {Content = content.ToArray()};
        }

        private byte[] ReadToNextBoundary(string boundary, StreamReader reader)
        {
            int b = 0;
            var buffer = new List<byte>(1000);
            var data = new List<byte>(1000);
            while (true)
            {
                if (b == -1) throw new Exception("Could not find trailing boundary: " + boundary);
                b = reader.Read();
                if (b != -1) buffer.Add((byte)b);
                if (b == '\n' || b == -1)
                {
                    var line = Encoding.Default.GetString(buffer.ToArray());
                    if (line.Contains(boundary))
                    {
                        data.RemoveRange(data.Count - 3, 2);
                        return data.ToArray();
                    }
                    else
                    {
                        data.AddRange(buffer);
                        buffer.Clear();
                    }
                }
            }
        }
    }
}
