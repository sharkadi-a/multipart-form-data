using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    internal static class ReflectionHelper
    {
        public static Tuple<ContentTransferDecoderTypeAttribute, Type>[] FindAllTypesMatching()
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.GetCustomAttribute<ContentTransferDecoderTypeAttribute>() != null)
                    .Select(
                        t =>
                            new Tuple<ContentTransferDecoderTypeAttribute, Type>(
                                t.GetCustomAttribute<ContentTransferDecoderTypeAttribute>(), t))
                    .ToArray();
        }

        public static IEnumerable<Type> Filter(IEnumerable<Tuple<ContentTransferDecoderTypeAttribute, Type>> types,
            string contentTransferTypeName, Type enumerationType)
        {
            IEnumerable<Tuple<ContentTransferDecoderTypeAttribute, Type>> select = types;
            if (!string.IsNullOrEmpty(contentTransferTypeName))
                select = select.Where(t => t.Item1.ContentTransferEncodingName == contentTransferTypeName);
            if (enumerationType != null) select = select.Where(t => t.Item1.EnumerationType == enumerationType);
            return select.Select(t => t.Item2);
        }
    }
}
