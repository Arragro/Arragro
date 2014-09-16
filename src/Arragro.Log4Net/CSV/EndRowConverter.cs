using log4net.Util;
using System.IO;

namespace Arragro.Log4Net.CSV
{
    public class EndRowConverter : PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            var ctw = writer as CsvTextWriter;
            // write the ending quote for the last field
            if (ctw != null)
                ctw.WriteQuote();
            writer.WriteLine();
        }
    }
}
