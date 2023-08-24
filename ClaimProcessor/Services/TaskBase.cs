using Common.Cryptography;
using Common.Models;
using Common.Utilities;
using Common.Enumerators;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using Common.Interfaces;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Bibliography;

namespace ClaimProcessor.Services
{
    public abstract class TaskBase<T> : IConfigurationProcessor
    {
        #region Properties
        public ProcessResponse Response { get; set; }
        #endregion

        #region Abstract Properties
        public abstract string SettingsFileName { get; }
        #endregion

        #region Delegates
        protected delegate void login();
        #endregion

        #region Fields
        protected string _homeUrl;
        protected AES _aes;
        protected IWebDriver _driver;
        protected WebDriverWait _wait;
        protected WebDriverWait _shortWait;
        protected login _login;
        protected T _settings;
        protected LoginSettings _loginSettings;
        protected string _driverPath;
        protected int _waitTimeInSeconds;
        protected ChromeOptions _chromeOptions;
        protected TimeSleepDetails _additionalPause;
        protected int _fluentWaitInSeconds;
        protected string _serviceSettingsFileSubFolder = string.Empty;
        protected int _configurationsExported = 0;
        #endregion

        #region Constants
        private const int MAX_WAIT_FOR_ELEMENT_TO_BECOME_UNAVAILABLE = 120;
        private const int WAIT_POLLING_INTERVAL_IN_MILLISECONDS = 250;
        private const string START = "********** Start {0} **********\r\n";
        private const string THE_END = "**********The End**********\r\n";
        #endregion

        #region Constructors
        protected TaskBase(string homeUrl, AES aes, EnumLoginType loginType, LoginSettings loginSettings,
            string driverPath, int waitTimeInSeconds, ChromeOptions chromeOptions,
            TimeSleepDetails additionalPause, int fluentWaitInSeconds, string serviceSettingsFileSubFolder)
        {
            _homeUrl = homeUrl;
            _aes = aes;
            if (loginType == EnumLoginType.SSO)
            {
                _login = Login2FA;
            }
            else
            {
                _login = LoginUsernamePassword;
            }
            _loginSettings = loginSettings;
            _driverPath = driverPath;
            _waitTimeInSeconds = waitTimeInSeconds;
            _chromeOptions = chromeOptions;
            _additionalPause = additionalPause;
            _fluentWaitInSeconds = fluentWaitInSeconds;
            _serviceSettingsFileSubFolder = serviceSettingsFileSubFolder;
        }
        #endregion

        #region Public Methods
        public void Run(string fileDirectory)
        {
            try
            {
                _configurationsExported = 0;               
                _settings = CommonUtilities.DeserializeJsontoObject<T>(Path.Combine(_serviceSettingsFileSubFolder,
                    SettingsFileName));
                InitializeResponse();
                Response.Log = string.Format(START, "Start Hello World");
                SetDriverDetails();
                RunTest(fileDirectory);
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
            finally
            {
                _driver.Quit();
                Response.Log = Response.ProcessDetails;
                Response.Log = THE_END;
            }
        }
        #endregion

        #region Private Methods
        private void InitializeResponse()
        {
            Response = new ProcessResponse
            {
                IsSuccess = true,
                ErrorDescription = string.Empty,
                SystemError = string.Empty,
                WarningDescription = string.Empty,
                ReturnedResponseObject = null,
                ProcessDetails = string.Empty
            };
        }

        private void SetDriverDetails()
        {
            try
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(_driverPath);
                service.HideCommandPromptWindow = true;
                _driver = new ChromeDriver(service, _chromeOptions);
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_waitTimeInSeconds));
                _shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_fluentWaitInSeconds));
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        private void Login2FA()
        {
            if (_wait.IsWebElementFound(By.Id(_loginSettings.HomePageButtonId)))
            {
                Response.Log = "Click on \"LOGIN WITH SSO\" button";
                _wait.GetElementIfVisibleAndEnabled(By.Id(_loginSettings.HomePageButtonId)).Click();
            }

            Response.Log = "Check if current page is a login form";
            if (!_shortWait.IsWebElementFound(By.Id(_loginSettings.EmailSubmitButtonId)))
            { return; }

            Response.Log = "Enter user email and click next";
            IWebElement usernameBox = _wait.GetElementIfVisibleAndEnabled(By.Id(_loginSettings.EmailBoxId));
            usernameBox.SendKeys(_loginSettings.User);
            _driver.FindElement(By.Id(_loginSettings.EmailSubmitButtonId)).Click();

            Response.Log = "Decrypt and enter password and click next";
            IWebElement passwordBox = _wait.GetElementIfVisibleAndEnabled(By.Id(_loginSettings.PasswordBoxId));
            passwordBox.SendKeys(_aes.DecryptData(_loginSettings.Pwd));
            _driver.FindElement(By.Id(_loginSettings.PasswordSubmitButtonId)).Click();

            Response.Log = "Click yes to stay signed in";
            _wait.GetElementIfVisibleAndEnabled(By.Id(_loginSettings.StaySignInButtonId)).Click();
        }

        private void LoginUsernamePassword()
        {
            if (!_wait.IsWebElementFound(By.Id(_loginSettings.EmailBoxId)))
            { return; }

            Response.Log = "Enter user name";
            IWebElement usernameBox = _wait.GetElementIfVisibleAndEnabled(By.Id(_loginSettings.EmailBoxId));
            usernameBox.SendKeys(_loginSettings.User);

            Response.Log = "Decrypt and enter password and click next";
            IWebElement passwordBox = _wait.GetElementIfVisibleAndEnabled(By.Id(_loginSettings.PasswordBoxId));
            passwordBox.SendKeys(_aes.DecryptData(_loginSettings.Pwd));

            Response.Log = "Click on login button";
            _wait.GetElementIfVisibleAndEnabled(By.CssSelector(_loginSettings.LoginButtonCssSelector)).Click();
        }

        protected void PageLoadingWaitOut(string settingsPageLoaderId)
        {
            IWebElement pageLoadingElement = _driver.FindElementNullable(By.Id(settingsPageLoaderId));
            if (pageLoadingElement != null)
            {
                try
                {
                    pageLoadingElement.WaitForElementToBecomeNotDisplayed(MAX_WAIT_FOR_ELEMENT_TO_BECOME_UNAVAILABLE);
                }
                catch (StaleElementReferenceException)
                {
                    Response.Log = $"StaleElementReferenceException for Page Loading Element \"{settingsPageLoaderId}\". Ignore";
                }
            }
        }       

        protected void GoBackToPreviousPage()
        {
            Response.Log = $"Go back to previous page";
            _driver.Navigate().Back();
        }

        protected string InformationPopup(string settingsPopupMessageCssSelector, bool settingsPopupClick)
        {
            string result = string.Empty;
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(_driver);
            IWebElement notificationContainer;
            try
            {
                notificationContainer = fluentWait.GetElementIfVisibleAndEnabled(TimeSpan.FromSeconds(_fluentWaitInSeconds),
                    TimeSpan.FromMilliseconds(WAIT_POLLING_INTERVAL_IN_MILLISECONDS),
                    By.CssSelector(settingsPopupMessageCssSelector));
            }
            catch (Exception)
            {
                return result;
            }
            IWebElement paragraphElement = notificationContainer.FindElements(By.TagName("p")).FirstOrDefault();
            if (paragraphElement != null)
            {
                result = paragraphElement.Text;
            }
            if (settingsPopupClick)
            {
                Response.Log = "Click on notification message";
                IWebElement infoPopup = _wait.GetElementIfVisibleAndEnabled(By.CssSelector(settingsPopupMessageCssSelector));
                infoPopup.Click();
            }
            Response.Log = "Wait until notification message disappears";
            _driver.WaitForElementToBecomeStaleOrUnavailable(By.CssSelector(settingsPopupMessageCssSelector), MAX_WAIT_FOR_ELEMENT_TO_BECOME_UNAVAILABLE);
            return result;
        }

        protected void DialogActions(string buttonId, string settingsDialogWindowId, string settingsDialogWindowTitleId)
        {
            IWebElement dialogBox = _driver.FindElementNullable(By.Id(settingsDialogWindowId));
            if (dialogBox != null)
            {
                Response.Log = "Dialog window opened.";
                IWebElement dialogBoxTitle = _wait.GetElementIfVisibleAndEnabled(By.Id(settingsDialogWindowTitleId));
                Response.Log = $"\"{dialogBoxTitle.Text}\" dialog window is open. Clicking \"{buttonId}\"";
                _wait.GetElementIfVisibleAndEnabled(By.Id(buttonId)).Click();
                _driver.WaitForElementToBecomeStaleOrUnavailable(By.Id(settingsDialogWindowId), MAX_WAIT_FOR_ELEMENT_TO_BECOME_UNAVAILABLE);
            }
        }

        protected void navigateToGoogleAndTypeHelloWorld(string settingsHomepageLoadClass)
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(_homeUrl);

            Response.Log = "Getting the Web Page Heading of Google.com";
            if (!_shortWait.IsWebElementFound(By.CssSelector(settingsHomepageLoadClass)))
            {
                // Interact with the web page
                IWebElement searchInput = _driver.FindElement(By.Name("q"));
                searchInput.SendKeys("Hello World");
                searchInput.Submit();
            }            
            if (_additionalPause.Active) { Thread.Sleep(_additionalPause.SleepTimeInMilliseconds); }
        }       
        #endregion

        #region Abstract Methods
        protected abstract void RunTest(string _fileDirectory);
        #endregion

        private void SendKeysSlowly(string text)
        {
            foreach (char s in text)
            {
                SendKeys.SendWait(s.ToString()); // Choose the appropriate send routine
                System.Threading.Thread.Sleep(50); // Milliseconds, adjust as needed
            }
        }
    }
}


