using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElectronicsStore
{
    public static class CsvHandler
    {
        public static void EnsureFileExists(string filePath, string header)
        {
            if (!File.Exists(filePath)) File.WriteAllText(filePath, header + Environment.NewLine, Encoding.UTF8);
        }

        public static int GenerateId(string filePath)
        {
            int maxId = File.ReadLines(filePath).Skip(1)
                        .Select(line => line.Split(';').FirstOrDefault())
                        .Where(idStr => int.TryParse(idStr, out _))
                        .Select(int.Parse)
                        .DefaultIfEmpty(0)
                        .Max();
            return maxId + 1;
        }

        public static void AppendLine(string filePath, string line)
        {
            File.AppendAllText(filePath, line + Environment.NewLine, Encoding.UTF8);
        }

        public static void RewriteFile(string filePath, string header, List<string> lines)
        {
            var allLines = new List<string> { header };
            allLines.AddRange(lines);
            File.WriteAllLines(filePath, allLines, Encoding.UTF8);
        }

        public static List<string> ReadAllLines(string filePath)
        {
            if (!File.Exists(filePath)) return new List<string>();
            return File.ReadLines(filePath).Skip(1).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
    }
}