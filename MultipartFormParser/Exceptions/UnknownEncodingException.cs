using System;

namespace MultipartFormParser.Exceptions
{
    /// <summary>
    /// This exception raises when <see cref="EncodingResolver"/> cannot resolve charset into <see cref="System.Text.Encoding"/> 
    /// </summary>
    public class UnknownEncodingException : Exception
    {
        internal UnknownEncodingException(string charsetName) : base("Encoding not found for charset " + charsetName)
        {
            
        }
    }
}
