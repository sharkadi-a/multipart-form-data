using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public sealed class TextEncoder
    {
        private readonly MultipartFormDataItem _multipartFormDataItem;

        public TextEncoder(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            _multipartFormDataItem = multipartFormDataItem;
        }

        public bool CanEncode
        {
            get
            {
                return Encoding != null && IsText() &&
                       (string.IsNullOrEmpty(_multipartFormDataItem.ContentTransferEncoding) ||
                        ContentTransferDecoderFactory.FindType<char>(_multipartFormDataItem.ContentTransferEncoding) !=
                        null ||
                        ContentTransferDecoderFactory.FindType<byte>(_multipartFormDataItem.ContentTransferEncoding) !=
                        null);
            }
        }

        public Encoding Encoding
        {
            get
            {
                return GetEncodingFromCharSet();
            }
        }

        private bool IsText()
        {
            if (string.IsNullOrEmpty(_multipartFormDataItem.ContentType)) return false;
            return _multipartFormDataItem.ContentType.ToLower().ContainsAny("text", "xml", "javascript", "json");
        }

        private Encoding GetEncodingFromCharSet()
        {
            if (string.IsNullOrEmpty(_multipartFormDataItem.Charset))
            {
                try
                {
                    return Encoding.GetEncoding(_multipartFormDataItem.Charset);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
            return Encoding.ASCII;
        }

        public string Encode()
        {
            if (!CanEncode) throw new Exception();
            byte[] bytes = _multipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(_multipartFormDataItem.ContentTransferEncoding))
            {
                var charDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<char>(
                        _multipartFormDataItem.ContentTransferEncoding);
                if (charDecoder != null)
                {
                    return new string(charDecoder.Decode(_multipartFormDataItem).ToArray());
                }
                var byteDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(
                        _multipartFormDataItem.ContentTransferEncoding);
                if (byteDecoder == null) throw new Exception();
                bytes = byteDecoder.Decode(_multipartFormDataItem).ToArray();
            }
            return GetEncodingFromCharSet().GetString(bytes);
        }

        public override string ToString()
        {
            return Encode();
        }
    }
}
