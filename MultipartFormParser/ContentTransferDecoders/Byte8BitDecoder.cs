using System.Collections.Generic;

namespace MultipartFormParser.ContentTransferDecoders
{
    [ContentTransferDecoderType("8bit", typeof(byte))]
    internal class Byte8BitDecoder : IContentTransferDecoder<byte>
    {
        public string ContentTransferEncodingName { get { return "8bit"; } }
        public IEnumerable<byte> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            var decoder = new BinaryDecoder();
            return decoder.Decode(multipartFormDataItem);
        }
    }
}
