using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day8
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var instructions = new List<string>();
            while (!streamReader.EndOfStream)
            {
                instructions.Add(streamReader.ReadLine());
            }

            Console.WriteLine($"Accumulator value: {Execute(instructions).accVal}"); // Problem 1
            Console.WriteLine($"Accumulator value: {BruteForceJmpToNop(instructions)}"); // Problem 2
        }

        static int BruteForceJmpToNop(List<string> instructions)
        {
            var jmpIndices = instructions
                .Select((instruction, i) => new { Instruction = instruction, Index = i })
                .Where(x => x.Instruction.Contains("jmp"))
                .Select(x => x.Index);

            foreach (var jmpIndex in jmpIndices)
            {
                var instructionsCopy = new List<string>(instructions);
                instructionsCopy[jmpIndex] = instructionsCopy[jmpIndex].Replace("jmp", "nop");

                (int accVal, bool infiniteLoop) = Execute(instructionsCopy);

                if (!infiniteLoop)
                {
                    return accVal;
                }
            }
            return 0;
        }

        static (int accVal, bool infiniteLoop) Execute(List<string> instructions)
        {
            var index = 0;
            var instructionsVisited = new List<int> { 0 };
            var infiniteLoopDetected = false;
            var accumulatorValue = 0;

            while (!infiniteLoopDetected && index < instructions.Count)
            {
                var currentLine = instructions[index];
                (int skip, int accVal) = GetSkip(currentLine);
                accumulatorValue += accVal;

                index = instructionsVisited[instructionsVisited.Count - 1] + skip;
                if (instructionsVisited.Contains(index))
                {
                    infiniteLoopDetected = true;
                }
                instructionsVisited.Add(index);
            }
            return (accumulatorValue, infiniteLoopDetected);
        }

        static (int skip, int accVal) GetSkip(string instruction)
        {
            var splitInstruction = instruction.Split(" ");
            var isPlus = splitInstruction[1].StartsWith("+");
            var value = int.Parse(splitInstruction[1][1..]);

            if (splitInstruction[0].Equals("acc"))
            {
                return (1, isPlus ? value : -value);
            }
            else if (splitInstruction[0].Equals("jmp"))
            {
                return (isPlus ? value : -value, 0);
            }

            return (1, 0);
        }
    }
}
