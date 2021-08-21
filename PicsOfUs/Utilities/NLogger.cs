using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using NLog;

namespace PicsOfUs.Utilities
{
    public class NLogger : ILogger
    {
        private static NLogger _instance;
        private static Logger _logger;
        private const string RuleName = "LoggerRule";

        public static NLogger GetInstance()
        {
            if (_instance == null)
                _instance = new NLogger();
            
            return _instance;
        }

        public static Logger GetLogger(string theLogger)
        {
            if (_logger == null)
                _logger = LogManager.GetLogger(theLogger);

            return _logger;
        }

        public void Debug(string message, string arguments = null)
        {
            if (arguments == null)
                GetLogger(RuleName).Debug(message);
            else
                GetLogger(RuleName).Debug(message, arguments);
        }

        public void Info(string message, string arguments = null)
        {
            if (arguments == null)
                GetLogger(RuleName).Info(message);
            else
                GetLogger(RuleName).Info(message, arguments);
        }

        public void Warning(string message, string arguments = null)
        {
            if (arguments == null)
                GetLogger(RuleName).Warn(message);
            else
                GetLogger(RuleName).Warn(message, arguments);
        }

        public void Error(string message, string arguments = null)
        {
            if (arguments == null)
                GetLogger(RuleName).Error(message);
            else
                GetLogger(RuleName).Error(message, arguments);
        }
    }
}