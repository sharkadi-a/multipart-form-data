using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    internal sealed class StreamLineReader
    {
        private readonly Stream _stream;

        public StreamLineReader(Stream stream)
        {
            if (!stream.CanRead) throw  new Exception("Stream is not readable");
            _stream = stream;
        }

        public void Read(Func<byte[], bool> lineReadFunc)
        {
            int b = 0;
            var buffer = new List<byte>(1000);
            while (true)
            {
                b = _stream.ReadByte();
                if (b != -1) buffer.Add((byte)b);
                if (b == '\n' || b == -1)
                {
                    var continueResult = lineReadFunc(buffer.ToArray());
                    buffer.Clear();
                    if (!continueResult || b == -1) break;
                }
            }            
        }
    }
}
