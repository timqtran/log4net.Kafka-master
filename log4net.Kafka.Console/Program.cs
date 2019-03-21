using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace log4net.Kafka.Console
{
	class Program
	{
		static void Main(string[] args)
		{

			XmlConfigurator.ConfigureAndWatch(new FileInfo(@"log4net.config"));

			ILog logger = LogManager.GetLogger(typeof(Program));

			logger.Debug("this Debug msg");
			logger.Warn("this Warn msg");
			logger.Info("this Info msg");
			logger.Error("this Error msg");
			logger.Fatal("this Fatal msg");
		    var logger1 = LoggerManager.GetLogger(Assembly.GetCallingAssembly(),
		        System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            logger1.Log(CreateLoggingEvent(Level.Error, 1, "Test", "This Error Log has properties", null, "123456789"));

            try
			{
				var i = 0;
				var j = 5 / i;
			}
			catch (Exception ex)
			{
				logger.Error("this Error msg,中文测试", ex);
			}
			System.Console.ReadKey();
		}
	    private static LoggingEvent CreateLoggingEvent(Level level, int? eventID, string logName, object message, Exception exceptionObject, string sessionID)
	    {
	        LoggingEvent loggingEvent = new LoggingEvent(typeof(LogImpl), null, logName, level, message, exceptionObject);

	        if (eventID.HasValue)
	        {
	            if (eventID < 0 || eventID > 0xFFFF)
	            {

	                throw new ArgumentOutOfRangeException(
	                    "eventID", string.Format("An event id of value {0} is not supported. Valid event ids must be within a range of 0 and 65535.", eventID));
	            }

	            loggingEvent.Properties["EventID"] = eventID;
	        }

	        if (!string.IsNullOrWhiteSpace(sessionID))
	        {
	            loggingEvent.Properties["SessionID"] = sessionID;
	        }

	        return loggingEvent;
	    }

    }

}
