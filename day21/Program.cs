using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day21
{
    class Program
    {
        static Dictionary<string, List<string>> allergensDict = new Dictionary<string, List<string>>();

        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var foods = streamReader.ReadToEnd().Split(Environment.NewLine);
            Console.WriteLine($"Non-allergen ingredients count: {CountSafeIngredients(foods)}");

            var allergens = GetDefinitiveAllergens(allergensDict).OrderBy(x => x.Key);
            var sortedAllergens = string.Join(",", allergens.Select(x=>x.Value.First()));
            Console.WriteLine($"Canonical dangerous ingredients list: {sortedAllergens}");
        }

        static void BuildInconclusiveAllergensDict(string[] foods)
        {
            for (int i = 0; i < foods.Length; i++)
            {
                var maybeAllergens = foods[i].Split("(contains");
                if (maybeAllergens.Length > 0)
                {
                    var allergens = maybeAllergens[1][..^1].Trim().Split(",");
                    for (int j = 0; j < allergens.Length; j++)
                    {
                        var allergen = allergens[j].Trim();
                        if (!allergensDict.ContainsKey(allergen))
                        {
                            allergensDict[allergen] = maybeAllergens[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        else
                        {
                            var newValues = allergensDict[allergen].Intersect(maybeAllergens[0].Split(" "));
                            allergensDict[allergen] = newValues.ToList();
                        }
                    }
                }
            }
        }

        static int CountSafeIngredients(string[] foods)
        {
            BuildInconclusiveAllergensDict(foods);
            var nonAllergenIngredients = GetSafeIngredients(foods);
            var allIngredients = GetAllIngredients(foods);

            int sum = 0;
            for (int i = 0; i < allIngredients.Length; i++)
            {
                if (nonAllergenIngredients.Contains(allIngredients[i]))
                {
                    sum++;
                }
            }
            return sum;
        }

        static string[] GetSafeIngredients(string[] foods)
        {
            var allIngredients = GetAllIngredients(foods);
            var allAllergens = allergensDict.SelectMany(y => y.Value);
            var filteredIngredients = allIngredients.Except(allAllergens);

            return filteredIngredients.ToArray();
        }

        static string[] GetAllIngredients(string[] foods)
        {
            var allIngredients = new List<string>();
            for (int i = 0; i < foods.Length; i++)
            {
                allIngredients.AddRange(foods[i].Split("(contains")[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            return allIngredients.ToArray();
        }

        private static Dictionary<string, List<string>> GetDefinitiveAllergens(Dictionary<string, List<string>> allergens)
        {
            var singleIdentifiers = allergens.Where(x => x.Value.Count == 1).ToArray();

            foreach (var allergen in allergens)
            {
                if (singleIdentifiers.Contains(allergen))
                {
                    continue;
                }
                allergen.Value.RemoveAll(x => singleIdentifiers.Select(y => y.Value[0]).Contains(x));
            }

            if (singleIdentifiers.Length == allergens.Count())
            {
                return singleIdentifiers.ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                return GetDefinitiveAllergens(allergens);
            }
        }
    }
}
