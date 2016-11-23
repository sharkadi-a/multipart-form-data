using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser.Exceptions
{
    public class EncodingNotFoundException : Exception
    {
        public EncodingNotFoundException()
        {
            
        }

        public EncodingNotFoundException(string message) : base(message)
        {
            
        }
    }
}
