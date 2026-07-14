using System;

namespace UI.Interfaces
{
    public interface IConsole
    {
        ConsoleKeyInfo ReadKey(bool intercept);
        string ReadLine();
        void Write(string value);
        void WriteLine(string? value = null);
        void Clear();
    }
}
