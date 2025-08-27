using System;
using System.IO;
using System.Text;
using System.Configuration;

class Program
{
    static void Main()
    {
        // Read config values
        string folderPath = ConfigurationManager.AppSettings["FolderPath"];
        string endingDigits = ConfigurationManager.AppSettings["EndingDigits"] ?? "";
        string endingWantStr = ConfigurationManager.AppSettings["EndingWant"];
        string startDigits = ConfigurationManager.AppSettings["StartDigits"] ?? "";
        string startWantStr = ConfigurationManager.AppSettings["StartWant"];

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Console.WriteLine("Invalid folder path in config. Exiting...");
            return;
        }

        // Parse booleans with defaults
        bool endingWant = true;
        bool startWant = true;
        if (!string.IsNullOrWhiteSpace(endingWantStr))
            bool.TryParse(endingWantStr, out endingWant);
        if (!string.IsNullOrWhiteSpace(startWantStr))
            bool.TryParse(startWantStr, out startWant);

        // Limit digit length
        if (endingDigits.Length > 10 || startDigits.Length > 10)
        {
            Console.WriteLine("EndingDigits or StartDigits too long. Exiting...");
            return;
        }

        string fileName = "";

        if (!string.IsNullOrEmpty(startDigits) && startWant)
        {
            fileName += startDigits + "_";
        }

        fileName += "filtered_numbers";

        if (!string.IsNullOrEmpty(endingDigits) && endingWant)
        {
            fileName += "_" + endingDigits;
        }

        fileName += ".txt";


        string outputFile = Path.Combine(folderPath, fileName);

        const int totalLen = 10;
        int startLen = startDigits.Length;
        int endLen = endingDigits.Length;

        try
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var writer = new StreamWriter(outputFile, false, Encoding.ASCII, 65536))
            {
                if (startWant && endingWant)
                {
                    int fillLen = totalLen - (startLen + endLen);
                    if (fillLen < 0)
                    {
                        Console.WriteLine("StartDigits + EndingDigits length exceeds 10 digits.");
                        return;
                    }

                    long maxFill = (long)Math.Pow(10, fillLen) - 1;
                    for (long fill = 0; fill <= maxFill; fill++)
                    {
                        string middle = fill.ToString("D" + fillLen);
                        string num = startDigits + middle + endingDigits;

                        // Validate start and end explicitly
                        if (!num.StartsWith(startDigits)) continue;
                        if (!num.EndsWith(endingDigits)) continue;

                        writer.WriteLine(num);
                    }
                }
                else if (startWant && !endingWant)
                {
                    int fillLen = totalLen - startLen;
                    if (fillLen < 0)
                    {
                        Console.WriteLine("StartDigits length exceeds 10 digits.");
                        return;
                    }

                    long maxFill = (long)Math.Pow(10, fillLen) - 1;
                    for (long fill = 0; fill <= maxFill; fill++)
                    {
                        string middle = fill.ToString("D" + fillLen);
                        string num = startDigits + middle;

                        // Apply start digit filter only
                        if (!num.StartsWith(startDigits)) continue;

                        // Ending filter ignored because endingWant is false

                        writer.WriteLine(num);
                    }
                }
                else if (!startWant && endingWant)
                {
                    int fillLen = totalLen - endLen;
                    if (fillLen < 0)
                    {
                        Console.WriteLine("EndingDigits length exceeds 10 digits.");
                        return;
                    }

                    long maxFill = (long)Math.Pow(10, fillLen) - 1;
                    for (long fill = 0; fill <= maxFill; fill++)
                    {
                        string middle = fill.ToString("D" + fillLen);
                        string num = middle + endingDigits;

                        // Ignore start digit filter because startWant is false

                        if (!num.EndsWith(endingDigits)) continue;

                        writer.WriteLine(num);
                    }
                }
                else // startWant == false && endingWant == false
                {
                    int fillLen = totalLen;
                    if (startLen + endLen > totalLen)
                    {
                        Console.WriteLine("StartDigits + EndingDigits length exceeds 10 digits.");
                        return;
                    }

                    long maxNum = (long)Math.Pow(10, fillLen) - 1;
                    for (long num = 0; num <= maxNum; num++)
                    {
                        string numStr = num.ToString("D" + fillLen);
                        // No start or end filtering - write all
                        writer.WriteLine(numStr);
                    }
                }
            }

            Console.WriteLine($"Done! File created at: {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error writing file: " + ex.Message);
        }
    }
}
