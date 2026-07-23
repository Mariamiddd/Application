using System;
using System.Text;
using UI.Interfaces;

namespace UI.Helpers
{
    // helper class for reading password input from console with optional masking and cancellation
    internal static class InputHelpers
    {
        private const int DefaultMaxLength = 512;
        private const char DefaultMaskChar = '*';

        //   read password from console with optional masking and cancellation
       
        public static string ReadPassword(IConsole? console = null, char mask = DefaultMaskChar, 
                                         int maxLength = DefaultMaxLength, bool allowCancel = true)
        {
            console ??= new ConsoleWrapper();

            var password = new StringBuilder();

            while (true)
            {
                var key = console.ReadKey(intercept: true);

                // Handle enter - end input
                if (key.Key == ConsoleKey.Enter)
                {
                    console.WriteLine();
                    return password.ToString();
                }

                // Handle escape - cancel input
                if (allowCancel && key.Key == ConsoleKey.Escape)
                {
                    console.WriteLine();
                    return string.Empty;
                }

                // Handle backspace - remove last character
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Length--;
                        DisplayBackspace(console, mask);
                    }
                    continue;
                }

                // Handle regular characters - add to password
                if (IsValidPasswordChar(key, password.Length, maxLength))
                {
                    password.Append(key.KeyChar);
                    DisplayMask(console, mask);
                }
            }
        }

        // Validate if the key is a valid character for password input
        private static bool IsValidPasswordChar(ConsoleKeyInfo key, int currentLength, int maxLength)
        {
            // Must not be a control character
            if (char.IsControl(key.KeyChar))
                return false;

            // Must not exceed max length
            if (currentLength >= maxLength)
                return false;

            return true;
        }

        // Display the mask character for each input character
        private static void DisplayMask(IConsole console, char mask)
        {
            if (mask != '\0')
                console.Write(mask.ToString());
        }

        // Handle backspace display in console
        private static void DisplayBackspace(IConsole console, char mask)
        {
            if (mask != '\0')
                console.Write("\b \b");  // Backspace, space, backspace
        }
    }
}
