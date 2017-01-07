using System;
using System.Linq;
using System.Text;
using MultipartFormParser.Exceptions;
using MultipartFormParser.Helpers;

namespace MultipartFormParser.ContentDecoders
{
    internal class TextContentDecoder : IContentDecoder<string>
    {
        public string[] MimeContentTypes { get { return new string[0];} }
        public MultipartFormDataItem MultipartFormDataItem { get; set; }

        public bool CanDecode
        {
            get
            {
                return Encoding != null && IsText() &&
                       (string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding) ||
                        ContentTransferDecoderFactory.FindType<char>(MultipartFormDataItem.ContentTransferEncoding) !=
                        null ||
                        ContentTransferDecoderFactory.FindType<byte>(MultipartFormDataItem.ContentTransferEncoding) !=
                        null);
            }
        }

        public TextContentDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            MultipartFormDataItem = multipartFormDataItem;
        }

        public TextContentDecoder()
        {
            
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
            if (string.IsNullOrEmpty(MultipartFormDataItem.ContentType)) return false;
            return MultipartFormDataItem.ContentType.ToLower().ContainsAny("text", "xml", "javascript", "json");
        }

        private Encoding GetEncodingFromCharSet()
        {
            if (string.IsNullOrEmpty(MultipartFormDataItem.Charset))
            {
                //return Encoding.GetEncoding(MultipartFormDataItem.Charset);
                var encoding = EncodingResolver.FindEncoding(MultipartFormDataItem.Charset);
                if (encoding == null) throw new UnknownEncodingException(MultipartFormDataItem.Charset);
                return encoding;
            }
            return Encoding.ASCII;
        }

        public string Decode()
        {
            if (!CanDecode) throw new Exception();
            byte[] bytes = MultipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding))
            {
                var charDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<char>(
                        MultipartFormDataItem.ContentTransferEncoding);
                if (charDecoder != null)
                {
                    return new string(charDecoder.Decode(MultipartFormDataItem).ToArray());
                }
                var byteDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(
                        MultipartFormDataItem.ContentTransferEncoding);
                if (byteDecoder == null) throw new Exception();
                bytes = byteDecoder.Decode(MultipartFormDataItem).ToArray();
            }
            return GetEncodingFromCharSet().GetString(bytes);
        }

        public override string ToString()
        {
            return Decode();
        }
    }
}
