using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleAppPL
{
    public static class ConsoleHelper
    {
        public static void Print(object msg, ConsoleColor textColor)
        {
            var colorBefore = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            Console.Write(msg);
            Console.ForegroundColor = colorBefore;
        }
        public static void PrintLine(object msg, ConsoleColor textColor)
        {
            var colorBefore = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = colorBefore;
        }
        public static void Wait()
        {
            Console.Write($"Press Enter key to continue...");
            Console.ReadLine();
            Console.WriteLine();
        }
        public static void Wait(ConsoleColor textColor)
        {
            Print($"Press Enter key to continue...", textColor);
            Console.ReadLine();
            Console.WriteLine();
        }
        public static void Wait(string msg)
        {
            Console.Write($"{msg} Press Enter key to continue...");
            Console.ReadLine();
            Console.WriteLine();
        }
        public static void Wait(string msg, ConsoleColor textColor)
        {
            Print($"{msg} Press Enter key to continue...", textColor);
            Console.ReadLine();
            Console.WriteLine();
        }

        // ========================================================================================
        /* 
         * Get user input
         */

        public static uint GetUInt32(string msg = "Input an unsigned integer: ")
        {
            uint id;
            while (true)
            {
                Console.Write(msg);
                if (UInt32.TryParse(Console.ReadLine(), out id))
                {
                    break;
                }
                Console.WriteLine("Invalid input! Please input a non negative integer!");
            }
            return id;
        }
        public static uint GetLibraryCardID(string msg = "Input Library Card's ID: ")
        {
            uint id;
            while (true)
            {
                Console.Write(msg);
                if (UInt32.TryParse(Console.ReadLine(), out id) && id > 0)
                {
                    break;
                }
                Console.WriteLine("Invalid input! Please input a positive integer!");
            }
            return id;
        }
        public static char GetChoice(string msg, params char[] listOfChoices)
        {
            char choice;
            while (true)
            {
                Console.Write(msg);
                if (Char.TryParse(Console.ReadLine(), out choice) && Array.IndexOf<char>(listOfChoices, choice) >= 0)
                {
                    break;
                }
                PrintLine("Invalid input!", ConsoleColor.Red);
            }
            return choice;
        }
        public static string GetString(string msg)
        {
            Console.Write(msg);
            string getinput = Console.ReadLine();
            return getinput;
        }
        public static string GetStringOrNull(string msg)
        {
            Console.Write(msg);
            string getinput = Console.ReadLine().Trim();
            if (getinput.Length == 0)
                getinput = null;
            return getinput;
        }
        public static string GetStringTrim(string msg, bool allowWhitespace = false)
        {
            string getinput;
            do
            {
                Console.Write(msg);
                getinput = Console.ReadLine().Trim();
                if (allowWhitespace == false && String.IsNullOrWhiteSpace(getinput))
                {
                    PrintLine("String can not be empty and whitespace-only is not allowed!", ConsoleColor.Red);
                }
                else
                    break;
            } while (true);
            return getinput;
        }
        public static void PrintTable(List<List<string>> rowsContents, List<int> columnsFormat, string title = null, bool hasCenterBorder = true)
        {
            if (columnsFormat[1] >= 0)
                throw new FormatException("columnsFormat[1] must be a negative number");
            int rowLenght = columnsFormat.Sum<int>(x => Math.Abs(x)) + 3 * columnsFormat.Count + 1;
            string rowSeparator = $"+{new string('-', rowLenght - 2)}+";
            if (title != null)
            {
                Console.WriteLine(rowSeparator);
                Console.WriteLine($"|{title.CenterLine(rowLenght - 2)}|");
            }
            Console.WriteLine(rowSeparator);
            for (int i = 0; i < rowsContents.Count; i++)
            {
                List<string> row = rowsContents[i];
                if (row[1].Length > -columnsFormat[1])
                {
                    int chunks = (int)Math.Ceiling(-(double)row[1].Length / columnsFormat[1]);
                    row[1] = row[1].PadRight(-columnsFormat[1] * chunks);

                    for (int j = 0; j < chunks; j++)
                    {
                        if (j == 0)
                        {
                            Console.Write("|");
                            for (int k = 0; k < row.Count; k++)
                            {
                                if (k == 1)
                                    Console.Write(" {0} |", row[1].Substring(-j * columnsFormat[1], -columnsFormat[1]));
                                else
                                    Console.Write(" {0} |", columnsFormat[k] < 0 ? row[k].PadRight(-columnsFormat[k]) : row[k].PadLeft(columnsFormat[k]));
                            }
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.Write("|");
                            for (int k = 0; k < row.Count; k++)
                            {
                                if (k == 1)
                                    Console.Write(" {0} |", row[1].Substring(-j * columnsFormat[1], -columnsFormat[1]));
                                else
                                    Console.Write(" {0} |", "".PadLeft(Math.Abs(columnsFormat[k])));
                            }
                            Console.WriteLine();
                        }
                    }
                }
                else
                {
                    Console.Write("|");
                    for (int k = 0; k < row.Count; k++)
                    {
                        Console.Write(" {0} |", columnsFormat[k] < 0 ? row[k].PadRight(-columnsFormat[k]) : row[k].PadLeft(columnsFormat[k]));
                    }
                    Console.WriteLine();
                }
                if (hasCenterBorder && i < rowsContents.Count - 1) Console.WriteLine(rowSeparator);
            }
            Console.WriteLine(rowSeparator);
        }
        public static string CenterLine(this string oldText, int lineLength)
        {
            return oldText.PadLeft((lineLength - oldText.Length) / 2 + oldText.Length).PadRight(lineLength);
        }
    }
}