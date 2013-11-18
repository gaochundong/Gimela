using System.Globalization;
using System.IO;
using System.Text;

namespace Gimela.Data.Sparrow
{
  internal class StringWriterWithEncoding : StringWriter
  {
    private readonly Encoding _encoding;

    public StringWriterWithEncoding(Encoding encoding)
      : base(CultureInfo.InvariantCulture)
    {
      _encoding = encoding;
    }

    public override Encoding Encoding
    {
      get { return _encoding; }
    }
  }
}
