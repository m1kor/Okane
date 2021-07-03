using Okane.Broker.Streaming;
using System;
using System.Collections.Generic;

namespace Okane.Broker
{
    public class Timeline
    {
        private LinkedList<Tick> frames = new LinkedList<Tick>();
        private int minutes;

        public Timeline(int minutes = 30)
        {
            if (minutes > 0)
            {
                this.minutes = minutes;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Update(Tick t)
        {
            var tick = new Tick(t);
            frames.AddLast(tick);
            while (frames.Count > 0 && (Client.Now() - frames.First.Value.time).TotalMinutes > minutes)
            {
                frames.RemoveFirst();
            }
        }

        public decimal GetAskDelta(float msWindow)
        {
            var now = Client.Now();
            var el = frames.Last;
            decimal ask = 0.0m;
            while (el != null)
            {
                ask = el.Value.ask;
                if ((now - el.Value.time).TotalMilliseconds >= msWindow)
                {
                    return frames.Last.Value.ask - ask;
                }
                el = el.Previous;
            }
            if (frames.Count < 2)
            {
                return 0.0m;
            }
            else
            {
                return frames.Last.Value.ask - ask;
            }
        }

        public decimal GetBidDelta(float msWindow)
        {
            var now = Client.Now();
            var el = frames.Last;
            decimal bid = 0.0m;
            while (el != null)
            {
                bid = el.Value.bid;
                if ((now - el.Value.time).TotalMilliseconds >= msWindow)
                {
                    return frames.Last.Value.bid - bid;
                }
                el = el.Previous;
            }
            if (frames.Count < 2)
            {
                return 0.0m;
            }
            else
            {
                return frames.Last.Value.bid - bid;
            }
        }

        public Tick Last()
        {
            if (frames.Count == 0)
            {
                return null;
            }
            else
            {
                return frames.Last.Value;
            }
        }
    }
}
