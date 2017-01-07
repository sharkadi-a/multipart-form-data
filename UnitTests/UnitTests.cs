using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using MultipartFormParser;
using MultipartFormParser.ContentDecoders;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void TestTextFormData()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(TestResources.SampleTextFormData));
            var parser = new MultipartForm();
            parser.Parse(stream);
            var formData = parser.Data;

            Assert.AreEqual(3, formData.Content.Length);
            var encoding = Encoding.GetEncoding(formData.Content[1].Charset);
            Console.WriteLine(encoding.GetString(formData.Content[1].Content));
        }

        [Test]
        public void TestBinaryFormData()
        {
            var parser = new MultipartForm();
            parser.Parse(new MemoryStream(TestResources.SampleBinaryFormData));
            var data = parser.Data;
            Assert.AreEqual(1, data.Content.Length);
            var image = data.Content.Single();
            Assert.AreEqual("image/png", image.ContentType);
            Assert.AreEqual("f", image.Name);
            Assert.AreEqual("sample-icon.png", image.Filename);
            Assert.NotNull(image.Content);

            using (var temp = new TemproaryDirectory())
            {
                var decoder = ContentDecoderFactory.FindAndCreateInstance<Image>(image);
                using (Image img = decoder.Decode())
                {
                    img.Save(temp.AppendPath("sample-icon.png"), ((ImageContentDecoder)decoder).ImageFormat);
                }
            }
        }
    }
}
