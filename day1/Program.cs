using System;
using System.IO;
using System.Collections.Generic;

namespace day1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            // Fill expenses
            var expenses = new List<int>();
            using StreamReader streamReader = new StreamReader(filePath);

            while (!streamReader.EndOfStream)
            {
                if (int.TryParse(streamReader.ReadLine(), out int x))
                {
                    expenses.Add(x);
                }
            }

            // Find two numbers that sum to 2020
            int[,,] expensesSum = new int[expenses.Count, expenses.Count, expenses.Count];
            bool found = false;

            for (int i = 0; i < expensesSum.GetUpperBound(0); i++)
            {
                if (found) { break; }
                for (int j = 0; j < expensesSum.GetUpperBound(1); j++)
                {
                    if (found) { break; }
                    for (int k = 0; k < expensesSum.GetUpperBound(2); k++)
                    {
                        if ((expenses[i] + expenses[j] + expenses[k]) == 2020)
                        {
                            Console.WriteLine(expenses[i] * expenses[j] * expenses[k]);
                            found = true;
                            break;
                        }
                    }
                }
            }

            /*
            // "Lookup table" for fun
            (int index1, int index2, int index3) = FindIndicesForValue(expensesSum, 2020);

            // Quick check
            if (index1 >= 0)
            {
                Console.WriteLine($"Value 1: {expenses[index1]}, Value 2: {expenses[index2]}, Value 3: {expenses[index3]}");
                Console.WriteLine($"Multiplication: {expenses[index1] * expenses[index2] * expenses[index3]}");
            }

            // Find the fist occurrence of the value
            // Return -1, -1, -1 if nothing was found
            static (int, int, int) FindIndicesForValue(int[,,] array, int value)
            {
                for (int i = 0; i < array.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < array.GetUpperBound(1); j++)
                    {
                        for (int k = 0; k < array.GetUpperBound(2); k++)
                        {
                            if (array[i, j, k] == value)
                            {
                                return (i, j, k);
                            }
                        }
                    }
                }
                return (-1, -1, -1);
            }
            */
        }
    }
}
