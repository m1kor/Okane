using Okane.Broker.Commands;
using Okane.Broker.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using TimeZoneConverter;
using WebSocket4Net;

namespace Okane.Broker
{
    public class Client
    {
        public enum Mode
        {
            Demo,
            Real
        }

        public static readonly TimeZoneInfo TimeZone = TZConvert.GetTimeZoneInfo("Central European Standard Time");
        public string StreamSessionId { get; private set; }
        public bool Connected { get { return mainSocket.State == WebSocketState.Open && streamingSocket.State == WebSocketState.Open; } }

        private Stopwatch sw = new Stopwatch();
        private WebSocket mainSocket, streamingSocket;
        private Mutex commandSendLock = new Mutex();
        private ManualResetEvent openSync = new ManualResetEvent(false);
        private ManualResetEvent receiveSync = new ManualResetEvent(false);
        private string responseMessage;
        private List<Tuple<Type, Action<object>>> listeners = new List<Tuple<Type, Action<object>>>();
        private Thread pingThread;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };

        public static DateTime Now() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZone);

        public Client()
        {
            pingThread = new Thread(new ThreadStart(Ping));
        }

        public bool Connect(Mode mode)
        {
            if (mainSocket == null || mainSocket.State == WebSocketState.None || mainSocket.State == WebSocketState.Closed)
            {
                Log.Info($"Connecting to {mode.ToString().ToUpperInvariant()} server");
                switch (mode)
                {
                    case Mode.Demo:
                        mainSocket = new WebSocket("wss://ws.xtb.com/demo");
                        streamingSocket = new WebSocket("wss://ws.xtb.com/demoStream");
                        break;
                    case Mode.Real:
                        mainSocket = new WebSocket("wss://ws.xtb.com/real");
                        streamingSocket = new WebSocket("wss://ws.xtb.com/realStream");
                        break;
                    default:
                        return false;
                }
                mainSocket.Opened += MainSocketOpened;
                mainSocket.Closed += MainSocketClosed;
                mainSocket.MessageReceived += MainSocketMessageReceived;
                streamingSocket.Opened += StreamingSocketOpened;
                streamingSocket.Closed += StreamingSocketClosed;
                streamingSocket.MessageReceived += StreamingSocketMessageReceived;
                openSync.Reset();
                mainSocket.Open();
                streamingSocket.Open();
                openSync.WaitOne(10000);
                if (Connected)
                {
                    pingThread.Start();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void Idle(int msTimeout = 200) => Thread.Sleep(msTimeout);

        private void MainSocketOpened(object sender, EventArgs e)
        {
            if (streamingSocket.State != WebSocketState.Connecting)
            {
                openSync.Set();
            }
            Log.Info("Connected");
        }

        private void MainSocketClosed(object sender, EventArgs e)
        {
            Log.Info("Disconnected");
        }

        private void MainSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            responseMessage = e.Message;
            receiveSync.Set();
            Log.Trace("Incoming message");
            Log.Trace(e.Message);
        }

        private void StreamingSocketOpened(object sender, EventArgs e)
        {
            if (mainSocket.State != WebSocketState.Connecting)
            {
                openSync.Set();
            }
            Log.Info("Connected");
        }

        private void StreamingSocketClosed(object sender, EventArgs e)
        {
            Log.Info("Disconnected");
        }

        private void StreamingSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Log.Trace("Incoming message");
            Log.Trace(e.Message);
            var doc = JsonDocument.Parse(e.Message);
            string id = doc.RootElement.GetProperty("command").GetString();
            string dataRaw = doc.RootElement.GetProperty("data").GetRawText();
            var streamingDataTypes = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttributes(false).Any(y => (y as Streaming.DataDescriptor)?.Descriptor == id)).ToList();
            if (streamingDataTypes.Count == 1)
            {
                object data = JsonSerializer.Deserialize(dataRaw, streamingDataTypes[0]);
                var appropriateListeners = listeners.Where(x => x.Item1 == streamingDataTypes[0]).ToList();
                if (appropriateListeners.Count > 0)
                {
                    appropriateListeners.ForEach(x => x.Item2(data));
                }
                else
                {
                    Log.Info($"No listener for \"{id}\"");
                }
            }
            else if (streamingDataTypes.Count == 0)
            {
                Log.Error("Unknown streaming command");
            }
            else
            {
                Log.Error("Ambiguous streaming command");
            }
        }

        private bool SendCommand<C>(C command, [CallerMemberName] string caller = "") where C : Command
        {
            if (Connected)
            {
                commandSendLock.WaitOne();
                Log.Debug(caller);
                string message = JsonSerializer.Serialize(command, jsonSerializerOptions);
                Log.Trace(message);
                receiveSync.Reset();
                mainSocket.Send(message);
                receiveSync.WaitOne();
                var response = (Response)JsonSerializer.Deserialize(responseMessage, typeof(Response));
                commandSendLock.ReleaseMutex();
                if (!response.status)
                {
                    Log.Error("Server error");
                    Log.Error("  Code : " + response.errorCode);
                    Log.Error("  Description : " + response.errorDescr);
                }
                return response.status;
            }
            else
            {
                Log.Error("Connection is closed");
                return false;
            }
        }

        private R SendCommand<C, R>(C command, [CallerMemberName] string caller = "")
            where C : Command
            where R : Response
        {
            if (Connected)
            {
                commandSendLock.WaitOne();
                Log.Debug(caller);
                string message = JsonSerializer.Serialize(command, jsonSerializerOptions);
                Log.Trace(message);
                receiveSync.Reset();
                mainSocket.Send(message);
                receiveSync.WaitOne();
                var response = (R)JsonSerializer.Deserialize(responseMessage, typeof(R));
                commandSendLock.ReleaseMutex();
                if (!response.status)
                {
                    Log.Error("Server error");
                    Log.Error("  Code : " + response.errorCode);
                    Log.Error("  Description : " + response.errorDescr);
                }
                return response;
            }
            else
            {
                Log.Error("Connection is closed");
                return null;
            }
        }

        private void SendStreamingCommand<C>(C command, [CallerMemberName] string caller = "") where C : StreamingCommand
        {
            if (Connected)
            {
                Log.Debug(caller);
                string message = JsonSerializer.Serialize(command, jsonSerializerOptions);
                Log.Trace(message);
                streamingSocket.Send(message);
            }
            else
            {
                Log.Error("Connection is closed");
            }
        }

        private void Ping()
        {
            sw.Restart();
            while (Connected)
            {
                if (sw.Elapsed.TotalSeconds >= 15.0)
                {
                    SendStreamingCommand(new PingStreaming(StreamSessionId));
                    SendCommand(new Ping());
                    sw.Restart();
                }
                Idle();
            }
            sw.Stop();
        }

        public void RegisterListener<T>(Action<T> listener)
        {
            listeners.Add(new Tuple<Type, Action<object>>(typeof(T), x => listener((T)x)));
        }

        // Requests

        public void Login(string userId, string password, string appName)
        {
            Log.Info("Logging in...");
            var response = SendCommand<LoginCommand, LoginResponse>(new LoginCommand(userId, password, appName));
            if (response.status)
            {
                StreamSessionId = response.streamSessionId;
                Log.Info("StreamSessionId = " + StreamSessionId);
            }
        }

        public void Logout()
        {
            Log.Info("Logging out...");
            SendCommand(new LogoutCommand());
        }

        public DateTime GetServerTime()
        {
            var response = SendCommand<GetServerTimeCommand, GetServerTimeResponse>(new GetServerTimeCommand());
            return response.status ? UnixEpoch.AddMilliseconds((double)response.returnData.timeMs) : DateTime.MinValue;
        }

        public List<Responses.Data.Symbol> GetAllSymbols()
        {
            var response = SendCommand<GetAllSymbolsCommand, GetAllSymbolsResponse>(new GetAllSymbolsCommand());
            return response.status ? response.returnData : null;
        }

        public Responses.Data.Symbol GetSymbol(string symbol)
        {
            var response = SendCommand<GetSymbolCommand, GetSymbolResponse>(new GetSymbolCommand(symbol));
            return response.status ? response.returnData : null;
        }

        public List<Responses.Data.TradingHours> GetTradingHours(List<string> symbols)
        {
            var response = SendCommand<GetTradingHoursCommand, GetTradingHoursResponse>(new GetTradingHoursCommand(symbols));
            return response.status ? response.returnData : null;
        }

        public decimal GetMarginTrade(string symbol, decimal volume)
        {
            var response = SendCommand<GetMarginTradeCommand, GetMarginTradeResponse>(new GetMarginTradeCommand(symbol, volume));
            return response.status ? response.returnData["margin"] : decimal.MinValue;
        }

        // Streaming commands

        public void SubscribeBalance()
        {
            SendStreamingCommand(new SubscribeBalance(StreamSessionId));
        }

        public void UnsubscribeBalance()
        {
            SendStreamingCommand(new UnsubscribeBalance());
        }

        public void SubscribeKeepAlive()
        {
            SendStreamingCommand(new SubscribeKeepAlive(StreamSessionId));
        }

        public void UnsubscribeKeepAlive()
        {
            SendStreamingCommand(new UnsubscribeKeepAlive());
        }

        public void SubscribeTickPrices(string symbol, int? minArrivalTime = null, int? maxLevel = null)
        {
            SendStreamingCommand(new SubscribeTickPrices(StreamSessionId, symbol, minArrivalTime, maxLevel));
        }

        public void UnsubscribeTickPrices(string symbol)
        {
            SendStreamingCommand(new UnsubscribeTickPrices(symbol));
        }

        public void SubscribeProfits()
        {
            SendStreamingCommand(new SubscribeProfits(StreamSessionId));
        }

        public void UnsubscribeProfits()
        {
            SendStreamingCommand(new UnsubscribeProfits());
        }
    }
}
