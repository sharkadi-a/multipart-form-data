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
    /// <summary>
    /// Helper class for simple XML deserialization into object from multi-part form data item
    /// </summary>
    public sealed class MultipartFormXmlDeserializer
    {
        private readonly MultipartFormDataItem _multipartFormDataItem;
        private readonly IContentDecoder<string> _decoder;

        /// <summary>
        /// Creates a new instance of <see cref="MultipartFormXmlDeserializer"/> using specified form data item <paramref name="multipartFormDataItem"/> 
        /// </summary>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        public MultipartFormXmlDeserializer(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            _decoder = ContentDecoderFactory.CreateTextDecoder(multipartFormDataItem);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MultipartFormXmlDeserializer"/> using specified form data item <paramref name="multipartFormDataItem"/> and text content decoder <paramref name="textContentDecoder"/> (instead of default)
        /// </summary>
        /// <param name="multipartFormDataItem"></param>
        /// <param name="textContentDecoder"></param>
        public MultipartFormXmlDeserializer(MultipartFormDataItem multipartFormDataItem, IContentDecoder<string> textContentDecoder)
        {
            _multipartFormDataItem = multipartFormDataItem;
            _decoder = textContentDecoder;
        }

        /// <summary>
        /// Returns boolean value inditcating could multi-part form data item contents be deserailized into type <paramref name="objectType"/>
        /// </summary>
        /// <param name="objectType">Type to test deserialization into</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deserializes multi-part form data item into object of the specified type <paramref name="objectType"/>
        /// </summary>
        /// <param name="objectType">Type of XML serializer</param>
        /// <returns>Instance of an object</returns>
        public object Deserialize(Type objectType)
        {
            var deserializer = new XmlSerializer(objectType);
            var obj = deserializer.Deserialize(new MemoryStream(Encoding.Default.GetBytes(_decoder.Decode())));
            return obj;        
        }

        /// <summary>
        /// Deserializes multi-part form data item into instance of <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">Type into which deserialization shall be perfomed</typeparam>
        /// <returns>Instance of a type <see cref="T"/></returns>
        public T Deserialize<T>() where T : class
        {
            return (T) Deserialize(typeof (T));
        }
    }
}
