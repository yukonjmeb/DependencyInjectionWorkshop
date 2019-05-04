namespace DependencyInjectionWorkshop.Models
{
    using System.Net.Http;

    public interface ILogger
    {
        void Info(string accountId, HttpResponseMessage httpResponseMessage);
    }

    public class Logger : ILogger
    {
        public void Info(string accountId, HttpResponseMessage httpResponseMessage)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"accountid:{accountId}, failedCount:{httpResponseMessage.Content.ReadAsAsync<int>().Result}");
        }
    }
}