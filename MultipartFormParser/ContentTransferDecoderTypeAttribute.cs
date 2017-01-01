using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public class ContentTransferDecoderTypeAttribute : Attribute
    {
        public Type EnumerationType { get; private set; }
        public string ContentTransferEncodingName { get; private set; }

        public ContentTransferDecoderTypeAttribute(string contentTransferEncodingName, Type enumerationType)
        {
            EnumerationType = enumerationType;
            ContentTransferEncodingName = contentTransferEncodingName;
        }
    }
}
