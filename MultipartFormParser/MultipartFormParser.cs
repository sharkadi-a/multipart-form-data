using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MultipartFormParser.Exceptions;
using MultipartFormParser.Helpers;

namespace MultipartFormParser
{
    /// <summary>
    /// Multipart form parser from HTTP request
    /// </summary>
    public sealed class MultipartFormParser
    {
        private readonly Regex _boundaryRegex = new Regex(@"(?<=boundary=)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);
        private readonly Regex _nameRegex = new Regex(@"(?<=name\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private readonly Regex _filenameRegex = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private readonly Regex _charsetRegex = new Regex(@"(?<=charset\=\"")(.*?)(?=\"")", RegexOptions.IgnoreCase);
        private readonly Regex _contentTypeRegex = new Regex(@"(?<=Content\-Type:)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);
        private readonly Regex _contentTransferEncoding = new Regex(@"(?<=Content\-Transfer\-Encoding:)(.*?)(?=(\;)|$)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Result of form parsing (multi-part form content)
        /// </summary>
        public MultipartFormData Data { get; private set; }

        /// <summary>
        /// Parses multipart form data raw HTTP-request from stream
        /// </summary>
        /// <param name="stream"></param>
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
                    var boundaryMatch = _boundaryRegex.Match(line);
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
                    if (!line.Contains("form-data")) throw new MultiPartFormParsingException("The stream is not multi-part form data");
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
