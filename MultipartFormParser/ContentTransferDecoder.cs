using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public abstract class ContentTransferDecoder<T>
    {
        public abstract string ContentTransferEncodingName { get; }
        public abstract IEnumerable<T> Decode(MultipartFormDataItem multipartFormDataItem);
    }
}
