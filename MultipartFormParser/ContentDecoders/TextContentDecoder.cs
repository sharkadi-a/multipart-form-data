using System.Linq;
using System.Text;
using MultipartFormParser.Exceptions;
using MultipartFormParser.Helpers;

namespace MultipartFormParser.ContentDecoders
{
    /// <summary>
    /// This class is a common text decoder. First of all, it decodes data from transfer encdoing. It supports decoding from any data, that can be considered as text, thus, Content-Type should contain text, xml, javascript or json type classes.
    /// </summary>
    internal class TextContentDecoder : IContentDecoder<string>
    {
        public string[] MimeContentTypes { get { return new string[0];} }
        public MultipartFormDataItem MultipartFormDataItem { get; set; }

        public bool CanDecode
        {
            get
            {
                if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
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
            if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
            if (!CanDecode) throw new ContentDecodingException("Could not decode content");
            byte[] bytes = MultipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding))
            {
                var charDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<char>(MultipartFormDataItem);
                if (charDecoder != null)
                {
                    return new string(charDecoder.Decode(MultipartFormDataItem).ToArray());
                }
                var byteDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(MultipartFormDataItem);
                if (byteDecoder == null) throw new ContentDecodingException("Could not decode content");
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
