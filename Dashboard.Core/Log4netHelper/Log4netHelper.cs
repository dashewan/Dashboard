using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;
using System.IO;
using System.Data;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using System.Collections.Specialized;
using System.Configuration;

namespace Dashboard.Core.Log4netHelper
{
    public static class Log4netHelper
    {

        /// <summary>
        /// 使用SQLSERVER记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        public static void LoadADONetAppender()
        {
            Logger log = new Logger();
        }

        /// <summary>
        ///  异常处理
        /// </summary>
        /// <param name="methedType">出现异常方法的类型。例如：MethodBase.GetCurrentMethod().DeclaringType</param>
        /// <param name="errorMsg">错误信息。例如：SaveBuildingPhoto方法出错。Author:开发者名称</param>
        /// <param name="ex"></param>
        public static void InvokeErrorLog(Type methedType, string errorMsg, Exception ex)
        {
            LoadADONetAppender();
            ILog log = log4net.LogManager.GetLogger(methedType);
            log.Info(errorMsg, ex);
        }
    }


    public class Logger
    {
        private PatternLayout _layout = new PatternLayout();
        private const string LOG_PATTERN = "记录时间：%date 线程ID:[%thread] 日志级别：%-5level 出错类：%logger property:[%property{NDC}] - 错误描述：%message%newline";//"%d [%t] %-5p %m%n";
       
        public string DefaultPattern
        {
            get { return LOG_PATTERN; }
        }

        public Logger()
        {
            _layout.ConversionPattern = DefaultPattern;
            _layout.ActivateOptions();
        }

        public PatternLayout DefaultLayout
        {
            get { return _layout; }
        }

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy =
                    (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(appender);
        }

        static Logger()
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            string connectString = appSettings["LogConnectString"];

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string txtLogPath = string.Empty;
            string iisBinPath = AppDomain.CurrentDomain.RelativeSearchPath;
            if (!string.IsNullOrEmpty(iisBinPath))
                txtLogPath = Path.Combine(iisBinPath, @"Logs\ErrorLog.txt");
            else
                txtLogPath = Path.Combine(currentPath, @"Logs\ErrorLog.txt");

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LOG_PATTERN;
            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            RollingFileAppender roller = new RollingFileAppender();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "5MB";//"100KB";
            roller.Encoding = Encoding.UTF8;
            roller.StaticLogFileName = true;
            roller.File = txtLogPath;//"dnservices.txt";
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            /////////////////////////ADO.NET

            AdoNetAppender adoAppender = new AdoNetAppender();
            adoAppender.Name = "AdoNetAppender";
            adoAppender.CommandType = CommandType.Text;
            adoAppender.BufferSize = 1;
            adoAppender.ConnectionType = "System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            adoAppender.ConnectionString = connectString;
            adoAppender.CommandText = @"INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)";
            adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@log_date", DbType = System.Data.DbType.DateTime, Layout = new log4net.Layout.RawTimeStampLayout() });
            adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@thread", DbType = System.Data.DbType.String, Size = 255, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread")) });
            adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@log_level", DbType = System.Data.DbType.String, Size = 50, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level")) });
            adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@logger", DbType = System.Data.DbType.String, Size = 255, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger")) });
            adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@message", DbType = System.Data.DbType.String, Size = 4000, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message")) });
            adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@exception", DbType = System.Data.DbType.String, Size = 4000, Layout = new Layout2RawLayoutAdapter(new ExceptionLayout()) });
            adoAppender.ActivateOptions();
            hierarchy.Root.AddAppender(adoAppender);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        public static ILog Create()
        {
            return LogManager.GetLogger("dnservices");
        }
    }
}
