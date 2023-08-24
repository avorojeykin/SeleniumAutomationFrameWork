using OpenQA.Selenium.Chrome;
using Common.Enumerators;
using WpfScreenHelper;
using System;
using System.Windows;

namespace Common.Utilities
{
    public static class WebDriverHelper
    {
        public static ChromeOptions SetChromeOptions(EnumMonitorSelection monitorSelection, bool useCookies, string cookiesPath)
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            // silent diagnostics
            chromeOptions.AddArgument("--log-level=3");
            chromeOptions.AddArgument("--silent");

            // with cookies on login profile will be saved
            if (useCookies)
            {
                chromeOptions.AddArguments($"user-data-dir={cookiesPath}");
            }

            // pick on which monitor regarding main one to open a browser window
            if (monitorSelection != EnumMonitorSelection.None)
            {
                var monitor = monitorSelection == EnumMonitorSelection.Left
                ? Screen.FromPoint(new Point((int)Screen.PrimaryScreen.Bounds.Left - 1, (int)Screen.PrimaryScreen.Bounds.Top))
                : Screen.FromPoint(new Point((int)Screen.PrimaryScreen.Bounds.Right + 1, (int)Screen.PrimaryScreen.Bounds.Top));
                chromeOptions.AddArgument(String.Format("--window-position={0},{1}", monitor.Bounds.X, monitor.Bounds.Y));
            }
            return chromeOptions;
        }
    }
}
