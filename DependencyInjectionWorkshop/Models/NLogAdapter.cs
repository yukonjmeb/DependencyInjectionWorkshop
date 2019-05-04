namespace DependencyInjectionWorkshop.Models
{
    using System.Net.Http;

    public class NLogAdapter
    {
        public void LogFailedCount(string accountId, HttpResponseMessage failedCountResponse)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"accountid:{accountId}, failedCount:{failedCountResponse.Content.ReadAsAsync<int>().Result}");
        }
    }
}