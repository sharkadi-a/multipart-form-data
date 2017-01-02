using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser.ContentDecoders
{
    public class ContentDecoderTypeAttribute : Attribute
    {
        public Type ElementType { get; private set; }
        public string[] MimeContentTypes { get; private set; }

        public ContentDecoderTypeAttribute(Type elementType, params string[] mimeContentTypes)
        {
            if (elementType == null) throw new ArgumentNullException("elementType");
            if (mimeContentTypes == null) throw new ArgumentNullException("mimeContentTypes");
            ElementType = elementType;
            MimeContentTypes = mimeContentTypes;
        }
    }
}
