using System;

namespace TwitterLib.Model
{
   

    public class Envelope
    {
        public Envelope(string payload)
        {
            Payload = payload;
        }

        public string Payload { get; set; }
    }
}
