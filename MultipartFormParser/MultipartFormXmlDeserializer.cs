using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MultipartFormParser.ContentDecoders;

namespace MultipartFormParser
{
    public sealed class MultipartFormXmlDeserializer
    {
        private readonly MultipartFormDataItem _multipartFormDataItem;
        private readonly IContentDecoder<string> _decoder;

        public MultipartFormXmlDeserializer(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            _decoder = ContentDecoderFactory.CreateGeneralTextDecoder(multipartFormDataItem);
        }

        public MultipartFormXmlDeserializer(MultipartFormDataItem multipartFormDataItem, IContentDecoder<string> textContentDecoder)
        {
            _multipartFormDataItem = multipartFormDataItem;
            _decoder = textContentDecoder;
        }

        public bool CanDeserialize(Type objectType)
        {
            if (!_decoder.CanDecode) return false;
            var deserializer = new XmlSerializer(objectType);
            try
            {
                using (var xmlReader = new XmlTextReader(new MemoryStream(Encoding.Default.GetBytes(_decoder.Decode())))
                    )
                {
                    return deserializer.CanDeserialize(xmlReader);
                }
            }
            catch
            {
                return false;
            }
        }

        public object Deserialize(Type objectType)
        {
            var deserializer = new XmlSerializer(objectType);
            var obj = deserializer.Deserialize(new MemoryStream(Encoding.Default.GetBytes(_decoder.Decode())));
            return obj;        
        }

        public T Deserialize<T>() where T : class
        {
            return (T) Deserialize(typeof (T));
        }
    }
}
