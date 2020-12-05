using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day5
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);

            int highestSeatId = 0;
            var seatIds = new List<int>();

            while (!streamReader.EndOfStream)
            {
                (int min, int max) rows = (0, 127);
                (int min, int max) columns = (0, 7);
                var seat = streamReader.ReadLine();

                for (int i = 0; i < seat.Length; i++)
                {
                    if (seat[i].Equals('F') || seat[i].Equals('B'))
                    {
                        rows = GetSplit(seat[i], rows.min, rows.max);
                    }
                    else if (seat[i].Equals('L') || seat[i].Equals('R'))
                    {
                        columns = GetSplit(seat[i], columns.min, columns.max);
                    }
                }

                var seatId = (rows.min * 8) + columns.min;
                seatIds.Add(seatId);
                highestSeatId = seatId > highestSeatId ? seatId : highestSeatId;
            }

            seatIds = seatIds.OrderBy(x => x).ToList();
            var mySeatId = Enumerable.Range(seatIds[0], seatIds[seatIds.Count - 1]).Except(seatIds).First();

            Console.WriteLine("Highest boarding pass ID: " + highestSeatId);
            Console.WriteLine("My seat ID: " + mySeatId);

            static (int, int) GetSplit(char input, int min, int max)
            {
                if (input.Equals('F') || input.Equals('L'))
                {
                    return (min, (min + max) / 2);
                }

                if (input.Equals('B') || input.Equals('R'))
                {
                    return ((min + max) / 2 + 1, max);
                }

                return (min, max);
            }
        }
    }
}
