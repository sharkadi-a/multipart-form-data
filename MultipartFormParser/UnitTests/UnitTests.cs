using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultipartFormParser;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class UnitTests
    {
        private static string _multipart =
@"POST / HTTP/1.1
Host: localhost:8000
User-Agent: Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:29.0) Gecko/20100101 Firefox/29.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: en-US,en;q=0.5
Accept-Encoding: gzip, deflate
Cookie: __atuvc=34%7C7; permanent=0; _gitlab_session=226ad8a0be43681acf38c2fab9497240; __profilin=p%3Dt; request_method=GET
Connection: keep-alive
Content-Type: _multipart/form-data; boundary=---------------------------9051914041544843365972754266
Content-Length: 554

-----------------------------9051914041544843365972754266
Content-Disposition: form-data; name=""text""

text default
-----------------------------9051914041544843365972754266
Content-Disposition: form-data; name=""file1""; filename=""a.txt""
Content-Type: text/plain; charset=utf-16

Content of a.txt.

-----------------------------9051914041544843365972754266
Content-Disposition: form-data; name=""file2""; filename=""a.html""
Content-Type: text/html

<!DOCTYPE html><title>Content of a.html.</title>

-----------------------------9051914041544843365972754266--";

        [Test]
        public void Test()
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(_multipart));
            var parser = new MultipartForm(stream);
            var formData = parser.Parse();

            Assert.AreEqual(3, formData.Content.Length);
            var encoding = Encoding.GetEncoding(formData.Content[1].Charset);
            Console.WriteLine(encoding.GetString(formData.Content[1].Content));
        }
    }
}
