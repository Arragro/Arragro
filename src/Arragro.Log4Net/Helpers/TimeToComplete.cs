using System;

namespace Arragro.Log4Net.Helpers
{
    public class TimeToComplete
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }

        public override string ToString()
        {
            var hours = Hours > 0 ? Hours.ToString() + " hours " : "";
            var minutes = Minutes > 0 ? Minutes.ToString() + " minutes " : "";
            var seconds = Seconds > 0 ? Seconds.ToString() + " seconds " : "";
            var milliseconds = Milliseconds > 0 ? Milliseconds.ToString() + " milliseconds" : "";

            return
                string.Format("{0}{1}{2}{3}", hours, minutes, seconds, milliseconds);
        }

        public TimeToComplete(DateTime start)
        {
            var finish = (DateTime.Now - start);

            Hours = finish.Hours;
            Minutes = finish.Minutes;
            Seconds = finish.Seconds;
            Milliseconds = finish.Milliseconds;
        }
    }
}
