using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;


namespace ModuleTester.Services
{
    public struct InfoEventArgs
    {
        public ELogLevel ELogLevel;
        public string Text;
    }

    public enum ELogLevel
    {
        DEBUG,
        ERROR,
        FATAL,
        INFO,
        WARN
    }

    public delegate void LogInfoEvent(object Sender, InfoEventArgs e);

    public static class Logger
    {
        static public event LogInfoEvent LogInfoEventNotification;

        #region Constants

        #endregion

        private static readonly ILog _logger = LogManager.GetLogger(typeof(Logger));

        public static void Init(string filePrefix)
        {
            string localPath = AppData.LogSettings.LocalPath;
            string log4netConfigFileLocation = Path.Combine(localPath, App.StaticConfig.GetSection("Log4Net")["RelativeConfigFile"]);
            string logPath = Path.Combine(localPath, String.Format(App.StaticConfig.GetSection("Log4Net")["RelativeOutputFilePath"], filePrefix));
            if (!File.Exists(log4netConfigFileLocation))
            {
                log4netConfigFileLocation = App.StaticConfig.GetSection("Log4Net")["ConfigFile"];
            }

            XmlDocument log4netConfig = new XmlDocument();

            log4netConfig.Load(File.OpenRead(log4netConfigFileLocation));

            XmlNode node = log4netConfig.SelectSingleNode("//param[@name='File']");
            node.Attributes["value"].Value = logPath;

            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));

            XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        public static void WriteLog(ELogLevel logLevel, String log)
        {

            if (logLevel.Equals(ELogLevel.DEBUG))
            {
                _logger.Debug(log);
            }
            else if (logLevel.Equals(ELogLevel.ERROR))
            {
                _logger.Error(log);
            }
            else if (logLevel.Equals(ELogLevel.FATAL))
            {
                _logger.Fatal(log);
            }
            else if (logLevel.Equals(ELogLevel.INFO))
            {
                _logger.Info(log);
            }
            else if (logLevel.Equals(ELogLevel.WARN))
            {
                _logger.Warn(log);
            }


            //when something gets written to the log file, raise an event to display info on the screen
            if (LogInfoEventNotification != null)
            {
                InfoEventArgs eventArgs = new InfoEventArgs();
                eventArgs.ELogLevel = logLevel;
                eventArgs.Text = log;

                //raise the event
                LogInfoEventNotification(_logger, eventArgs);
            }
        }

        public static void WriteLog(String message, Exception exception)
        {
            _logger.Error(message, exception);

            //when something gets written to the log file, raise an event to display info on the screen
            if (LogInfoEventNotification != null)
            {
                InfoEventArgs eventArgs = new InfoEventArgs();
                eventArgs.ELogLevel = ELogLevel.ERROR;
                eventArgs.Text = message + exception;

                LogInfoEventNotification(_logger, eventArgs);

            }
        }


    }
}
