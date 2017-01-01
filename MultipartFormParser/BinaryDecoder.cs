using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    [ContentTransferDecoderType("binary", typeof(byte))]
    internal class BinaryDecoder : IContentTransferDecoder<byte>
    {
        public string ContentTransferEncodingName { get { return "binary"; } }
        public IEnumerable<byte> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            return multipartFormDataItem.Content ?? Enumerable.Empty<byte>();
        }
    }
}
