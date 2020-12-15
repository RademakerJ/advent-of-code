using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day14
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            Console.WriteLine($"Problem 1 solution: {GetSolution(filePath, true)}"); // problem 1
            Console.WriteLine($"Problem 2 solution: {GetSolution(filePath, false)}"); // problem 2
        }

        static long GetSolution(string filePath, bool isFirstProblem)
        {
            var dict = new Dictionary<string, string>();
            var currentMask = new List<char>();

            using StreamReader streamReader = new StreamReader(filePath);
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine().Split('=');
                if (line[0].StartsWith("mask"))
                {
                    currentMask = line[1].Trim().ToCharArray().ToList();
                    continue;
                }

                var key = line[0].Substring(line[0].IndexOf("[") + 1, line[0].IndexOf("]") - line[0].IndexOf("[") - 1);
                var values = Convert.ToString(int.Parse(line[1]), 2).PadLeft(36, '0').ToCharArray().Select(x => x.ToString()).ToArray();

                if (isFirstProblem)
                {
                    values = GetMaskResultSingle(currentMask, values);
                    dict[key] = new string(values.SelectMany(x => x).ToArray());
                }
                else
                {
                    var keyArray = Convert.ToString(int.Parse(key), 2).PadLeft(36, '0').ToCharArray().Select(x => x.ToString()).ToArray();
                    var binaryKeysMultiple = GetMaskResultWithPermutations(currentMask, keyArray);
                    foreach (var binaryKeyMultiple in binaryKeysMultiple)
                    {
                        key = Convert.ToInt64(new string(binaryKeyMultiple.SelectMany(x => x).ToArray()), 2).ToString();
                        dict[key] = new string(values.SelectMany(x => x).ToArray());
                    }
                }
            }

            long sum = 0;
            foreach (var item in dict)
            {
                sum += Convert.ToInt64(item.Value, 2);
            }

            return sum;
        }

        static string[] GetMaskResultSingle(List<char> currentMask, string[] values)
        {
            for (int i = 0; i < currentMask.Count; i++)
            {
                if (!currentMask[i].Equals('X'))
                {
                    values[i] = currentMask[i].ToString();
                    continue;
                }
            }
            return values;
        }

        static List<string[]> GetMaskResultWithPermutations(List<char> currentMask, string[] values)
        {
            for (int i = 0; i < currentMask.Count; i++)
            {
                if (!currentMask[i].Equals('0'))
                {
                    values[i] = currentMask[i].ToString();
                }
            }
            List<string[]> mutations = FillFloatingBits(values);
            return mutations;
        }

        static List<string[]> FillFloatingBits(string[] values)
        {
            if (!values.Contains("X"))
            {
                return new List<string[]> { values };
            }
            else
            {
                int index = Array.IndexOf(values, "X");
                string[] add0 = (string[])values.Clone();
                string[] add1 = (string[])values.Clone();
                add0[index] = "0";
                add1[index] = "1";
                return FillFloatingBits(add0).Concat(FillFloatingBits(add1)).ToList();
            }
        }
    }
}


