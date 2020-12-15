using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace day4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);
            var passportsRaw = streamReader.ReadToEnd().Split(new string[] { Environment.NewLine + Environment.NewLine },
               StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"Passports with required fields: {GetPassportsWithRequiredFields(passportsRaw).Count}"); // Problem 1
            Console.WriteLine($"Valid passports: {GetValidPassports(passportsRaw).Count}"); // Problem 2
        }

        static List<Passport> GetValidPassports(string[] passportsRaw)
        {
            var passports = GetPassportsWithRequiredFields(passportsRaw);
            var passedPassports = new List<Passport>();

            // Hello darkness my old friend
            for (int i = 0; i < passports.Count; i++)
            {
                bool invalid = false;
                foreach (var (key, value) in passports[i].Fields)
                {
                    if (key.Equals("eyr"))
                    {
                        if (!ValidateNumber(value, 2020, 2030)) { invalid = true; break; };
                    }
                    else if (key.Equals("iyr"))
                    {
                        if (!ValidateNumber(value, 2010, 2020)) { invalid = true; break; };
                    }
                    else if (key.Equals("byr"))
                    {
                        if (!ValidateNumber(value, 1920, 2002)) { invalid = true; break; };
                    }
                    else if (key.Equals("hgt"))
                    {
                        if (!ValidateHeight(value)) { invalid = true; break; };
                    }
                    else if (key.Equals("ecl"))
                    {
                        if (!ValidateEyeColor(value)) { invalid = true; break; };
                    }
                    else if (key.Equals("hcl"))
                    {
                        if (!ValidateHairColor(value)) { invalid = true; break; };
                    }
                    else if (key.Equals("pid"))
                    {
                        if (!ValidatePassportId(value)) { invalid = true; break; };
                    }
                }

                if (!invalid)
                {
                    passedPassports.Add(passports[i]);
                }
            }
            return passedPassports;
        }

        static List<Passport> GetPassportsWithRequiredFields(string[] passportsRaw)
        {
            var validPassports = new List<Passport>();
            var requiredFields = new List<string> { "eyr", "hgt", "iyr", "ecl", "byr", "hcl", "pid" };
            for (int i = 0; i < passportsRaw.Length; i++)
            {
                var components = passportsRaw[i]
                    .Replace(Environment.NewLine, " ")
                    .Split(new char[] { ' ', ':' });
                var fields = components.Where((x, i) => i % 2 == 0);
                var values = components.Where((x, i) => i % 2 != 0);

                if (requiredFields.All(value => fields.Contains(value)))
                {
                    var dict = fields.Zip(values).ToDictionary(x => x.First, x => x.Second);
                    validPassports.Add(new Passport(dict));
                }
            }

            return validPassports;
        }

        static bool ValidateNumber(string value, int min, int max)
        {
            return int.TryParse(value, out int number) && number >= min && number <= max;
        }

        static bool ValidateHeight(string value)
        {
            return Regex.Match(value, "^(1[5-8][0-9]|19[0-3])cm|(59|6[0-9]|7[0-6])in$").Success;
        }

        static bool ValidateHairColor(string value)
        {
            return Regex.Match(value, "^#[0-9a-f]{6}$").Success;
        }

        static bool ValidateEyeColor(string value)
        {
            return Regex.Match(value, @"^(amb|blu|brn|gry|grn|hzl|oth)\b").Success;
        }

        static bool ValidatePassportId(string value)
        {
            return Regex.Match(value, "^[0-9]{9}$").Success;
        }

        struct Passport
        {
            public Dictionary<string, string> Fields { get; }

            public Passport(Dictionary<string, string> fields)
            {
                Fields = fields;
            }
        }
    }
}
