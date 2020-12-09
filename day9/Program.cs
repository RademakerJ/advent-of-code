using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day9
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var numbers = new List<long>();
            while (!streamReader.EndOfStream)
            {
                numbers.Add(long.Parse(streamReader.ReadLine()));
            }

            (long defectNumber, int range) = FindDefect(numbers);                 // Problem 1
            Console.WriteLine($"Defect number: {defectNumber}");

            (long min, long max) = GetMinMax(numbers.Take(range).ToList(), defectNumber);
            Console.WriteLine($"Sum lowest and highest: {min + max}");            // Problem 2
        }

        static (long min, long max) GetMinMax(List<long> numbers, long targetNumber)
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                var sum = numbers[i];
                for (int j = (i + 1); j < numbers.Count; j++)
                {
                    sum += numbers[j];
                    if (sum == targetNumber)
                    {
                        var range = numbers.GetRange(i, j - i);
                        return (range.Min(), range.Max());
                    }
                }
            }
            return (0, 0);
        }

        static (long defectNumber, int range) FindDefect(List<long> numbers)
        {
            var startIndex = 0;
            // Start from 25, because the initial preamble is index 0-24
            for (int i = 25; i < numbers.Count; i++)
            {
                var preamble = numbers.GetRange(startIndex, 25);

                if (!TargetIsValid(preamble, numbers[i]))
                {
                    return (numbers[i], i);
                }

                startIndex++;
            }

            return (0, 0);
        }

        static bool TargetIsValid(List<long> preamble, long target)
        {
            var indexedInput = preamble.Select((n, i) => new { Number = n, Index = i });

            var result = preamble.Select((n, i) => new { Number = n, Index = i })
                    .SelectMany(
                        x => indexedInput,
                        (x, y) => new { x = x, y = y })
                    .Where(item => item.x.Index != item.y.Index)
                    .Select(item => item.x.Number + item.y.Number == target);

            return result.Any(item => item);
        }
    }
}
