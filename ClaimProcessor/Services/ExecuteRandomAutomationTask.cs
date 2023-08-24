using Common.Cryptography;
using Common.Models;
using Common.Utilities;
using Common.Enumerators;
using Common.AutoItXService;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using ClaimProcessor.Models;
using System.Threading.Tasks;

namespace ClaimProcessor.Services
{
    public class ExecuteRandomAutomationTask : TaskBase<ConfigurationSettings>
    {
        #region Override Properties
        public override string SettingsFileName { get { return SERVICE_SETTINGS_FILE_NAME; } }
        #endregion

        #region Constants
        private const string SERVICE_SETTINGS_FILE_NAME = "ConfigurationSettings.json";
        private const string FILE_NAME_CLAIM_AMOUNT_PREFIX = "_total_";
        private const int DIALOG_OPEN_FILE_LISTENING_PERIOD_IN_SECONDS = 30;
        private const string ERROR_WEB_DRIVER_NOT_FOUND = "Web driver not found.";
        private const string ERROR_NO_SUCH_ELEMENT = "Error no such element.";
        private const string ERROR_EXPORT_CONFIGURATION = "Error performing step \"Export Configuration\". Details: {0}";
        #endregion

        #region Constructors
        public ExecuteRandomAutomationTask(string homeUrl, AES aes, EnumLoginType loginType, LoginSettings loginSettings,
            string driverPath, int waitTimeInSeconds, ChromeOptions chromeOptions,
            TimeSleepDetails additionalPause, int fluentWaitInSeconds, string serviceSettingsFileSubFolder) : base(homeUrl, aes, loginType, loginSettings,
            driverPath, waitTimeInSeconds, chromeOptions, 
            additionalPause, fluentWaitInSeconds, serviceSettingsFileSubFolder)
        { }
        #endregion        

        #region Override Methods
        protected override void RunTest(string fileDirectory)
        {
            ProcessResponseEmail result = new ProcessResponseEmail();
            List<string> configurationsFailed = new List<string>();
            List<string> configurationsPassed = new List<string>();
            try
            {
                Console.WriteLine("Starting Hello World...");
                navigateToGoogleAndTypeHelloWorld(_settings.HomepageLoadClass);
                Console.WriteLine(" Hello World completed.");
            }
            catch (DriverServiceNotFoundException ex)
            {
                Response.Log = ex.Message;
                throw new Exception(ERROR_WEB_DRIVER_NOT_FOUND);
            }
            catch (NoSuchElementException ex)
            {
                Response.Log = ex.Message;
                throw new Exception(string.Format(ERROR_NO_SUCH_ELEMENT));
            }
            catch (Exception ex)
            {
                Response.Log = CommonUtilities.GetExceptionString(ref ex);
                throw new Exception(string.Format(ERROR_EXPORT_CONFIGURATION, CommonUtilities.GetExceptionString(ref ex)));
            }
            finally
            {
                Response.ReturnedResponseObject = result;
            }
        }
        #endregion
    }
}