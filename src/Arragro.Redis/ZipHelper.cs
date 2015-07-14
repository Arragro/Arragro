using ICSharpCode.SharpZipLib.GZip;
using System.IO;
using System.Text;

namespace Arragro.Redis
{
    public static class ZipHelper
    {
        public static byte[] ZipString(string str)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(str);
            using (var ms = new MemoryStream())
            {
                GZipOutputStream gzipOut = new GZipOutputStream(ms);
                gzipOut.IsStreamOwner = false;

                gzipOut.Write(textBytes, 0, textBytes.Length);
                gzipOut.Close();
                return ms.ToArray();
            }
        }
        
        public static string UnZipString(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                ms.Seek(0, SeekOrigin.Begin);
                GZipInputStream zin = new GZipInputStream(ms);
                int size = 2048;
                byte[] writeData = new byte[2048];
                var stringBuilder = new StringBuilder();
                while (true)
                {
                    size = zin.Read(writeData, 0, size);
                    if (size > 0)
                    {
                        stringBuilder.Append(Encoding.UTF8.GetString(writeData, 0, size));
                    }
                    else
                    {
                        break;
                    }
                }
                
                return stringBuilder.ToString();
            }
        }
    }
}
