using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultipartFormParser.ContentDecoders;
using MultipartFormParser.ContentTransferDecoders;

namespace MultipartFormParser.Helpers
{
    internal static class ReflectionHelper
    {
        public static Tuple<T, Type>[] FindAllTypesMatching<T>() where T:Attribute
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.GetCustomAttribute<T>() != null)
                    .Select(
                        t =>
                            new Tuple<T, Type>(
                                t.GetCustomAttribute<T>(), t))
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

        public static IEnumerable<Type> Filter(IEnumerable<Tuple<ContentDecoderTypeAttribute, Type>> types,
            Type elementType, params string[] mimeContentTypes)
        {
            IEnumerable<Tuple<ContentDecoderTypeAttribute, Type>> select = types;
            if (elementType != null) select = select.Where(t => t.Item1.ClrType == elementType);
            if (mimeContentTypes != null && mimeContentTypes.Length > 0)
                select = select.Where(t => t.Item1.MimeContentTypes.Intersect(mimeContentTypes).Any());
            return select.Select(t => t.Item2);
        }

        public class Constructor
        {
            public ConstructorParameter[] Parameters { get;  set; }
        }

        public class ConstructorParameter
        {
            public string ParameterName { get; set; }
            public Type ParameterType { get; set; }
        }

        public static Constructor[] GetConstructors(Type type)
        {
            return
                type.GetConstructors()
                    .Select(
                        c =>
                            new Constructor
                            {
                                Parameters =
                                    c.GetParameters()
                                        .Select(
                                            p =>
                                                new ConstructorParameter
                                                {
                                                    ParameterName = p.Name,
                                                    ParameterType = p.ParameterType
                                                })
                                        .ToArray()
                            })
                    .ToArray();
        }
    }
}
