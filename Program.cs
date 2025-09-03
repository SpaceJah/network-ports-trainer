using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

class Program
{
    static void Main()
    {
        var data = LoadCsv("Ports.csv");
        if (data.Count == 0)
        {
            Console.WriteLine("No data loaded. Make sure Ports.csv is in the same folder as the executable.");
            return;
        }

        // Intro text
        Console.WriteLine("=== Ports & Protocols Drill ===\n");
        Console.WriteLine("This program helps you memorize ports, protocols, and related info.");
        Console.WriteLine();
        Console.WriteLine("How it works:");
        Console.WriteLine("1. Choose which column you want to be shown (the QUESTION).");
        Console.WriteLine("2. Choose which column you want to recall (the ANSWER).");
        Console.WriteLine("3. The drill will keep asking until you get every item correct.");
        Console.WriteLine();
        Console.WriteLine("Column meanings:");
        Console.WriteLine(" - Protocol: The short identifier (e.g. SSH, FTP, DNS).");
        Console.WriteLine(" - Port: The network port(s), including protocol (e.g. tcp/22, udp/67, udp/68).");
        Console.WriteLine(" - Name: The full service name (e.g. Secure Shell, File Transfer Protocol).");
        Console.WriteLine(" - Description: What the service does (e.g. \"Encrypted console access\").");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine(" - If QUESTION = Protocol (SSH) and ANSWER = Port, type: tcp/22");
        Console.WriteLine(" - If QUESTION = Name (File Transfer Protocol) and ANSWER = Port, type: tcp/20, tcp/21");
        Console.WriteLine();
        Console.WriteLine("Type \"quit\" at any time to exit.\n");

        var header = data[0];
        while (true)
        {
            Console.WriteLine("Choose the QUESTION column:");
            for (int i = 0; i < header.Count; i++)
                Console.WriteLine($"{i + 1}. {header[i]}");

            int colQuestion = GetColumnChoice(header.Count);

            Console.WriteLine("Choose the ANSWER column (not the same):");
            for (int i = 0; i < header.Count; i++)
                Console.WriteLine($"{i + 1}. {header[i]}");

            int colAnswer = GetColumnChoice(header.Count, colQuestion);

            RunDrill(data, colQuestion, colAnswer);

            Console.Write("Play another round? (y/n): ");
            string again = Console.ReadLine()?.Trim().ToLower();
            if (again != "y" && again != "yes")
                break;
        }
    }

    static int GetColumnChoice(int max, int exclude = -1)
    {
        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int idx) && idx >= 1 && idx <= max && idx - 1 != exclude)
                return idx - 1;
            Console.WriteLine("Invalid selection.");
        }
    }

    static void RunDrill(List<List<string>> data, int colQuestion, int colAnswer)
    {
        var header = data[0];
        var dataRows = data.GetRange(1, data.Count - 1);
        var rand = new Random();
        var queue = new List<List<string>>(dataRows);

        int score = 0, total = 0;

        Console.WriteLine($"\n=== Drill Started: {header[colQuestion]} → {header[colAnswer]} ===\n");

        while (queue.Count > 0)
        {
            int idx = rand.Next(queue.Count);
            var row = queue[idx];
            string questionVal = CleanField(row[colQuestion]);
            string correctAnswer = CleanField(row[colAnswer]);

            Console.WriteLine($"{header[colQuestion]}: {questionVal}");
            Console.Write("> ");
            string userInput = Console.ReadLine()?.Trim();

            if (string.Equals(userInput, "quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Final score: {score}/{total}");
                return;
            }

            total++;

            if (Normalize(userInput) == Normalize(correctAnswer))
            {
                Console.WriteLine("✅ Correct!\n");
                score++;
                queue.RemoveAt(idx);
            }
            else
            {
                Console.WriteLine($"❌ Wrong! Correct answer: {correctAnswer}\n");
            }
        }

        Console.WriteLine($"Round complete! Final score: {score}/{total}\n");
    }

    static string Normalize(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Trim().ToLower();
    }

    static List<List<string>> LoadCsv(string path)
    {
        var result = new List<List<string>>();
        using (var parser = new TextFieldParser(path))
        {
            parser.SetDelimiters(new string[] { "," });
            parser.HasFieldsEnclosedInQuotes = true;
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                var row = new List<string>();
                foreach (var f in fields)
                {
                    row.Add(CleanField(f));
                }
                result.Add(row);
            }
        }
        return result;
    }

    static string CleanField(string s)
    {
        if (s == null) return "";
        s = s.Trim();
        if (s.StartsWith("\"") && s.EndsWith("\"") && s.Length > 1)
            s = s.Substring(1, s.Length - 2);
        return s.Trim();
    }
}
