using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AnnaWebDiningFin.Infrastructure.Calculations
{
    class LogWriter
    {
        public static void Log(string log)
        {
            Console.WriteLine($"{GetThreadId()}: {log}");
        }

        private static string GetThreadId()
        {
            var idx = Thread.CurrentThread.ManagedThreadId;
            return $"{DateTime.Now:HH:mm:ss:ffff} (Thread {idx})";
        }
    }
}