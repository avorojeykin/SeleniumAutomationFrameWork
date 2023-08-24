using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Common.Utilities
{
    public static class CommonUtilities
    {

        public static T DeserializeJsontoObject<T>(string fileFullPath)
        {
            using (StreamReader reader = new StreamReader(fileFullPath))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public static string SerializeObjecttoJson(this object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
        }

        public static string GetExceptionString(ref Exception oE)
        {
            System.Text.StringBuilder oSB = new System.Text.StringBuilder(300);
            oSB.Append("Error type of System.Exception occured:\r\n");
            oSB.Append("InnerException:\t" + oE.InnerException + "\r\n");
            oSB.Append("Message:\t" + oE.Message + "\r\n");
            oSB.Append("Source:\t" + oE.Source + "\r\n");
            oSB.Append("TargetSite:\t" + oE.TargetSite + "\r\n");
            oSB.Append("Stack:\t" + oE.StackTrace + "\r\n");
            return oSB.ToString();
        }

        public static string CSharpScriptToString(string script)
        {
            string result = null;
            if (!string.IsNullOrEmpty(script))
            {
                var response = CSharpScript.EvaluateAsync(script).Result;
                result = response.ToString();
            }
            return result;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = path;
            fileopener.Start();
        }

        public static bool IsValueDecimal(string valueToCheck)
        {
            decimal value;
            if (Decimal.TryParse(valueToCheck, out value))
            { return true; }
            else
            { return false; }
        }
    }
}
