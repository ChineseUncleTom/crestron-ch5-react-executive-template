using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicConfig
{
    /// <summary>
    /// The various logging levels
    /// </summary>
    public enum LogLevel {
        DEBUG,
        NOTICE,
        WARN,
        ERROR
    }

    /// <summary>
    /// Helper class to make logging easier.
    /// </summary>
    public class LogHelper
    {
        private string Header;
        private LogLevel logLevelVisible;

        /// <summary>
        /// Logs a message to the log.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        public void Log(LogLevel level, string message)
        {
            if (logLevelVisible <= level)
            {

                Console.WriteLine(String.Format("[{0}] {1}: {2}", level.ToString(), Header, message));
            }

        }

        /// <summary>
        /// Logs a message to the log.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        public void Log(LogLevel level, string format, params object[] args)
        {
            this.Log(level, String.Format(format, args));
        }

        /// <summary>
        /// Logs a message to the log.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            this.Log(LogLevel.NOTICE, message);
        }

        /// <summary>
        /// Logs a message to the log.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string format, params object[] args)
        {
            this.Log(LogLevel.NOTICE, format, args);
        }

        /// <summary>
        /// Setups the logger.
        /// </summary>
        /// <param name="visible">The visible log level.</param>
        /// <param name="header">The header.</param>
        private void SetupLogger(LogLevel visible, string header)
        {
            this.Header = header;
            logLevelVisible = visible;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogHelper"/> class.
        /// </summary>
        /// <param name="visible">The visible log level.</param>
        /// <param name="header">The header.</param>
        public LogHelper(LogLevel visible, string header)
        {
            SetupLogger(visible, header);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogHelper"/> class.
        /// </summary>
        /// <param name="visible">The visible log level.</param>
        public LogHelper(LogLevel visible)
        {
            SetupLogger(visible, "Log");
        }

        /// <summary>
        /// Log a Debug level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            this.Log(LogLevel.DEBUG, message);
        }

        /// <summary>
        /// Log a Debug level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string format, params object[] args)
        {
            this.Log(LogLevel.DEBUG, format, args);
        }

        /// <summary>
        /// Log a Notice level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Notice(string message)
        {
            this.Log(LogLevel.NOTICE, message);
        }

        /// <summary>
        /// Log a Notice level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Notice(string format, params object[] args)
        {
            this.Log(LogLevel.NOTICE, format, args);
        }

        /// <summary>
        /// Log a Warn level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            this.Log(LogLevel.WARN, message);
        }

        /// <summary>
        /// Log a Warn level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string format, params object[] args)
        {
            this.Log(LogLevel.WARN, format, args);
        }

        /// <summary>
        /// Log a Error level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            this.Log(LogLevel.ERROR, message);
        }

        /// <summary>
        /// Log a Error level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string format, params object[] args)
        {
            this.Log(LogLevel.ERROR, format, args);
        }
    }
}