using System;
using System.Linq;
using System.IO;

namespace day6
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);

            var groupAnswers = streamReader.ReadToEnd().Split(new string[] { Environment.NewLine + Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries);

            var anyoneCount = groupAnswers.Select(groupAnswer => groupAnswer.Replace(Environment.NewLine, string.Empty).ToCharArray().Distinct().Count()).Sum();
            Console.WriteLine("Sum of yes (any): " + anyoneCount); // Problem 1

            var everyoneCount = groupAnswers.Select(groupAnswer => groupAnswer.Replace(Environment.NewLine, string.Empty).GroupBy(eYes => eYes)
                .Count(group => group.Count() == groupAnswer.Count(c => c.Equals('\n')) + 1)).Sum();
            Console.WriteLine("Sum of yes (every): " + everyoneCount); // Problem 2
        }
    }
}
