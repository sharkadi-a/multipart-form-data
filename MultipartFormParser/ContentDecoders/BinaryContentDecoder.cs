using System.Linq;
using MultipartFormParser.Exceptions;

namespace MultipartFormParser.ContentDecoders
{
    /// <summary>
    /// This class is a common binary data decoder. First, it decodes data from transfer encdoing, and then it simply returns data from content of <see cref="MultipartFormDataItem"/>
    /// </summary>
    internal class BinaryContentDecoder : IContentDecoder<byte[]>
    {
        public string[] MimeContentTypes { get { return new string[0]; } }
        public MultipartFormDataItem MultipartFormDataItem { get; set; }

        public BinaryContentDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            MultipartFormDataItem = multipartFormDataItem;
        }

        public BinaryContentDecoder()
        {
            
        }

        public bool CanDecode 
        {
            get
            {
                if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
                return (string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding) ||
                        ContentTransferDecoderFactory.FindType<byte>(MultipartFormDataItem.ContentTransferEncoding) !=
                        null) &&
                       string.IsNullOrEmpty(MultipartFormDataItem.Charset);
            }
        }

        public byte[] Decode()
        {
            if (MultipartFormDataItem == null) throw new ContentDecodingException("MultipartFormDataItem must not be null");
            if (!CanDecode) throw new ContentDecodingException("Could not decode content");
            var bytes = MultipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding))
            {
                var byteDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(MultipartFormDataItem);
                if (byteDecoder == null) throw new ContentDecodingException("Could not decode content");
                bytes = byteDecoder.Decode(MultipartFormDataItem).ToArray();
            }
            return bytes;
        }
    }
}
