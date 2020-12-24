using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace day18
{
    class Program
    {
        static Dictionary<string, long> solutions = new Dictionary<string, long>();

        static Dictionary<char, Func<long, long, long>> operators = new Dictionary<char, Func<long, long, long>>()
        {
            {'+', (x, y) => x + y},
            {'*', (x, y) => x * y},
        };

        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            var results = new List<long>();
            using StreamReader streamReader = new StreamReader(filePath);
            while (!streamReader.EndOfStream)
            {
                var equation = streamReader.ReadLine().Replace(" ", "");
                equation = equation.Insert(0, "(");
                equation += ")";

                var equations = FindAllEquations(equation).OrderBy(x => x.Order).ToList();
                results.Add(SolveEquation(equations, true));
            }

            Console.WriteLine($"Solution: {results.Sum()}");
        }

        static long SolveEquation(List<Equation> equations, bool usePrecedence)
        {
            foreach (var equation in equations)
            {
                var equationStr = equation.OriginalEquation;
                if (!equationStr.Contains("("))
                {
                    solutions[equation.OriginalEquation] = GetResult(equationStr, usePrecedence);
                }
                else
                {
                    var innerEquations = equations
                        .Where(x => x.MinIndex > equation.MinIndex && x.MaxIndex < equation.MaxIndex)
                        .OrderByDescending(x => x.MaxIndex);

                    foreach (var innerEquation in innerEquations)
                    {
                        equationStr = equationStr.Replace($"({innerEquation.OriginalEquation})",
                            solutions[innerEquation.OriginalEquation].ToString());
                    }

                    solutions[equation.OriginalEquation] = GetResult(equationStr, usePrecedence);
                }
            }
            return solutions[equations[equations.Count - 1].OriginalEquation];
        }

        static long GetResult(string equationStr, bool usePrecedence)
        {
            long result = -1;
            for (int i = 0; i < equationStr.Length; i++)
            {
                if (operators.ContainsKey(equationStr[i]))
                {
                    if (!usePrecedence)
                    {
                        (long left, long right) = GetLeftRightNumbers(equationStr, i, result);
                        result = operators[equationStr[i]].Invoke(left, right);
                    }
                }
            }

            // First solve + then solve *
            if (usePrecedence)
            {
                var eqPrecedence = equationStr;
                var replaces = CalculatePrecedences(equationStr);
                foreach (var replace in replaces)
                {
                    eqPrecedence = eqPrecedence.Replace(replace.Item1, replace.Item2.ToString());
                }

                if (eqPrecedence.Contains('+') || eqPrecedence.Contains('*'))
                {
                    for (int i = 0; i < eqPrecedence.Length; i++)
                    {
                        if (operators.ContainsKey(eqPrecedence[i]))
                        {
                            (long left, long right) = GetLeftRightNumbers(eqPrecedence, i, result);
                            result = operators[eqPrecedence[i]].Invoke(left, right);
                        }
                    }
                }
                else
                {
                    result = long.Parse(eqPrecedence);
                }
            }

            return result;
        }

        static List<(string, long)> CalculatePrecedences(string equationStr)
        {
            var matches = equationStr.Split("*").Where(x => x.Contains("+")).ToArray();
            var results = new List<(string, long)>();

            foreach (var match in matches)
            {
                long result = -1;
                for (int j = 0; j < match.Length; j++)
                {
                    if (operators.ContainsKey(match[j]))
                    {
                        (long left, long right) = GetLeftRightNumbers(match, j, result);
                        result = operators[match[j]].Invoke(left, right);
                    }
                }
                results.Add((match, result));
            }
            return results.OrderByDescending(x => x.Item1.Length).ToList();
        }

        static (long, long) GetLeftRightNumbers(string equationStr, int i, long curResult)
        {
            var leftIndex = FindPreviousOperatorIndex(equationStr, i - 1);
            var rightIndex = FindNextOperatorIndex(equationStr, i + 1);

            var leftLength = leftIndex > 0 ? i - leftIndex : i;
            var rightLength = rightIndex > 0 ? rightIndex - (i + 1) : equationStr.Length - (i + 1);

            var left = curResult == -1 ? long.Parse(equationStr.Substring(leftIndex, leftLength).ToString()) : curResult;
            var right = long.Parse(equationStr.Substring(i + 1, rightLength).ToString());

            return (left, right);
        }

        static int FindNextOperatorIndex(string equation, int startIndex)
        {
            int index = -1;
            for (int i = startIndex; i < equation.Length; i++)
            {
                if (operators.ContainsKey(equation[i]))
                {
                    index = i;
                    break;
                }
            }
            return index == -1 ? 0 : index;
        }

        static int FindPreviousOperatorIndex(string equation, int startIndex)
        {
            int index = -1;
            for (int i = startIndex; i >= 0; i--)
            {
                if (operators.ContainsKey(equation[i]))
                {
                    index = i;
                    break;
                }
            }
            return index == -1 ? 0 : index;
        }

        static Equation[] FindAllEquations(string equation)
        {
            var openingParens = equation
                .Select((x, i) => new { character = x, index = i })
                .Where(x => x.character.Equals('('))
                .Select(x => x.index)
                .ToList();

            var closingParens = equation
                .Select((x, i) => new { character = x, index = i })
                .Where(x => x.character.Equals(')'))
                .Select(x => x.index)
                .ToList();

            var order = 0;
            var combinations = new List<Equation>();
            var stack = new Stack<int>();
            for (int i = 0; i < equation.Length; i++)
            {
                switch (equation[i])
                {
                    case '(':
                        stack.Push(i);
                        break;
                    case ')':
                        {
                            int index = stack.Count - 1;
                            var equationSub = equation.Substring(openingParens[index] + 1, i - openingParens[index] - 1);
                            combinations.Add(new Equation(equationSub, openingParens[index], i, order));

                            // Shift 
                            closingParens.RemoveAt(0);
                            openingParens.RemoveAt(index);
                            order++;
                            stack.TryPop(out int _);
                            break;
                        }
                    default:
                        break;
                }
            }
            return combinations.ToArray();
        }

        struct Equation
        {
            public string OriginalEquation { get; set; }
            public int MinIndex { get; }
            public int MaxIndex { get; }
            public int Order { get; }

            public Equation(string equation, int minIndex, int maxIndex, int order)
            {
                OriginalEquation = equation;
                MinIndex = minIndex;
                MaxIndex = maxIndex;
                Order = order;
            }
        }
    }
}
