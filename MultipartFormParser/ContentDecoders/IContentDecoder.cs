using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser.ContentDecoders
{
    public interface IContentDecoder<out TElement>
    {
        string[] MimeContentTypes { get; }
        MultipartFormDataItem MultipartFormDataItem { get; set; }
        bool CanDecode { get; }
        TElement Decode();
    }
}
