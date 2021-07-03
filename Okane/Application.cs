using Okane.Broker;
using Okane.Broker.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Okane
{
    public class Application
    {
        enum State
        {
            Initializing,
            Waiting,
            WillBuy,
            Bought,
            WillSell
        }

        State state = State.Waiting;
        Timeline timeline = new Timeline();
        decimal inTotal = 0.0m;
        decimal openPrice = 0m;
        decimal decision, push;

        decimal margin = decimal.MinValue;

        object timelineLock = new object();
        object marginLock = new object();

        void TickReceived(Tick t)
        {
            lock (timelineLock)
            {
                timeline.Update(t);
            }
            /*decimal askDelta = timeline.GetAskDelta(5000);
            //Log.Info(askDelta.ToString("F5"));
            switch (state)
            {
                case State.Waiting:
                    if (askDelta <= -(t.spreadRaw * 0.4m))
                    {
                        state = State.WillBuy;
                        decision = t.ask;
                        push = t.ask;
                        Log.Info("Will buy");
                    }
                    break;
                case State.WillBuy:
                    if (t.ask != decision && decision != push && decision - push > t.spreadRaw * 0.05m && (t.ask - push) / (decision - push) > 0.2m)
                    {
                        state = State.Bought;
                        openPrice = t.ask;
                        Log.Info($"Bought at {t.ask.ToString("F5")}");
                    }
                    if (t.ask < push)
                    {
                        push = t.ask;
                    }
                    break;
                case State.Bought:
                    if (t.bid > openPrice)
                    {
                        state = State.WillSell;
                        decision = t.bid;
                        Log.Info("Will sell");
                    }
                    break;
                case State.WillSell:
                    // t.bid > openPrice * 1.0001m &&
                    if ((t.bid - openPrice) / (push - openPrice) <= 0.75m)
                    {
                        state = State.Waiting;
                        inTotal += t.bid - openPrice;
                        Log.Info($"Sell at {t.bid}");
                        Log.Info($"{(inTotal * 333.0m).ToString("F5")} in total");
                    }
                    if (t.bid > push)
                    {
                        push = t.bid;
                    }
                    break;
            }*/
        }

        decimal maxProfit = 0.0m;

        void ProfitReceived(Profit p)
        {
            Log.Info($"Bid delta = {timeline.GetBidDelta(60000).ToString("F5")}, spread = {(timeline.Last()?.spreadRaw * margin)?.ToString("F5") ?? "<no data>"}");
            lock (timelineLock)
            {
                if (p.profit > maxProfit)
                {
                    maxProfit = p.profit;
                    Log.Info($"New maximum at {maxProfit.ToString("F5")}");
                }
                else if (p.profit > 0.0m && timeline.Last() != null && (p.profit > timeline.Last().spreadRaw * margin && (p.profit < maxProfit * 0.7m || -timeline.GetBidDelta(60000) > timeline.Last().spreadRaw * 1.5m)))
                {
                    Log.Info($"Sell at {timeline.Last().bid.ToString("F5")} generates {p.profit.ToString("F2")} in result");
                    Environment.Exit(0);
                }
            }
        }

        public void Main(string[] args)
        {
            Log.FileLevel = Log.Level.Info;
            Log.ConsoleLevel = Log.Level.Info;
            while (true)
            {
                Log.Info("Start");
                Log.Info($"Time zone is {Client.TimeZone.StandardName}");
                Log.Info($"It is {Client.Now()} now");
                Client client = new Client();
                client.Connect(Client.Mode.Demo);
                client.Login("userId", "userPass*", "Okane");
                Client.Idle();
                if (client.Connected)
                {
                    /*var symbols = client.GetAllSymbols();
                    List<string> lines = new List<string>();
                    lines.Add("categoryName,groupName,symbol,currency,ask,bid,low,high,delta,deltaPercents");
                    symbols.ForEach(x => lines.Add($"{x.categoryName},{x.groupName},{x.symbol},{x.currency},{x.ask},{x.bid},{x.low},{x.high},{x.high-x.low},{(x.high - x.low) / x.ask}"));
                    client.Logout();
                    File.WriteAllLines("symbols.csv", lines.ToArray());*/
                    //client.SubscribeBalance();
                    /*var hours = client.GetTradingHours(new List<string>() { "GME.US_4" });
                    if (!hours[0].IsOpen())
                    {
                        Log.Info("Market is closed");
                    }*/
                    margin = client.GetMarginTrade("GBPJPY", 0.1m);
                    Client.Idle();
                    Log.Info($"Server time is {client.GetServerTime()}");
                    Client.Idle();
                    client.RegisterListener<Tick>(x => TickReceived(x));
                    client.RegisterListener<Profit>(x => ProfitReceived(x));
                    client.SubscribeTickPrices("GBPJPY");
                    client.SubscribeProfits();
                }
                while (client.Connected)
                {
                    margin = client.GetMarginTrade("GBPJPY", 0.1m);
                    Client.Idle(1000);
                }
                client.Logout();
                Log.Info("Stop");
            }
        }
    }
}
