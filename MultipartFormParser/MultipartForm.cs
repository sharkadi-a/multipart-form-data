using System;
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
            using (var r = new StreamReader(_stream, Encoding.ASCII))
            {
                string line = null, boundary = null;
                bool isMultipart = false, isContentBegin = false;
                while ((line = r.ReadLine()) != null)
                {
                    if (line.StartsWith("Content-Type:"))
                    {
                        isMultipart = true;
                        if (!line.Contains("multipart/form-data")) throw new Exception();
                        var boundaryMatch = boundaryRegex.Match(line);
                        if (boundaryMatch.Success) boundary = boundaryMatch.Value;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        isContentBegin = true;
                        if (!isMultipart) throw new Exception();
                        if (!string.IsNullOrEmpty(boundary)) return ParseBoundary(boundary, r);
                        continue;
                    }
                    if (isMultipart && isContentBegin)
                    {
                        boundary = line;
                        return ParseBoundary(boundary, r);
                    }
                }
            }
            throw new Exception();
        }

        private MultipartFormData ParseBoundary(string boundary, StreamReader reader)
        {
            string line = null;
            IList<MultipartFormDataItem> content = new List<MultipartFormDataItem>(10);
            MultipartFormDataItem item = new MultipartFormDataItem();
            var appendData = false;
            StringBuilder data = new StringBuilder(1000);
            bool first = true;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(boundary))
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    item.Content = Encoding.ASCII.GetBytes(data.ToString());
                    content.Add(item);
                    item = new MultipartFormDataItem();
                    data = new StringBuilder(1000);
                    appendData = false;
                }
                else if (appendData)
                {
                    data.Append(line);
                }
                else if (line.StartsWith("Content-Disposition:"))
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
                    if (contentTypeMatch.Success) item.ContentType = contentTypeMatch.Value;
                    var charsetMatch = _charsetRegex.Match(line);
                    if (charsetMatch.Success) item.Charset = charsetMatch.Value;
                }
                else if (line.StartsWith("Content-Transfer-Encoding:"))
                {
                    var contentTransferEncodingMatch = _contentTransferEncoding.Match(line);
                    if (contentTransferEncodingMatch.Success)
                        item.ContentTransferEncoding = contentTransferEncodingMatch.Value;
                }
                else if (string.IsNullOrWhiteSpace(line)) appendData = true;
            }
            return new MultipartFormData() {Content = content.ToArray()};
        }
    }
}
