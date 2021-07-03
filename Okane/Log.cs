using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Okane
{
    public static class Log
    {
        public static TimeZoneInfo TimeZone = TimeZoneInfo.Local;

        private static Mutex ConsoleLock = new Mutex();
        private static Mutex FileLock = new Mutex();

        public enum Level {
            Off,
            Error,
            Info,
            Debug,
            Trace
        }

        public static bool ConsoleColors = true;
        public static Level ConsoleLevel { get; set; } = Level.Info;
        public static Level FileLevel { get; set; } = Level.Off;

        private static DateTime Now() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZone);
        private static string LogsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetCallingAssembly().GetName().Name, "logs");

        private static int MaxLogLevelStringLength = Enum.GetValues(typeof(Level)).Cast<Level>().Where(x => x != Level.Off).Max(x => x.ToString().Length);
        private static ConsoleColor[] ColorExceptions = { ConsoleColor.Black, ConsoleColor.White, ConsoleColor.Red };
        private static ConsoleColor[] Colors = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().Where(x => !ColorExceptions.Contains(x)).ToArray();

        public static void Error(string value, [CallerMemberName] string caller = "")
        {
            if (ConsoleLevel >= Level.Error)
            {
                ConsoleLock.WaitOne();
                if (ConsoleColors)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write($"{Level.Error.ToString().ToUpperInvariant().PadRight(MaxLogLevelStringLength)} [{caller}] {value}".PadRight(Console.WindowWidth - 1));
                if (ConsoleColors)
                {
                    Console.ResetColor();
                }
                Console.WriteLine();
                ConsoleLock.ReleaseMutex();
            }
            if (FileLevel >= Level.Error)
            {
                WriteToFile(Level.Error, value, caller);
            }
        }

        public static void Info(string value, [CallerMemberName] string caller = "")
        {
            if (ConsoleLevel >= Level.Info)
            {
                WriteToConsole(Level.Info, value, caller);
            }
            if (FileLevel >= Level.Info)
            {
                WriteToFile(Level.Info, value, caller);
            }
        }

        public static void Debug(string value, [CallerMemberName] string caller = "")
        {
            if (ConsoleLevel >= Level.Debug)
            {
                WriteToConsole(Level.Debug, value, caller);
            }
            if (FileLevel >= Level.Debug)
            {
                WriteToFile(Level.Debug, value, caller);
            }
        }

        public static void Trace(string value, [CallerMemberName] string caller = "")
        {
            if (ConsoleLevel >= Level.Trace)
            {
                WriteToConsole(Level.Trace, value, caller);
            }
            if (FileLevel >= Level.Trace)
            {
                WriteToFile(Level.Trace, value, caller);
            }
        }

        private static void WriteToConsole(Level level, string value, string caller)
        {
            ConsoleLock.WaitOne();
            Console.Write($"{level.ToString().ToUpperInvariant().PadRight(MaxLogLevelStringLength)} [");
            if (ConsoleColors)
            {
                Console.ForegroundColor = Colors[caller.Select(x => (int)x).Sum() % Colors.Length];
            }
            Console.Write(caller);
            if (ConsoleColors)
            {
                Console.ResetColor();
            }
            Console.WriteLine($"] {value}");
            ConsoleLock.ReleaseMutex();
        }

        private static void WriteToFile(Level level, string value, string caller)
        {
            FileLock.WaitOne();
            if (!Directory.Exists(LogsDirectoryPath))
            {
                Directory.CreateDirectory(LogsDirectoryPath);
            }
            DateTime now = Now();
            string filePath = Path.Combine(LogsDirectoryPath, now.ToString("dd.MM.yy") + ".txt");
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(FormatForFile(now, level, value, caller));
                }
            }
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(FormatForFile(now, level, value, caller));
            }
            FileLock.ReleaseMutex();
        }

        private static string FormatForFile(DateTime time, Level level, string value, string caller)
        {
            return $"{time.ToString("dd.MM.yy HH:mm:ss")}\t{level.ToString().ToUpperInvariant()}\t[{caller}] {value}";
        }
    }
}
