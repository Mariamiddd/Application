using System;
using UI.Interfaces;

namespace UI.Helpers
{
    // ConsoleWrapper class implements Iconsole masking the password input and providing methods to read and write to the console.
    internal class ConsoleWrapper : IConsole
    {
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
        public string ReadLine() => Console.ReadLine() ?? string.Empty;
        public void Write(string value) => Console.Write(value);
        public void WriteLine(string? value = null) => Console.WriteLine(value);
        public void Clear() => Console.Clear();
    }
}
