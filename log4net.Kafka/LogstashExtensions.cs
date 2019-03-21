using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;

namespace log4net.Kafka
{
	internal static class LogstashExtensions
	{
		public static StringBuilder WriteString(this StringBuilder sb, string name, object value)
		{
			return sb.Append($"\"{name}\":").WriteString(value.ToString());
		}
		private static StringBuilder WriteString(this StringBuilder sb, string value)
		{
			sb.Append('\"');

			int runIndex = -1;
			int l = value.Length;
			for (var index = 0; index < l; ++index)
			{
				var c = value[index];

				if (c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\')// && c != ':' && c!=',')
				{
					if (runIndex == -1)
						runIndex = index;

					continue;
				}

				if (runIndex != -1)
				{
					sb.Append(value, runIndex, index - runIndex);
					runIndex = -1;
				}

				switch (c)
				{
					case '\t': sb.Append("\\t"); break;
					case '\r': sb.Append("\\r"); break;
					case '\n': sb.Append("\\n"); break;
					case '"':
					case '\\': sb.Append('\\'); sb.Append(c); break;
					default:
						sb.Append(c);
						break;
				}
			}

			if (runIndex != -1)
				sb.Append(value, runIndex, value.Length - runIndex);

			return sb.Append('\"');
		}
		public static StringBuilder WriteValueObject(this StringBuilder sb, string name, object value)
		{
			return sb.Append($"\"{name}\":{value}");
		}
		public static StringBuilder WriteMessage(this StringBuilder sb, LogstashEvent evt)
		{
			sb.WriteString(nameof(LogstashEvent.message), evt.message);

			if (evt.exception != null)
			{
				sb.Append(",")
				  .Append("\"exception\":{")
				  .WriteString(nameof(LogstashException.exception_class), evt.exception.exception_class).Append(",")
				  .WriteString(nameof(LogstashException.exception_message), evt.exception.exception_message).Append(",")
				  .WriteString(nameof(LogstashException.stacktrace), evt.exception.stacktrace)
				  .Append("}");
			}
			return sb;
		}
		public static string ToJson(this LogstashEvent evt)
		{
			var logstash = new StringBuilder();
			logstash.Append("{")
			.WriteValueObject("@version", evt.version).Append(",")
			.WriteString("@timestamp", evt.timestamp).Append(",")
			.WriteString(nameof(LogstashEvent.source_host), evt.source_host).Append(",")
			.WriteString(nameof(LogstashEvent.app), evt.app).Append(",")
			.WriteString(nameof(LogstashEvent.thread_name), evt.thread_name).Append(",")
			.WriteString(nameof(LogstashEvent.@class), evt.@class).Append(",")
			.WriteString(nameof(LogstashEvent.method), evt.method).Append(",")
			.WriteString(nameof(LogstashEvent.line_number), evt.line_number).Append(",")
			.WriteString(nameof(LogstashEvent.level), evt.level).Append(",")
			.WriteString(nameof(LogstashEvent.logger_name), evt.logger_name).Append(",")
			.WriteMessage(evt)
			.WritePropertyString(evt.propertyDictionary)
            .Append("}");
			return logstash.ToString();
		}
	    public static Dictionary<string,string>  GetPropertyDictionary(this LoggingEvent loggingEvent)
	    {
	        if (!(loggingEvent?.Properties?.Count > 0)) return null;

	        var propertyDictionary = new Dictionary<string, string>();
	        foreach (var key in loggingEvent.Properties.GetKeys().Where(k => !string.IsNullOrWhiteSpace(k)))
	        {
	            if (loggingEvent.Properties[key] == null) continue;
	            propertyDictionary.Add(key, loggingEvent.Properties[key].ToString());
            }
	        return propertyDictionary;

	    }

	    private static StringBuilder WritePropertyString(this StringBuilder sb, Dictionary<string, string> propertyDictionary)
	    {
	        if (propertyDictionary ==null || propertyDictionary.Count == 0) return sb;

	        foreach (var key in propertyDictionary.Keys.Where(k => !string.IsNullOrWhiteSpace(k)))
	        {
	            if (propertyDictionary[key] == null) continue;
	            sb.Append(",");
	            sb.WriteString(key, propertyDictionary[key]);

	        }

	        return sb;
	    }
    }
}
