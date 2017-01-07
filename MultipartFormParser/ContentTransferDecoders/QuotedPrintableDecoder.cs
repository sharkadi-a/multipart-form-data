using System;
using System.Collections.Generic;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// This class decodes data from quoted-printable content transfer encoding into array of characters. Such decoder should be used by transfer environments, which treat data as a set of characters, not bytes
    /// https://en.wikipedia.org/wiki/Quoted-printable
    /// </summary>
    [ContentTransferDecoderType("quoted-printable", typeof(char))]
    internal class QuotedPrintableDecoder : IContentTransferDecoder<char>
    {
        public string ContentTransferEncodingName { get { return "quoted-printable"; } }
        public IEnumerable<char> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            if (multipartFormDataItem.Content == null) yield break;

            char[] escapeArray = new char[3];
            int escapeIdx = 0;
            foreach (var ch in new Char7BitDecoder().Decode(multipartFormDataItem))
            {
                if (ch != '=') yield return ch;
                escapeArray[escapeIdx++] = ch;
                if (escapeIdx > 1)
                {
                    escapeIdx = 0;
                    if (new string(escapeArray) == "=\r\n") continue;
                    if (escapeArray[0] == escapeArray[1])
                    {
                        yield return escapeArray[0];
                        yield return escapeArray[1];
                        yield return escapeArray[2];
                        continue;
                    }
                    string hex = new string(new[] {escapeArray[1], escapeArray[2]});
                    yield return (char) Convert.ToInt32(hex, 16);
                }
            }
            for (int i = 0; i <= escapeIdx; i++)
            {
                yield return escapeArray[i];
            }
        }
    }
}
