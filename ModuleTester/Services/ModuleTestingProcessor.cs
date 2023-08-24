using System.Collections.Generic;
using System.IO;
using Common.Models;
using ModuleTester.Windows;
using System.Linq;
using Common.Cryptography;
using Common.Enumerators;
using OpenQA.Selenium.Chrome;
using System.ComponentModel;
using System.Windows;

namespace ModuleTester.Services
{
    public class ModuleTestingProcessor
    {
        #region Properties
        public ProcessResponse Response { get; set; }
        #endregion

        #region Fields
        private string _homeUrl = string.Empty;
        private string _driverPath = string.Empty;
        private string _fileDirectory = string.Empty;
        private int _waitTimeout;
        private int _fluentWait;
        private TimeSleepDetails _additionalPause;
        private AES _aes;
        private EnumLoginType _loginType;
        private LoginSettings _loginSettings;
        private ChromeOptions _chromeOptions;
        private string _serviceSettingsFileSubFolder = string.Empty;
        SpinnerWindow _spinner;
        Window _parentWindow;
        
        #endregion

        #region Constructors
        public ModuleTestingProcessor(string homeUrl, AES aes, EnumLoginType loginType, LoginSettings loginSettings,
            string driverPath, int waitTimeInSeconds, ChromeOptions chromeOptions, 
            TimeSleepDetails additionalPause, int fluentWaitInSeconds, string serviceSettingsFileSubFolder, Window parentWindow)
        {
            _homeUrl = homeUrl;
            _aes = aes;
            _loginType = loginType;
            _loginSettings = loginSettings;
            _driverPath = driverPath;
            _waitTimeout = waitTimeInSeconds;
            _chromeOptions = chromeOptions;
            _additionalPause = additionalPause;
            _fluentWait = fluentWaitInSeconds;
            _serviceSettingsFileSubFolder = serviceSettingsFileSubFolder;
            _parentWindow = parentWindow;
        }
        #endregion

        #region Public Methods
        public void Run(string moduleToTest)
        {
            Response = null;

            switch (moduleToTest)
            {
                case "Execute Random Automation Task":
                    ExecuteRandomAutomationTask();
                    break;               
                default:
                    break;
            }
         }
        #endregion

        #region Private Methods        

        private void ExecuteRandomAutomationTask()
        {
            ConfigurationLocator configurationExportLocator = new ConfigurationLocator();
            configurationExportLocator.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            configurationExportLocator.Owner = _parentWindow;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            
            worker.DoWork += worker_DoWork_ExecuteRandomAutomationTask;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            startSpinner();
        }                

        private void worker_DoWork_ExecuteRandomAutomationTask(object sender, DoWorkEventArgs e)
        {          
            ClaimProcessor.Services.ExecuteRandomAutomationTask ModuleToTest =
                new ClaimProcessor.Services.ExecuteRandomAutomationTask(_homeUrl, _aes, _loginType, _loginSettings, _driverPath, _waitTimeout,
                _chromeOptions, _additionalPause, _fluentWait, _serviceSettingsFileSubFolder);
            ModuleToTest.Run(_fileDirectory);
            e.Result = ModuleToTest.Response;
        }
        
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Response = (ProcessResponse)e.Result;
            _spinner.Close();
        }

        private void startSpinner()
        {
            _spinner = new SpinnerWindow();
            _spinner.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _spinner.Owner = _parentWindow;
            _spinner.ShowDialog();
        }
        #endregion
    }
}
