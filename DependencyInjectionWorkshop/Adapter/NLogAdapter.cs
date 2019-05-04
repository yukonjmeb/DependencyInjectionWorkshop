﻿namespace DependencyInjectionWorkshop.Models
{
    using System.Net.Http;

    public interface ILogger
    {
        void Info(string message);
    }

    public class NLogAdapter : ILogger
    {
        public void Info(string message)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(message);
        }
    }
}