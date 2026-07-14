using System;
using System.Text;
using UI.Interfaces;

namespace UI.Helpers
{
    internal static class InputHelpers
    {
        public static string ReadPassword(IConsole? console = null, char mask = '*', int maxLength = 512, bool allowCancel = true)
        {
            console ??= new ConsoleWrapper();

            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (allowCancel && key.Key == ConsoleKey.Escape)
                {
                    console.WriteLine();
                    return string.Empty;
                }

                if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Length--;
                    if (mask != '\0') console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar) && sb.Length < maxLength)
                {
                    sb.Append(key.KeyChar);
                    if (mask != '\0') console.Write(mask.ToString());
                }
            }
            console.WriteLine();
            return sb.ToString();
        }
    }
}
