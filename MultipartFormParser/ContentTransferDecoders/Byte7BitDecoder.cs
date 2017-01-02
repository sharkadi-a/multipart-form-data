using System.Collections.Generic;

namespace MultipartFormParser.ContentTransferDecoders
{
    [ContentTransferDecoderType("7bit", typeof (byte))]
    internal class Byte7BitDecoder : IContentTransferDecoder<byte>
    {
        public string ContentTransferEncodingName
        {
            get { return "7bit"; }
        }

        public IEnumerable<byte> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            var decoder = new BinaryDecoder();
            return decoder.Decode(multipartFormDataItem);
        }
    }
}
