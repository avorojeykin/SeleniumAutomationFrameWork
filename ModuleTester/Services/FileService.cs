using Common.Utilities;
using System;

namespace ModuleTester.Services
{
    public class FileService
    {

        private const string TIME_STAMP_FORMAT = "yyyy-MMM-dd.hh-mm-ss.tt";

        public static string WriteArchive(string fullFileName, string content)
        {
            string result = "ok";
            try
            {
                fullFileName = string.Format(fullFileName, DateTime.Now.ToString(TIME_STAMP_FORMAT));
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullFileName));
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(fullFileName))
                {
                    sw.WriteLine(content);
                }
                return result;
            }
            catch (Exception ex)
            {
                return CommonUtilities.GetExceptionString(ref ex);
            }
        }
    }
}
