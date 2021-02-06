namespace Talegen.Common.Core.Extensions
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// This class is used to override String Writer class.
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        /// <summary>
        /// This method is used to override the encoding to UTF8.
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;
    }
}