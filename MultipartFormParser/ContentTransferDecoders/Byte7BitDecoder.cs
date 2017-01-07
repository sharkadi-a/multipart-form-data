using System.Collections.Generic;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// This class decodes data from 7bit content transfer encoding into array of bytes. Naturally, there is no difference between 7bit, 8bit or binary transfer encodings for transfer environment which is working with data as a stream of bytes
    /// </summary>
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
