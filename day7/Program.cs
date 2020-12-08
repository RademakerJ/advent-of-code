using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace day7
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var pattern = @"\b(?!\bcontain|,\b)\w+\b.*?(bags|bag).*?"; // Hello darkness my old friend
            var bags = new List<Bag>();

            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                var formula = Regex.Matches(line, pattern).Select(m => m.Value).ToArray();

                var name = "";
                var ingredients = new Dictionary<string, int>();
                for (int i = 0; i < formula.Length; i++)
                {
                    var lastWord = formula[i].Split(" ")[^1];
                    var realBag = formula[i].Replace(lastWord, "");

                    if (i == 0)
                    {
                        name = realBag.Trim();
                        continue;
                    }

                    if (!realBag.Contains("no other"))
                    {
                        ingredients.Add(realBag[1..].Trim(), int.Parse(realBag.Substring(0, 1)));
                    }
                }

                bags.Add(new Bag(name, ingredients));
            }

            int shinyGoldBagCount = 0;
            foreach (var bag in bags)
            {
                if (bag.Name.Equals("shiny gold", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                shinyGoldBagCount += HasShinyBag(bags, bag.Name) ? 1 : 0;
            }

            Console.WriteLine("Bag count: " + shinyGoldBagCount); // Problem 1
            //Console.WriteLine("Bag count: " + GetBagCount(bags, "shiny gold")); // Problem 2
        }

        static int GetBagCount(List<Bag> bags, string bagName)
        {
            int count = 0;
            var bag = bags.First(bag => bag.Name.Equals(bagName));

            if (bag.Ingredients.Count == 0)
            {
                return 0;
            }

            foreach (var (key, value) in bag.Ingredients)
            {
                count += value + GetBagCount(bags, key) * value;
            }

            return count;
        }

        static bool HasShinyBag(List<Bag> bags, string bagName)
        {
            bool containsBag = false;
            var bag = bags.First(bag => bag.Name.Equals(bagName));
            if (bag.Ingredients.ContainsKey("shiny gold"))
            {
                return true;
            }

            if (bag.Ingredients.Count == 0)
            {
                return false;
            }

            foreach (var (key, _) in bag.Ingredients)
            {
                containsBag |= HasShinyBag(bags, key);
            }

            return containsBag;
        }

        class Bag
        {
            public string Name { get; }

            public Dictionary<string, int> Ingredients { get; }

            public Bag(string name, Dictionary<string, int> ingredients)
            {
                Name = name;
                Ingredients = ingredients;
            }

        }
    }
}
