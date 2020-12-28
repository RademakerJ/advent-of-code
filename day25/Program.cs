using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day25
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var publicKeys = streamReader.ReadToEnd().Split(Environment.NewLine).Select(x => int.Parse(x)).ToArray();
            var cardLoopSize = GetLoopSize(publicKeys[0]);
            var doorLoopSize = GetLoopSize(publicKeys[1]);

            Console.WriteLine($"Encryption key: {GetEncryptionKey(publicKeys[1], cardLoopSize)}");
        }

        static int GetLoopSize(int publicKey)
        {
            // Start with the value 1, then to DETERMINE THE LOOP SIZE:
            // Set the value to itself multiplied by the subject number (7).
            // Set the value to the remainder after dividing the value by 20201227.
            // Until you have the value of 'publicKey'.
            int subjectNumber = 7;
            int date = 20201227;
            int loopSize = 0;
            int value = 1;

            while (value != publicKey)
            {
                value *= subjectNumber;
                value %= date;
                loopSize++;
            }

            return loopSize;
        }

        static long GetEncryptionKey(int publicKey, int loopSize)
        {
            // Start with the value 1, then for the loop size:
            // Set the value to itself multiplied by the subject number ('publicKey').
            // Set the value to the remainder after dividing the value by 20201227.
            long value = 1;
            int subjectNumber = publicKey;
            int date = 20201227;

            for (int i = 0; i < loopSize; i++)
            {
                value *= subjectNumber;
                value %= date;
            }

            return value;
        }
    }
}
