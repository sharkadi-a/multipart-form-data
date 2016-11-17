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
        private readonly Regex boundaryRegex = new Regex(@"(?<=boundary=)(.*?)(?=(\r\n\r\n)|\;)");
        private Regex _nameRegex = new Regex(@"(?<=name\=\"")(.*?)(?=\"")");
        private Regex _filenameRegex = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
        private Regex _contentTypeRegex = new Regex(@"(?<=Content\-Type:)(.*?)(?=(\r\n\r\n)|\;)");


        private byte[] ReadBytes(Stream stream)
        {
            int b = 0;
            IList<byte> bytes = new List<byte>(1000);
            while ((b = stream.ReadByte()) != -1)
            {
                bytes.Add((byte) b);
            }
            return bytes.ToArray();
        }

        public MultipartForm(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new Exception("Stream should support reading");
            _stream = stream;
        }

        public MultipartFormContent Parse()
        {
            if (!_stream.CanRead) throw new Exception("Stream should support reading");
            using (var r = new StreamReader(_stream, Encoding.ASCII))
            {
                string line = null, boundary = null;
                bool isMultipart = false;
                while ((line = r.ReadLine()) != null)
                {
                    if (line.StartsWith("Content-Type:"))
                    {
                        isMultipart = true;
                        if (!line.Contains("multipart/form-data")) throw new Exception();
                        var boundaryMatch = boundaryRegex.Match(line);
                        if (boundaryMatch.Success) boundary = boundaryMatch.Value;
                    }
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (isMultipart) throw new Exception();
                        if (!string.IsNullOrEmpty(boundary)) return ParseBoundary(boundary, r);
                        continue;
                    }
                    if (isMultipart)
                    {
                        boundary = line;
                        return ParseBoundary(boundary, r);
                    }
                }
            }
            throw new Exception();
        }

        private MultipartFormContent ParseBoundary(string boundary, StreamReader reader)
        {
            string line = null;
            IList<MultipartFormContentItem> content = new List<MultipartFormContentItem>(10);
            MultipartFormContentItem item = new MultipartFormContentItem();
            var beginData = false;
            StringBuilder data = new StringBuilder(1000);
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(boundary))
                {
                    item.Content = Encoding.ASCII.GetBytes(data.ToString());
                    content.Add(item);
                    item = new MultipartFormContentItem();
                    data = new StringBuilder(1000);
                }
                if (beginData)
                {
                    data.Append(line);
                }
                if (line.StartsWith("Content-Disposition:"))
                {
                    if (!line.Contains("form-data")) throw new Exception();
                    var nameMatch = _nameRegex.Match(line);
                    var filenameMatch = _filenameRegex.Match(line);
                    if (nameMatch.Success) item.Name = nameMatch.Value;
                    if (filenameMatch.Success) item.Filename = filenameMatch.Value;
                }
                if (line.StartsWith("Content-Type:"))
                {
                    var contentTypeMatch = _contentTypeRegex.Match(line);
                    if (contentTypeMatch.Success) item.ContentType = contentTypeMatch.Value;
                }
                if (string.IsNullOrWhiteSpace(line)) beginData = true;
            }
            return new MultipartFormContent() {MultipartFormContentItems = content.ToArray()};
        }
    }
}
