namespace HallOfFame
{
    using System;
    using System.IO;
    using System.Reflection;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    /// <summary>
    /// Класс для логирования.
    /// </summary>
    public static class FileLogger
    {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly Logger Instance;

        /// <summary>
        /// Конфигурирует логгер.
        /// Директория хранения логов: текущая_директория\logs.
        /// По умолчанию сообщения Debug не логгируются.
        /// </summary>
        static FileLogger()
        {
            var curDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName;
            var fileTarget = new FileTarget
            {
                FileName = curDir + "/logs/${shortdate}.txt",
                Layout = "${time} | ${callsite} | ${level:uppercase=true}${newline}${message}${newline}"
            };

            var loggingRule = new LoggingRule("*", LogLevel.Debug, fileTarget);

            var config = new LoggingConfiguration();
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(loggingRule);

            LogManager.Configuration = config;

            Instance = LogManager.GetLogger("TAP");
        }

        /// <summary>
        /// Отладочное сообщение.
        /// </summary>
        /// <param name="message">Текст сообщения с возможностью подставить агрументы вида {0}.</param>
        /// <param name="args">Аргументы.</param>
        public static void Debug(string message, params object[] args)
        {
            Instance.Debug(message, args);
        }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        public static void Error(Exception exception)
        {
            Instance.Error(exception);
        }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="exception">Исключение.</param>
        public static void Error(string message, Exception exception = null)
        {
            if (exception != null)
                Instance.Error(exception);
            else
                Instance.Error(message);
        }

        /// <summary>
        /// Позволяет логгеру записывать сообщения уровня Debug.
        /// </summary>
        public static void SetDebugLevel()
        {
            foreach (var loggingRule in LogManager.Configuration.LoggingRules)
            {
                loggingRule.EnableLoggingForLevel(LogLevel.Debug);
            }

            LogManager.ReconfigExistingLoggers();
        }

        /// <summary>
        /// Предупреждение.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        public static void Warn(string message)
        {
            Instance.Warn(message);
        }
    }
}