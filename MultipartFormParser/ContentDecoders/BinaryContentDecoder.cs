using System;
using System.Linq;

namespace MultipartFormParser.ContentDecoders
{
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
                if (MultipartFormDataItem == null) throw new Exception();
                return (string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding) ||
                        ContentTransferDecoderFactory.FindType<byte>(MultipartFormDataItem.ContentTransferEncoding) !=
                        null) &&
                       string.IsNullOrEmpty(MultipartFormDataItem.Charset);
            }
        }

        public byte[] Decode()
        {
            if (MultipartFormDataItem == null) throw new Exception();
            if (!CanDecode) throw new Exception();
            var bytes = MultipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(MultipartFormDataItem.ContentTransferEncoding))
            {
                var byteDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(
                        MultipartFormDataItem.ContentTransferEncoding);
                if (byteDecoder == null) throw new Exception();
                bytes = byteDecoder.Decode(MultipartFormDataItem).ToArray();
            }
            return bytes;
        }
    }
}
