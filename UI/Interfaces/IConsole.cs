using System;

namespace UI.Interfaces
{
    // This interface defines the contract for console operations, allowing for reading input and writing output to the console.
    public interface IConsole
    {
        ConsoleKeyInfo ReadKey(bool intercept);
        string ReadLine();
        void Write(string value);
        void WriteLine(string? value = null);
        void Clear();
    }
}