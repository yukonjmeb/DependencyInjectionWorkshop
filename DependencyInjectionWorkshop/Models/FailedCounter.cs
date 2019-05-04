namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Net.Http;

    using DependencyInjectionWorkshop.Exceptions;

    public class FailedCounter : IFailedCounter
    {
        public void Add(string accountId)
        {
            var addFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
        }

        public int Get(string accountId)
        {
            var failedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
            failedCountResponse.EnsureSuccessStatusCode();
            return failedCountResponse.Content.ReadAsAsync<int>().Result;
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