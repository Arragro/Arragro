using log4net.Util;
using System.IO;

namespace Arragro.Log4Net.CSV
{
    public class NewFieldConverter : PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            var ctw = writer as CsvTextWriter;
            // write the ending quote for the previous field
            if (ctw != null)
                ctw.WriteQuote();
            writer.Write(',');
            // write the starting quote for the next field
            if (ctw != null)
                ctw.WriteQuote();
        }
    }
}
