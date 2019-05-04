namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Net.Http;

    public class FailedCounter
    {
        public HttpResponseMessage AddFailedCount(string accountId)
        {
            var addFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
            return addFailedCountResponse;
        }
    }
}