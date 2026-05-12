// Add to a new file named Logger.cs in your UI project
using System;
using System.IO;
using System.Text;

namespace DocumentProcessor.UI
{
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static string _logFilePath;

        public static void Initialize(string logDirectory)
        {
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, $"DocProcessor_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            Log("Logging initialized");
        }

        public static void Log(string message)
        {
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}{Environment.NewLine}");
                }
                catch
                {
                    // Ignore errors in logging
                }
            }
        }
    }
}