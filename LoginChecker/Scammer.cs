using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoginChecker
{
    internal class Scammer
    {
        public string IP { get; private set; }
        public List<string> UserNames { get; private set; }
        private int loginAttempt = 0;

        public Scammer(string IP)
        {
            this.IP = IP;
            UserNames = new List<string>();
        }

        public void RegisterUsername(string username)
        {
            if(!UserNames.Contains(username)) UserNames.Add(username);
            loginAttempt++;
        }


        private void WriteTableLine(string line, int fullSize, char lastChar = '\n')
        {
            string formattedString = $"| {line}";
            Console.Write(formattedString);
            int noSpace = fullSize - formattedString.Length - 1;
            for (int i = 0; i < noSpace; i++) Console.Write(' ');
            Console.Write($"|{lastChar}");
        }

        string GetJValue(JObject json, string valueName)
        {
            int tries = 3;
            do
            try
            {
                return json[valueName].Value<string>();
            } catch
            {
                    System.Threading.Thread.Sleep(1500);
            } while (tries-- > 0);
            return "Error";

        }

        public void TableOfShame()
        {
            if (IP.StartsWith("192.168")) return;
            JObject apiResponse = JObject.Parse(new WebClient().DownloadString($"http://ipwho.is/{IP}"));

            string country = GetJValue(apiResponse, "country");
            Program.CheckCountry(country);
            for (int i = 0; i < UserNames.Count; i++) if (UserNames[i].StartsWith("invalid user")) UserNames[i] = UserNames[i].Replace("invalid user", null);

            string header = $"/----------------------{IP}----------------------\\";
            int fullSize = header.Length;
            Console.WriteLine(header);
            WriteTableLine($"IP Geolocation: {GetJValue(apiResponse, "continent")} - {country}({GetJValue(apiResponse, "country_code")})", fullSize);
            WriteTableLine($"Region/State: {GetJValue(apiResponse, "region")}", fullSize);
            WriteTableLine($"City: {GetJValue(apiResponse, "city")}", fullSize);
            WriteTableLine($"Login Attempt: {loginAttempt}", fullSize);
            WriteTableLine("UserNames:", fullSize);

            string userNameLine = string.Empty;
            foreach(string userName in UserNames)
            {
                string newString = userNameLine + userName + ", ";
                if (newString.Length >= fullSize - 2)
                {
                    WriteTableLine(userNameLine, fullSize);
                    userNameLine = string.Empty;
                }
                else userNameLine = newString;

            }
            WriteTableLine(userNameLine + "\b\b ", fullSize + 4);
            Console.Write("\\"); //delete the last comma
            for (int i = 0; i < fullSize - 2; i++) Console.Write('-');
            Console.WriteLine("/");
        }

    }
}
