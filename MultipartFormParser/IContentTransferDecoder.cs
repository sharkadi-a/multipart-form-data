using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public interface IContentTransferDecoder<out TEnumeration>
    {
        string ContentTransferEncodingName { get; }
        IEnumerable<TEnumeration> Decode(MultipartFormDataItem multipartFormDataItem);
    }
}
