using System;
using System.Linq;
using MultipartFormParser.ContentDecoders;
using MultipartFormParser.Helpers;

namespace MultipartFormParser
{
    /// <summary>
    /// Creates content decoder from HTTP request, which returns specific managed type.
    /// </summary>
    public static class ContentDecoderFactory
    {
        private static readonly object _lock = new object();
        private static Tuple<ContentDecoderTypeAttribute, Type>[] _types;

        static ContentDecoderFactory()
        {
            Refresh();
        }

        /// <summary>
        /// Refreshes list of types annotated with ContentDecoderTypeAttribute through reflection. Use this method to add custom content decoders
        /// </summary>
        public static void Refresh()
        {
            lock (_lock)
                _types = ReflectionHelper.FindAllTypesMatching<ContentDecoderTypeAttribute>();
        }

        /// <summary>
        /// Creates common binary decoder for any content, which is binary by nature and returns byte array
        /// </summary>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        /// <returns>Binary content decoder</returns>
        public static IContentDecoder<byte[]> CreateBinaryDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            return new BinaryContentDecoder(multipartFormDataItem);
        }

        /// <summary>
        /// Creates common text decoder for any content, which can be represented as text data and returns string
        /// </summary>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        /// <returns>String content decoder</returns>
        public static IContentDecoder<string> CreateTextDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            return new TextContentDecoder(multipartFormDataItem);
        }

        /// <summary>
        /// Creates image decoder for the form data with content base type "image" and returns <see cref="System.Drawing.Image"/>
        /// </summary>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        /// <returns><see cref="ImageContentDecoder"/> decoder</returns>
        public static ImageContentDecoder CreateImageContentDecoder(MultipartFormDataItem multipartFormDataItem)
        {
            return new ImageContentDecoder(multipartFormDataItem);
        } 

        /// <summary>
        /// Finds type which implements <see cref="IContentDecoder{TClrType}"/> interface for the specific Content-Type <paramref name="mimeContentTypeName"/>
        /// </summary>
        /// <typeparam name="TElement">Managed type into which decoding should be perfomed</typeparam>
        /// <param name="mimeContentTypeName">Content-Type MIME type</param>
        /// <returns>Type which implements <see cref="IContentDecoder{TClrType}"/> or null, if such not found</returns>
        public static Type FindType<TElement>(string mimeContentTypeName)
        {
            return ReflectionHelper.Filter(_types, typeof (TElement), mimeContentTypeName).FirstOrDefault();
        }

        /// <summary>
        /// Finds and creates instance of a type which implements <see cref="IContentDecoder{TClrType}"/> interface for specific Content-Type in <paramref name="multipartFormDataItem"/>
        /// </summary>
        /// <typeparam name="TClrType">Managed type into which decoding should be perfomed</typeparam>
        /// <param name="multipartFormDataItem">Multi-part form data item</param>
        /// <returns>Instance of a type which decodes form content into type <see cref="TClrType"/> or null, if such not found</returns>
        public static IContentDecoder<TClrType> FindAndCreateInstance<TClrType>(MultipartFormDataItem multipartFormDataItem)
        {
            lock (_lock)
            {
                var type = FindType<TClrType>(multipartFormDataItem.ContentType);
                if (type == null) return null;
                var ctors = ReflectionHelper.GetConstructors(type);
                if (ctors.Any(c => c.Parameters.Any(p => p.ParameterType == typeof (MultipartFormDataItem))))
                {
                    return (IContentDecoder<TClrType>) Activator.CreateInstance(type, multipartFormDataItem);
                }
                else
                {
                    var inst = (IContentDecoder<TClrType>) Activator.CreateInstance(type);
                    inst.MultipartFormDataItem = multipartFormDataItem;
                    return inst;
                }
            }
        } 
    }
}
