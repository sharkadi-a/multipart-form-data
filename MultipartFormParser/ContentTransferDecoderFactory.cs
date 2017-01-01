using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    public static class ContentTransferDecoderFactory
    {
        private static Tuple<ContentTransferDecoderTypeAttribute, Type>[] _contentTypes;

        static ContentTransferDecoderFactory()
        {
            Refresh();
        }

        public static void Refresh()
        {
            _contentTypes = ReflectionHelper.FindAllTypesMatching();            
        }

        public static IContentTransferDecoder<char> Create7BitDecoder()
        {
            return new _7BitDecoder();
        }

        public static IContentTransferDecoder<char> Create8BitDecoder()
        {
            return new _8BitDecoder();
        }

        public static IContentTransferDecoder<char> CreateQuotedPrintableDecoder()
        {
            return new QuotedPrintableDecoder();
        }

        public static IContentTransferDecoder<byte> CreateBase64Decoder()
        {
            return new Base64Decoder();
        }

        public static IContentTransferDecoder<byte> CreateBinaryDecoder()
        {
            return new BinaryDecoder();
        }

        public static Type[] FindAllForEnumeration<TEnumeration>()
        {
            return ReflectionHelper.Filter(_contentTypes, null, typeof(TEnumeration)).ToArray();
        }

        public static IContentTransferDecoder<TEnumeration> CreateInstance<TEnumeration>(Type contentTransferDecoderType)
        {
            if (_contentTypes.All(t => t.Item2 != contentTransferDecoderType)) throw new Exception();
            return (IContentTransferDecoder<TEnumeration>)Activator.CreateInstance(contentTransferDecoderType);
        }

        public static IContentTransferDecoder<TEnumeration> FindAndCreateInstance<TEnumeration>(string contentTransferName)
        {
            var type =
                ReflectionHelper.Filter(_contentTypes, contentTransferName, typeof(TEnumeration))
                    .FirstOrDefault();
            if (type == null) return null;
            if (type != typeof(IContentTransferDecoder<TEnumeration>)) throw new Exception();
            return (IContentTransferDecoder<TEnumeration>)Activator.CreateInstance(type);
        } 
    }
}
