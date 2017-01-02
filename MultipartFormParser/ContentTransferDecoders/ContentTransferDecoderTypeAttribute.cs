using System;

namespace MultipartFormParser.ContentTransferDecoders
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
