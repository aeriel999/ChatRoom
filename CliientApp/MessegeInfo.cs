using System;

namespace CliientApp
{
    public class MessegeInfo
    {
        public string Messege { get; set; }
        public DateTime Time { get; set; }

        public MessegeInfo(string text)
        {
            Messege = text;
            Time = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Messege} : {Time.ToShortTimeString()}";
        }
    }
}
