using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day3
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);

            // Create grid
            var grid = new List<List<char>>();
            while (!streamReader.EndOfStream)
            {
                var characters = (streamReader.ReadLine()).ToCharArray();
                grid.Add(characters.ToList());
            }

            // Solve puzzle
            Console.WriteLine($"Problem 1 amount trees: {CountTrees(grid, new int[] { 1 }, new int[] { 3 })}"); // problem 1
            Console.WriteLine($"Problem 2 amount trees: {CountTrees(grid, new int[] { 1, 1, 1, 1, 2 }, new int[] { 1, 3, 5, 7, 1 }) }"); // problem 2

            static int CountTrees(List<List<char>> grid, int[] rowSlope, int[] columnSlope)
            {
                int treesMultiplied = 1;

                for (int i = 0; i < rowSlope.Length; i++)
                {
                    int treesSum = 0;
                    int currentColumnIndex = 0;
                    int currentRowIndex = 0;

                    while (currentRowIndex + rowSlope[i] < grid.Count)
                    {
                        currentRowIndex += rowSlope[i];
                        currentColumnIndex = (currentColumnIndex + columnSlope[i]) % grid[0].Count;
                        treesSum += grid[currentRowIndex][currentColumnIndex].Equals('#') ? 1 : 0;
                    }
                    treesMultiplied *= treesSum;
                }
                return treesMultiplied;
            }
        }
    }
}
