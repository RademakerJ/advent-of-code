using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day13
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);

            // Only 2 lines, first is earliest departure time, second bus-lines
            var earliestDepTime = int.Parse(streamReader.ReadLine());
            var busesRaw = streamReader.ReadLine().Split(',');
            var buses = busesRaw
                .Where(busRaw => !busRaw.Equals("x"))
                .Select(bus => int.Parse(bus))
                .ToArray();

            // Find nearest bus times
            var nearestBusTimes = new Dictionary<int, int>();
            foreach (var bus in buses)
            {
                nearestBusTimes[bus] = bus * (int)Math.Ceiling((double)earliestDepTime / bus);
            }

            var earliestBus = nearestBusTimes.Aggregate((left, right) => left.Value < right.Value ? left : right);
            var problemOne = (earliestBus.Value - earliestDepTime) * earliestBus.Key;
            Console.WriteLine($"Solution problem l: {problemOne}"); // Problem 1

            var rems = busesRaw.Where(x => !x.Equals("x")).Select(x => long.Parse(x) - Array.IndexOf(busesRaw, x)).ToArray();
            var result = SolveWithChineseRemTheorem(buses.Select(x => (long)x).ToArray(), rems);
            Console.WriteLine($"Solution problem 2: {result}"); // Problem 2

            PrintExplanations(buses.Select(x => (long)x).ToArray(), rems, result);
        }

        static long SolveWithChineseRemTheorem(long[] numbers, long[] rems)
        {
            long s = 1;
            long prod = numbers.Aggregate(s, (i, j) => i * j);
            long p;
            long sm = 0;
            for (long i = 0; i < (long)numbers.Length; i++)
            {
                p = prod / numbers[i];
                sm += rems[i] * ModularMultiplicativeInverse(p, numbers[i]) * p;
            }
            return sm % prod; 
        }

        private static long ModularMultiplicativeInverse(long a, long mod)
        {
            long b = a % mod;
            for (long x = 1; x < mod; x++)
            {
                if ((b * x) % mod == 1)
                {
                    return x;
                }
            }
            return 1;
        }

        private static void PrintExplanations(long[] buses, long[] rems, long result)
        {
            int counter = 0;
            int maxCount = buses.Length - 1;
            while (counter <= maxCount)
            {
                Console.WriteLine($"{result} ≡ {rems[counter]} (mod {buses[counter]})");
                counter++;
            }
        }
    }
}
