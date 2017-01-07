using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MultipartFormParser.Exceptions;
using MultipartFormParser.Helpers;

namespace MultipartFormParser
{
    public class MultipartForm
    {
        private readonly Regex boundaryRegex = new Regex(@"(?<=boundary=)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);
        private Regex _nameRegex = new Regex(@"(?<=name\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private Regex _filenameRegex = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private Regex _charsetRegex = new Regex(@"(?<=charset\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private Regex _contentTypeRegex = new Regex(@"(?<=Content\-Type:)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);
        private Regex _contentTransferEncoding = new Regex(@"(?<=Content\-Transfer\-Encoding:)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);

        public MultipartFormData Data { get; private set; }

        public void Parse(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new Exception("Stream should support reading");
            var reader = new StreamLineReader(stream);

            string boundary = null;
            bool isMultipart = false, isContentBegin = false;
            MultipartFormData data = null;
            reader.Read((c, bytes) =>
            {
                string line = Encoding.ASCII.GetString(bytes);
                if (line.StartsWith("Content-Type:"))
                {
                    isMultipart = true;
                    if (!line.Contains("multipart/form-data")) throw new MultiPartFormParsingException("The stream is not multi-part form data");
                    var boundaryMatch = boundaryRegex.Match(line);
                    if (boundaryMatch.Success) boundary = boundaryMatch.Value.Trim();
                    return true;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    isContentBegin = true;
                    if (!isMultipart) throw new MultiPartFormParsingException("The stream is not multi-part form data");
                    if (!string.IsNullOrEmpty(boundary))
                    {
                        data = ParseWithBoundary(boundary, reader);
                        return false;
                    }
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
            if (data == null) throw new MultiPartFormParsingException("The stream is not multi-part form data");
            Data = data;
        }

        private MultipartFormData ParseWithBoundary(string boundary, StreamLineReader reader)
        {
            IList<MultipartFormDataItem> content = new List<MultipartFormDataItem>(10);
            MultipartFormDataItem item = new MultipartFormDataItem();
            bool found = false;
            reader.Read( (c, bytes) =>
            {
                string line = Encoding.ASCII.GetString(bytes);
                if (line.StartsWith("Content-Disposition:"))
                {
                    if (!line.Contains("form-data")) throw new Exception();
                    var nameMatch = _nameRegex.Match(line);
                    var filenameMatch = _filenameRegex.Match(line);
                    if (nameMatch.Success) item.Name = nameMatch.Value;
                    if (filenameMatch.Success) item.Filename = filenameMatch.Value;
                    found = true;
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
                else if (found && string.IsNullOrWhiteSpace(line))
                {
                    item.Content = ReadToNextBoundary(boundary, reader);
                    content.Add(item);
                    item = new MultipartFormDataItem();
                    found = false;
                }
                return true;
            });
            return new MultipartFormData() { Content = content.ToArray() };
        }

        private byte[] ReadToNextBoundary(string boundary, StreamLineReader reader)
        {
            var data = new List<byte>(1000);
            reader.Read((c, bytes) =>
            {
                string line = Encoding.ASCII.GetString(bytes.ToArray());
                if (line.Contains(boundary))
                {
                    data.RemoveRange(data.Count - 2, 2);
                    return false;
                }
                data.AddRange(bytes);
                return true;
            });
            return data.ToArray();
        }
    }
}
