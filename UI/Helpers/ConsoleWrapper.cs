using System;
using UI.Interfaces;

namespace UI.Helpers
{
    internal class ConsoleWrapper : IConsole
    {
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
        public string ReadLine() => Console.ReadLine() ?? string.Empty;
        public void Write(string value) => Console.Write(value);
        public void WriteLine(string? value = null) => Console.WriteLine(value);
        public void Clear() => Console.Clear();
    }
}
