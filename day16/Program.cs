using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day16
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");

            using StreamReader streamReader = new StreamReader(filePath);
            var inputSplit = streamReader.ReadToEnd().Split(
                new string[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // First part = identifiers
            var identifiers = GetIdentifiers(inputSplit[0]);

            // Second part = my ticket
            var myTicket = GetTicket(inputSplit[1]);

            // Third part = other tickets
            var otherTickets = GetTickets(inputSplit[2]);

            // Invalid tickets + invalid sum
            (Ticket[] invalidTickets, int invSum) = GetInvalidNumbersSum(identifiers, otherTickets);

            Console.WriteLine($"Solution problem 1: {invSum}"); // Problem 1

            var validOtherTickets = otherTickets.Except(invalidTickets).ToArray();
            (Dictionary<Identifier, List<int>> dict, long depMult) = GetDepartureMult(identifiers, validOtherTickets, myTicket);

            Console.WriteLine($"Solution problem 1: {depMult}"); // Problem 2
        }

        private static (Dictionary<Identifier, List<int>>, long) GetDepartureMult(Identifier[] identifiers, Ticket[] tickets, Ticket myTicket)
        {
            // Identifier vs. columns that fit the identifier.
            var dict = new Dictionary<Identifier, List<int>>();
            foreach (var identifier in identifiers)
            {
                dict[identifier] = new List<int>();
            }

            for (int i = 0; i < identifiers.Length; i++)
            {
                // Select column.
                var numbers = tickets.Select(x => x.Numbers[i]).ToArray();

                for (int j = 0; j < identifiers.Length; j++)
                {
                    // Match column against every identifier.
                    var first = identifiers[j].MinMaxFirst;
                    var second = identifiers[j].MinMaxSecond;

                    var validNumbers = numbers.Where(x => (x >= first[0] && x <= first[1]) || (x >= second[0] && x <= second[1])).ToArray();

                    // If everything fits, then it is a possible match.
                    if (validNumbers.Length == numbers.Length)
                    {
                        dict[identifiers[j]].Add(i);
                    }
                }
            }

            dict = CrossOutDuplicates(dict);
            long mult = MultiplyDepartureCategories(dict, myTicket);

            return (dict, mult);
        }

        private static Dictionary<Identifier, List<int>> CrossOutDuplicates(Dictionary<Identifier, List<int>> identifiers)
        {
            // Single value, so that must be the category for that identifier.
            var singleIdentifiers = identifiers.Where(x => x.Value.Count == 1).ToArray();

            // Remove these values for the other identifiers.
            foreach (var identifier in identifiers)
            {
                if (singleIdentifiers.Contains(identifier))
                {
                    continue;
                }
                identifier.Value.RemoveAll(x => singleIdentifiers.Select(y => y.Value[0]).Contains(x));
            }

            if (singleIdentifiers.Length == identifiers.Count())
            {
                return singleIdentifiers.ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                return CrossOutDuplicates(identifiers);
            }
        }

        private static long MultiplyDepartureCategories(Dictionary<Identifier, List<int>> identifiers, Ticket myTicket)
        {
            var departureIdentifiers = identifiers.Where(x => x.Key.Name.StartsWith("departure"));

            var ticketIndices = myTicket.Numbers
                .Select((x, i) => new { x = x, i = i })
                .Where(x => departureIdentifiers.Select(x => x.Value[0]).Contains(x.i))
                .ToArray();

            long mult = ticketIndices[0].x;
            for (int i = 1; i < ticketIndices.Length; i++)
            {
                mult *= ticketIndices[i].x;
            }

            return mult;
        }

        private static (Ticket[], int) GetInvalidNumbersSum(Identifier[] identifiers, Ticket[] tickets)
        {
            var sum = 0;
            var invalidTickets = new List<Ticket>();
            var minMaxes = identifiers
                .Select(x => new { first = x.MinMaxFirst, second = x.MinMaxSecond });

            foreach (var ticket in tickets)
            {
                var invalidNumbers = new List<int>();
                var numbers = ticket.Numbers;

                foreach (var number in numbers)
                {
                    var minMaxApplies = minMaxes.Where(x => number >= x.first[0] && number <= x.first[1] ||
                               (number >= x.second[0] && number <= x.second[1])).ToArray();

                    if (minMaxApplies.Length == 0)
                    {
                        invalidNumbers.Add(number);
                        if (!invalidTickets.Contains(ticket)) { invalidTickets.Add(ticket); }
                    }
                }

                sum += invalidNumbers.Sum();
            }
            return (invalidTickets.ToArray(), sum);
        }

        private static Identifier[] GetIdentifiers(string input)
        {
            var identifiers = new List<Identifier>();
            var lines = input.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                var colonSplit = line.Split(new string[] { ":", " or " }, StringSplitOptions.RemoveEmptyEntries);
                var name = colonSplit[0];

                var minMaxFirst = colonSplit[1].Trim().Split('-');
                var minMaxFirstRange = new int[] { int.Parse(minMaxFirst[0]), int.Parse(minMaxFirst[1]) };

                var minMaxSecond = colonSplit[2].Trim().Split('-');
                var minMaxSecondRange = new int[] { int.Parse(minMaxSecond[0]), int.Parse(minMaxSecond[1]) };

                identifiers.Add(new Identifier(name, minMaxFirstRange, minMaxSecondRange));
            }
            return identifiers.ToArray();
        }

        private static Ticket GetTicket(string input)
        {
            var line = input.Split(new string[] { ":", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[1];
            var numbers = line.Split(',').Select(x => int.Parse(x)).ToArray();
            return new Ticket(numbers);
        }

        private static Ticket[] GetTickets(string input)
        {
            var tickets = new List<Ticket>();
            var lines = input.Split(new string[] { ":", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[1..];
            foreach (var line in lines)
            {
                var numbers = line.Split(',').Select(x => int.Parse(x)).ToArray();
                tickets.Add(new Ticket(numbers));
            }

            return tickets.ToArray();
        }

        private static int[] GetNumberRange(int min, int max)
        {
            var number = min;
            var numbers = new List<int>() { number };
            while (number < max)
            {
                number++;
                numbers.Add(number);
            }
            numbers.Add(max);
            return numbers.ToArray();
        }

        struct Ticket
        {
            public int[] Numbers { get; }

            public Ticket(int[] numbers)
            {
                Numbers = numbers;
            }
        }

        struct Identifier
        {
            public string Name { get; }
            public int[] MinMaxFirst { get; }
            public int[] MinMaxSecond { get; }

            public Identifier(string name, int[] minMaxFirst, int[] minMaxSecond)
            {
                Name = name;
                MinMaxFirst = minMaxFirst;
                MinMaxSecond = minMaxSecond;
            }
        }
    }
}
