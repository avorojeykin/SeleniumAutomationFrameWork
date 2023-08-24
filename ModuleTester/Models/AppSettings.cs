using System;
using System.Collections.Generic;
using System.Text;

namespace ModuleTester.Models
{
    public class AppSettings
    {
        public List<string> ServiceUIUrls { get; set; }
        public List<string> TestModules { get; set; }
        public string BrowserDriverFolder { get; set; }
        public string CookiesPath { get; set; }
        public int ExplicitWaitInSeconds { get; set; }
        public int FluentWaitInSeconds { get; set; }
        public bool AdditionalPauseFlag { get; set; }
        public int AdditionalPauseInSeconds { get; set; }
        public string CommonSettingsSubFolder { get; set; }
        public string ServiceSettingsSubFolder { get; set; }
        public bool DisplayLocalInfo { get; set; }
        public EmailSettings ResultInEmail { get; set; }
        public string ArchiveFile { get; set; }
    }
}
