namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Net.Http;

    using DependencyInjectionWorkshop.Exceptions;

    public class FailedCounter : IFailedCounter
    {
        public HttpResponseMessage Add(string accountId)
        {
            var addFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
            return addFailedCountResponse;
        }

        public HttpResponseMessage Get(string accountId)
        {
            var failedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
            failedCountResponse.EnsureSuccessStatusCode();
            return failedCountResponse;
        }

        public void Reset(string accountId)
        {
            var resetResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/ReSet", accountId).Result;
            resetResponse.EnsureSuccessStatusCode();
        }

        public void CheckAccountIsLocked(string accountId)
        {
            var isLockedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();

            if (isLockedResponse.Content.ReadAsAsync<bool>().Result)
            {
                throw new FailedTooManyTimesException();
            }
        }
    }
}