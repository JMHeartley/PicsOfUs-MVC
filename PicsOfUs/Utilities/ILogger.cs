using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;

namespace PicsOfUs.Utilities
{
    public interface ILogger
    {
        void Debug(string message, string arguments = null);
        void Info(string message, string args = null);
        void Warning(string message, string args = null);
        void Error(string message, string args = null);
    }
}