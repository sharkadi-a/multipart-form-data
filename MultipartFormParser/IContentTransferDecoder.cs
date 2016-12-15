using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public interface IContentTransferDecoder<T>
    {
        string ContentTransferEncodingName { get; }
        IEnumerable<T> Decode(MultipartFormDataItem multipartFormDataItem);
    }
}
