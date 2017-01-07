using System;

namespace MultipartFormParser.ContentDecoders
{
    /// <summary>
    /// This attribute is being used by ContentDecoderFactory to find appropriate decoders for specified MIME types and CLR types
    /// </summary>
    public class ContentDecoderTypeAttribute : Attribute
    {
        /// <summary>
        /// CLR type into which decoding of the annotated decoder should perform
        /// </summary>
        public Type ClrType { get; private set; }

        /// <summary>
        /// MIME types which are supported by the annotated decoder
        /// </summary>
        public string[] MimeContentTypes { get; private set; }

        /// <summary>
        /// Adds an attribute to the decoder
        /// </summary>
        /// <param name="clrType">CLR type into which decoding of the annotated decoder should perform</param>
        /// <param name="mimeContentTypes">MIME types which are supported by the annotated decoder</param>
        public ContentDecoderTypeAttribute(Type clrType, params string[] mimeContentTypes)
        {
            if (clrType == null) throw new ArgumentNullException("clrType");
            if (mimeContentTypes == null) throw new ArgumentNullException("mimeContentTypes");
            ClrType = clrType;
            MimeContentTypes = mimeContentTypes;
        }
    }
}
