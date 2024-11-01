using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sshinsp
{
    internal class Program
    {
        public static List<(string, int)> CountryCount = new List<(string, int)>();

        private static void SortList()
        {
            List<(string, int)> OrderedList = new List<(string, int)>();
            do
            {
                (string, int) biggestCountry = ("", 0);
                foreach (var country in CountryCount) if (country.Item2 > biggestCountry.Item2) biggestCountry = country;
                OrderedList.Add(biggestCountry);
                CountryCount.Remove(biggestCountry);
            } while (CountryCount.Count > 0);
            CountryCount = OrderedList;
        }

        public static void CheckCountry(string country)
        {
            bool found = false;
            for (int i = 0; i < CountryCount.Count; i++)
            {
                (string, int) svdcountry = CountryCount[i];
                if (svdcountry.Item1 == country)
                {
                    svdcountry.Item2++;
                    found = true;
                    CountryCount[i] = svdcountry;
                    break;
                }
            }
            if (!found) CountryCount.Add((country, 1));
        }
        static void Main(string[] args)
        {
            string[] fileContent;
            try
            {
                fileContent = File.ReadAllLines(args[0]).Where(x => x.ToLower().Contains("failed") || x.ToLower().Contains("authenticating")).ToArray<string>();
                if(fileContent.Length == 0)
                {
                    Console.WriteLine("Nothing to analyze");
                    Environment.Exit(0);
                }
            }
            catch
            {
                Console.WriteLine("Usage:\n\tsshinsp <pathToLogFile>");
                return;
            }

            Console.WriteLine("Checking..");
            List<Scammer> scammers = new List<Scammer>();
            foreach (string line in fileContent)
            {
                string ip, user;
                try
                {
                    ip = FilterData(line, out user);
                }
                catch
                {
                    continue;
                }
                bool found = false;

                foreach (Scammer scammer in scammers)
                {
                    if (scammer.IP == ip)
                    {
                        scammer.RegisterUsername(user);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Scammer bastard = new Scammer(ip);
                    bastard.RegisterUsername(user);
                    scammers.Add(bastard);
                }
            }

            foreach (Scammer pieceOfShit in scammers) pieceOfShit.TableOfShame();

            Console.WriteLine("\nI'm most wanted from");
            SortList();
            foreach ((string, int) country in CountryCount) Console.WriteLine($" * {country.Item1} - {country.Item2}");

            Console.WriteLine("Press a key to exit..");
            Console.ReadKey();
        }

        private static string FilterData(string line, out string userName)
        {
            if (line.ToLower().Contains("failed"))
            {
                int userStart = line.IndexOf("for") + 4; //for + space
                int userEnd = line.IndexOf("from") - 1;
                int ipStart = userEnd + 6; //from + space + -1
                int ipEnd = IndexOfWholeWord(line, "port") - 1;
                userName = line.Substring(userStart, userEnd - userStart);
                return line.Substring(ipStart, ipEnd - ipStart);
            }
            else
            {
                int start = line.IndexOf("user") + 5;
                int end = line.LastIndexOf("port") - 1;
                string[] filteredLine = line.Substring(start, end - start).Split();
                userName = filteredLine[0];
                return filteredLine[1];
            }

        }

        static int IndexOfWholeWord(string str, string word)
        {
            for (int j = 0; j < str.Length &&
                (j = str.IndexOf(word, j, StringComparison.Ordinal)) >= 0; j++)
                if ((j == 0 || !char.IsLetterOrDigit(str, j - 1)) &&
                    (j + word.Length == str.Length || !char.IsLetterOrDigit(str, j + word.Length)))
                    return j;
            return -1;
        }
    }
}
