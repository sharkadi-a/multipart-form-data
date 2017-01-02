using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipartFormParser.ContentDecoders;
using MultipartFormParser.Helpers;

namespace MultipartFormParser
{
    public static class ContentDecoderFactory
    {
        private static Tuple<ContentDecoderTypeAttribute, Type>[] _types;

        static ContentDecoderFactory()
        {
            Refresh();
        }

        public static void Refresh()
        {
            _types = ReflectionHelper.FindAllTypesMatching<ContentDecoderTypeAttribute>();
        }

        public static IContentDecoder<byte[]> CreateGeneralBinaryDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            return new BinaryContentDecoder(multipartFormDataItem);
        }

        public static IContentDecoder<string> CreateGeneralTextDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            return new TextContentDecoder(multipartFormDataItem);
        }

        public static ImageContentDecoder CreateImageContentDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            return new ImageContentDecoder(multipartFormDataItem);
        } 

        public static Type FindType<TElement>(string mimeContentTypeName)
        {
            return ReflectionHelper.Filter(_types, typeof (TElement), mimeContentTypeName).FirstOrDefault();
        }

        public static IContentDecoder<TElement> FindAndCreateInstance<TElement>(MultipartFormDataItem multipartFormDataItem)
        {
            var type = FindType<TElement>(multipartFormDataItem.ContentType);
            if (type == null) return null;
            var ctors = ReflectionHelper.GetConstructors(type);
            if (ctors.Any(c => c.Parameters.Any(p => p.ParameterType == typeof (MultipartFormDataItem))))
            {
                return (IContentDecoder<TElement>)Activator.CreateInstance(type, multipartFormDataItem);
            }
            else
            {
                var inst = (IContentDecoder<TElement>)Activator.CreateInstance(type);
                inst.MultipartFormDataItem = multipartFormDataItem;
                return inst;
            }
        } 
    }
}
