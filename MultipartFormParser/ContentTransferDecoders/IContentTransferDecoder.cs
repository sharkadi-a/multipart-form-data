using System.Collections.Generic;

namespace MultipartFormParser.ContentTransferDecoders
{
    public interface IContentTransferDecoder<out TEnumeration>
    {
        string ContentTransferEncodingName { get; }
        IEnumerable<TEnumeration> Decode(MultipartFormDataItem multipartFormDataItem);
    }
}
