using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZQNB.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static string Resource(this Assembly self, string name, Encoding encoding = null)
        {
            Assertion.NotNull(self);
            Assertion.NotEmpty(name);

            var stream = self.GetManifestResourceStream(name);
            if (stream == null)
            {
                return null;
            }

            if (!stream.CanRead)
            {
                return string.Empty;
            }

            //结果
            var result = string.Empty;
            using (var streamReader = encoding != null ? new StreamReader(stream, encoding) : new StreamReader(stream, Encoding.UTF8))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
