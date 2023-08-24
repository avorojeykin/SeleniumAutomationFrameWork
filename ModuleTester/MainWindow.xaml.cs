using Microsoft.Extensions.Options;
using ModuleTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModuleTester.Services;
using System.IO;
using Common.Utilities;
using Common.Models;
using Common.Cryptography;
using Common.Enumerators;
using ModuleTester.Windows;
using OpenQA.Selenium.Chrome;

namespace ModuleTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private readonly AppSettings _appSettings;
        private LocalSettings _localSettings;
        private string _homeUrl = string.Empty;
        private string _driverPath = string.Empty;
        private int _waitTimeout;
        private int _fluentWait;
        private TimeSleepDetails _additionalPause;
        private string _cookiesPath = string.Empty;
        private AES _aes;
        private EnumLoginType _loginType;
        private LoginSettings _loginSettings;
        private ChromeOptions _chromeOptions;
        private string _commonSettingsFileSubFolder = string.Empty;
        private string _serviceSettingsFileSubFolder = string.Empty;
        #endregion

        #region Constants
        private const string LOCAL_SETTINGS_FILE_NAME = "localsettings.json";
        private const string DEFAULT_COMBOBOX_VALUE = "-- select --";
        private const string LOGIN_SSO_SETTINGS_FILE_NAME = "Login2FASettings.json";
        private const string LOGIN_USERNAME_PASSWORD_SETTINGS_FILE_NAME = "LoginUsernamePasswordSettings.json";
        private const string GMAIL_EMAIL_DOMAIN = "@gmail.com";
        #endregion

        public MainWindow(IOptions<AppSettings> appSettings)
        {
            this._appSettings = appSettings.Value;
            InitializeComponent();
            SetInitialValues();
        }

        private void SetInitialValues()
        {
            try
            {
                AppData.LogSettings.LocalPath = AppDomain.CurrentDomain.BaseDirectory;
                Logger.Init("ModuleTester");
                Logger.WriteLog(ELogLevel.INFO, "App Started");
                LoadLocalSettings();
                this._appSettings.ServiceUIUrls.Insert(0, DEFAULT_COMBOBOX_VALUE);
                this._appSettings.TestModules.Insert(0, DEFAULT_COMBOBOX_VALUE);
                UrlComboBox.ItemsSource = this._appSettings.ServiceUIUrls;
                ModuleToTestComboBox.ItemsSource = this._appSettings.TestModules;

            }
            catch (Exception ex)
            {
                DisplayErrorDetails(CommonUtilities.GetExceptionString(ref ex));
            }
        }

        private void LoadLocalSettings()
        {
            try
            {
                if (File.Exists(System.IO.Path.Combine(AppData.LogSettings.LocalPath, LOCAL_SETTINGS_FILE_NAME)))
                {
                    Logger.WriteLog(ELogLevel.INFO, $"Read data from \"{LOCAL_SETTINGS_FILE_NAME}\" file");
                    _localSettings = CommonUtilities.DeserializeJsontoObject<LocalSettings>(System.IO.Path.Combine(AppData.LogSettings.LocalPath, LOCAL_SETTINGS_FILE_NAME));
                    UsernameTextBox.Text = _localSettings.Username;                    
                }
                else
                {
                    _localSettings = new LocalSettings { UserEmail = string.Empty, Username = string.Empty};
                }
            }
            catch (Exception ex)
            {
                DisplayErrorDetails(CommonUtilities.GetExceptionString(ref ex));
            }
        }

        private void SaveLocalSettings()
        {
            try
            {
                if(_localSettings.UserEmail != UsernameTextBox.Text)
                {
                    _localSettings.Username = UsernameTextBox.Text;
                    string newLocalSettings = _localSettings.SerializeObjecttoJson();
                    if (File.Exists(System.IO.Path.Combine(AppData.LogSettings.LocalPath, LOCAL_SETTINGS_FILE_NAME)))
                    {
                        File.Delete(System.IO.Path.Combine(AppData.LogSettings.LocalPath, LOCAL_SETTINGS_FILE_NAME));
                    }
                    File.WriteAllText(System.IO.Path.Combine(AppData.LogSettings.LocalPath, LOCAL_SETTINGS_FILE_NAME),
                        newLocalSettings);
                }
            }
            catch (Exception ex)
            {
                DisplayErrorDetails(CommonUtilities.GetExceptionString(ref ex));
            }
        }

        private void DisplayErrorDetails(string error)
        {
            try
            {
                Logger.WriteLog(ELogLevel.ERROR, error);
                MessageBox.Show(error, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                ExitButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying error details: {CommonUtilities.GetExceptionString(ref ex)}");
            }
        }

        private void EnterPrerequisitesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_appSettings.DisplayLocalInfo)
                {
                    //To show internal data and exit without runnning actual test
                    DisplayLocalInfo();
                    return;
                }

                if (!ArePrerequisitesValid())
                {
                    MessageBox.Show("Not all required data was entered!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; 
                }
                SaveLocalSettings();
                _homeUrl = UrlComboBox.Text;
                _driverPath = this._appSettings.BrowserDriverFolder;
                _waitTimeout = this._appSettings.ExplicitWaitInSeconds;
                _fluentWait = this._appSettings.FluentWaitInSeconds;
                _additionalPause = new TimeSleepDetails { Active = this._appSettings.AdditionalPauseFlag, 
                    SleepTimeInMilliseconds = this._appSettings.AdditionalPauseInSeconds * 1000 };
                _cookiesPath = this._appSettings.CookiesPath; 
                _aes = new AES(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                _commonSettingsFileSubFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), this._appSettings.CommonSettingsSubFolder);
                _serviceSettingsFileSubFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), this._appSettings.ServiceSettingsSubFolder);
                SetLoginTypeLoginSettingsPassword();
                SetChromeOptions();
                RunTestButton.IsEnabled = true;
                DisablePrerequisites();
            }
            catch (Exception ex)
            {
                DisplayErrorDetails(CommonUtilities.GetExceptionString(ref ex));
            }
        }

        private void SetLoginTypeLoginSettingsPassword()
        {
            _loginType = (bool)UseSSOCheckBox.IsChecked ? EnumLoginType.SSO : EnumLoginType.UsernamePassword;
            if (_loginType == EnumLoginType.SSO)
            {
                _loginSettings = CommonUtilities.DeserializeJsontoObject<LoginSettings>(
                    System.IO.Path.Combine(_commonSettingsFileSubFolder, LOGIN_SSO_SETTINGS_FILE_NAME));
            }
            else
            {
                _loginSettings = CommonUtilities.DeserializeJsontoObject<LoginSettings>(
                    System.IO.Path.Combine(_commonSettingsFileSubFolder, LOGIN_USERNAME_PASSWORD_SETTINGS_FILE_NAME));
            }
            _loginSettings.Pwd = _aes.EncryptData(PasswordTextBox.Password);
            _loginSettings.User = _localSettings.Username;
            _loginSettings.UserGmailEmail = $"{_localSettings.UserEmail}{GMAIL_EMAIL_DOMAIN}";
        }

        private void SetChromeOptions()
        {
            _chromeOptions = new ChromeOptions();
            EnumMonitorSelection monitorSelection = DisplayLocationComboBox.Text.ToLower() == "left"
                ? EnumMonitorSelection.Left
                    : DisplayLocationComboBox.Text.ToLower() == "right"
                        ? EnumMonitorSelection.Right
                        : EnumMonitorSelection.None;
            _chromeOptions = WebDriverHelper.SetChromeOptions(monitorSelection, 
                (bool)UseCookiesCheckBox.IsChecked, 
                string.Format(this._appSettings.CookiesPath, 
                _localSettings.UserEmail));
        }

        private bool ArePrerequisitesValid()
        {
            bool result = true;
            if (UrlComboBox.Text == DEFAULT_COMBOBOX_VALUE)
            { result = false; }
            if(string.IsNullOrEmpty(UsernameTextBox.Text))
            { result = false; }
            return result;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RunTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModuleToTestComboBox.Text == DEFAULT_COMBOBOX_VALUE)
                {
                    MessageBox.Show("Select Module to test!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ModuleTestingProcessor moduleTestingProcessor = new ModuleTestingProcessor(_homeUrl, _aes, _loginType, _loginSettings, _driverPath,
                    _waitTimeout, _chromeOptions, _additionalPause, _fluentWait, _serviceSettingsFileSubFolder, this);

                WaitWhileProcessing(true);

                moduleTestingProcessor.Run(ModuleToTestComboBox.Text);
                if (moduleTestingProcessor.Response == null)
                {
                    WaitWhileProcessing(false);
                    return; 
                }
                Logger.WriteLog(ELogLevel.INFO, moduleTestingProcessor.Response.Log);
                if (!moduleTestingProcessor.Response.IsSuccess)
                {
                    WaitWhileProcessing(false);
                    DisplayErrorDetails(moduleTestingProcessor.Response.ErrorDescription + moduleTestingProcessor.Response.SystemError);
                }

                if (this._appSettings.ResultInEmail.SendNotification)
                {
                    new Task(() => { SendNotification((ProcessResponseEmail)moduleTestingProcessor.Response.ReturnedResponseObject); }).Start();
                }

                new Task(() => { WriteArchive((ProcessResponseEmail)moduleTestingProcessor.Response.ReturnedResponseObject); }).Start();

                ResultTextBox.Document.Blocks.Add(new Paragraph(new Run(moduleTestingProcessor.Response.ProcessDetails)));
                WaitWhileProcessing(false);
            }
            catch (Exception ex)
            {
                WaitWhileProcessing(false);
                DisplayErrorDetails(CommonUtilities.GetExceptionString(ref ex));
            }
        }

        private void CleanResultButton_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Document.Blocks.Clear();
        }

        private void DisablePrerequisites()
        {
            UrlComboBox.IsEnabled = false;
            UsernameTextBox.IsEnabled = false;
            PasswordTextBox.IsEnabled = false;
            DisplayLocationComboBox.IsEnabled = false;
            UseCookiesCheckBox.IsEnabled = false;
            UseSSOCheckBox.IsEnabled = false;
            EnterPrerequisitesButton.IsEnabled = false;
        }

        private void WaitWhileProcessing(bool wait)
        {
            if (wait)
            {
                RunTestButton.IsEnabled = false;
                ExitButton.IsEnabled = false;
                CleanResultButton.IsEnabled = false;
            }
            else
            {
                RunTestButton.IsEnabled = true;
                ExitButton.IsEnabled = true;
                CleanResultButton.IsEnabled = true;
            }
        }

        private void DisplayLocalInfo()
        {
            try
            {
                string localPath = AppData.LogSettings.LocalPath;
                string log4netConfigFileLocation = System.IO.Path.Combine(localPath, App.StaticConfig.GetSection("Log4Net")["RelativeConfigFile"]);
                string logPath = System.IO.Path.Combine(localPath, String.Format(App.StaticConfig.GetSection("Log4Net")["RelativeOutputFilePath"], "ModuleTester"));
                ResultTextBox.Document.Blocks.Add(new Paragraph(new Run(AppDomain.CurrentDomain.BaseDirectory)));
                ResultTextBox.Document.Blocks.Add(new Paragraph(new Run($"config: {log4netConfigFileLocation}")));
                ResultTextBox.Document.Blocks.Add(new Paragraph(new Run($"logPath: {logPath}")));
            }
            catch (Exception ex)
            {
                DisplayErrorDetails(CommonUtilities.GetExceptionString(ref ex));
            }
        }

        private void SendNotification(ProcessResponseEmail emailDetails)
        {
            Logger.WriteLog(ELogLevel.INFO, "Sending email notification");
            string emailStatus = EmailService.SendEmail(this._appSettings.ResultInEmail, _loginSettings.UserGmailEmail, emailDetails);
            if (emailStatus != "ok")
            {
                Logger.WriteLog(ELogLevel.WARN, emailStatus);
            }
        }

        private void WriteArchive(ProcessResponseEmail emailDetails)
        {
            string fileContent = $"{emailDetails.Subject}\r\n{emailDetails.Header}\r\n{emailDetails.Body}\r\n{emailDetails.Trailer}";
            Logger.WriteLog(ELogLevel.INFO, "Writing Archive file");
            string fileCreationStatus = FileService.WriteArchive(System.IO.Path.Combine(AppData.LogSettings.LocalPath, this._appSettings.ArchiveFile), fileContent);
            if (fileCreationStatus != "ok")
            {
                Logger.WriteLog(ELogLevel.WARN, fileCreationStatus);
            }
        }
    }
}
