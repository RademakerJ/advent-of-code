using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day11
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var grid = new List<List<char>>();
            while (!streamReader.EndOfStream)
            {
                var row = streamReader.ReadLine().ToCharArray();
                grid.Add(row.ToList());
            }

            bool done = false;
            while (!done)
            {
                //(grid, done) = GetSeatMadness(grid, true, 4);
                (grid, done) = GetSeatMadness(grid, false, 5);
            }

            Console.WriteLine($"Occupied seats: {grid.SelectMany(x => x).Count(x => x.Equals('#'))}");
        }

        static (List<List<char>>, bool) GetSeatMadness(List<List<char>> grid, bool doOnce, int minAmountOccupied)
        {
            var newGrid = grid.Select(x => new List<char>(x)).ToList();

            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[0].Count; j++)
                {
                    if (grid[i][j].Equals('.'))
                    {
                        newGrid[i][j] = '.';
                        continue;
                    }

                    if (grid[i][j].Equals('L'))
                    {
                        if (!GetAdjacent(grid, new int[] { i, j }, doOnce).Contains('#'))
                        {
                            newGrid[i][j] = '#';
                        }
                    }
                    else if (grid[i][j].Equals('#'))
                    {
                        if (GetAdjacent(grid, new int[] { i, j }, doOnce).Count(item => item.Equals('#')) >= minAmountOccupied)
                        {
                            newGrid[i][j] = 'L';
                        }
                    }
                }
            }

            var done = Enumerable.SequenceEqual(grid.SelectMany(x => x), newGrid.SelectMany(y => y));
            return (newGrid, done);
        }

        static List<char> GetAdjacent(List<List<char>> grid, int[] currentIndex, bool doOnce)
        {
            var i = currentIndex[0];
            var j = currentIndex[1];

            var places = new List<char>()
            {
                GetSeat(grid, new int[,]{{i, -1},{j,  0}}, doOnce),    // i--
                GetSeat(grid, new int[,]{{i,  1},{j,  0}}, doOnce),    // i++
                GetSeat(grid, new int[,]{{i,  0},{j, -1}}, doOnce),    // j--
                GetSeat(grid, new int[,]{{i,  0},{j,  1}}, doOnce),    // j++
                GetSeat(grid, new int[,]{{i, -1},{j, -1}}, doOnce),    // i--, j--
                GetSeat(grid, new int[,]{{i,  1},{j, -1}}, doOnce),    // i++, j--
                GetSeat(grid, new int[,]{{i, -1},{j,  1}}, doOnce),    // i--, j++
                GetSeat(grid, new int[,]{{i,  1},{j,  1}}, doOnce)     // i++, j++
            };

            return places;
        }

        static char GetSeat(List<List<char>> grid, int[,] currentIndex, bool doOnce)
        {
            int indexI = currentIndex[0, 0];            // Current index i
            int indexIStep = currentIndex[0, 1];        // +1, -1, 0

            int indexJ = currentIndex[1, 0];            // Current index j
            int indexJStep = currentIndex[1, 1];        // +1, -1, 0

            var places = new List<char>();
            while (indexI + indexIStep < grid.Count && indexI + indexIStep >= 0 &&
                indexJ + indexJStep < grid[0].Count && indexJ + indexJStep >= 0)
            {
                indexI += indexIStep;
                indexJ += indexJStep;

                if (grid[indexI][indexJ].Equals('#') || grid[indexI][indexJ].Equals('L'))
                {
                    places.Add(grid[indexI][indexJ]);
                    break;
                }

                if (doOnce)
                {
                    break;
                }
            }

            return places.Contains('#') ? '#' : '.';
        }
    }
}
