using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public sealed class BinaryEncoder
    {
        private readonly MultipartFormDataItem _multipartFormDataItem;

        public BinaryEncoder(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            _multipartFormDataItem = multipartFormDataItem;
        }

        public bool CanEncode
        {
            get
            {
                return (string.IsNullOrEmpty(_multipartFormDataItem.ContentTransferEncoding) ||
                        ContentTransferDecoderFactory.FindType<byte>(_multipartFormDataItem.ContentTransferEncoding) !=
                        null) &&
                       string.IsNullOrEmpty(_multipartFormDataItem.Charset);
            }
        }

        public byte[] Encode()
        {
            if (!CanEncode) throw new Exception();
            var bytes = _multipartFormDataItem.Content;
            if (!string.IsNullOrEmpty(_multipartFormDataItem.ContentTransferEncoding))
            {
                var byteDecoder =
                    ContentTransferDecoderFactory.FindAndCreateInstance<byte>(
                        _multipartFormDataItem.ContentTransferEncoding);
                if (byteDecoder == null) throw new Exception();
                bytes = byteDecoder.Decode(_multipartFormDataItem).ToArray();
            }
            return bytes;
        }
    }
}
