using System;
using System.IO;
using System.Linq;

namespace day2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);

            int minMaxCount = 0;
            int indicesCount = 0;
            while (!streamReader.EndOfStream)
            {
                var splitElements = streamReader.ReadLine().Split(" ");
                (int left, int right) = GetLeftRight(splitElements[0]);
                var character = char.Parse(splitElements[1].Replace(":", string.Empty));
                var password = splitElements[2];

                // Method 1: minmax, Method 2: indices (only 1 must contain the character)
                minMaxCount += IsValidMinMaxPassword(password, character, left, right) ? 1 : 0;
                indicesCount += IsValidIndicesPassword(password, character, left, right) ? 1 : 0;

            }

            // Log the outcomes
            Console.WriteLine($"Min-max password count:\n{minMaxCount}");
            Console.WriteLine($"Indices password count(only 1 must contain the character):\n{indicesCount}");

            // Check the indices. The string must only contain the character at one of the indices.
            static bool IsValidIndicesPassword(string password, char character, int firstIndex, int secondIndex)
            {
                return StringContainsElementAt(password, character, firstIndex) ^ StringContainsElementAt(password, character, secondIndex);
            }

            // Check min-max. The amount of characters (that equal character) must be between the min-max.
            static bool IsValidMinMaxPassword(string password, char character, int min, int max)
            {
                var characterCount = password.Count(c => c == character);
                return characterCount >= min && characterCount <= max;
            }

            // Format string = 5-7. Remove dash and parse element at first and second index.
            static (int, int) GetLeftRight(string leftRight)
            {
                var leftRightInts = leftRight.Split("-");

                return (int.Parse(leftRightInts[0]), int.Parse(leftRightInts[1]));
            }

            // Index is not zero based (starts at 1). This function subtracts 1 from index.
            static bool StringContainsElementAt(string input, char character, int index)
            {
                if (index - 1 < 0)
                {
                    return false;
                }

                return input.ElementAt(index - 1).Equals(character);
            }

        }
    }
}
