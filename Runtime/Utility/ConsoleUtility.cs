using System;
using System.Collections.Generic;
using Tactile.Console.Commands;
using UnityEngine;

namespace Tactile.Console.Utility
{
    public static class ConsoleUtility
    {
        public static Color FromHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color))
                return color;

            throw new ArgumentException("Not a valid hex string!");
        }

        public static void PrintLine(this Console console) => console.Print(string.Empty);

        public static string[] GetCommandPrefix(this BaseCommand command)
        {
            var prefixNames = new List<string>();
            command = command.ParentCommand;
            
            while (command != null)
            {
                prefixNames.Add(command.Name);
                command = command.ParentCommand;
            }

            prefixNames.Reverse();
            return prefixNames.ToArray();
        }
    }
}