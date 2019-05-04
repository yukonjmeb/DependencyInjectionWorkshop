﻿namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Net.Http;

    public class OtpService
    {
        public string GetCurrentOtp(string accountId)
        {
            string CurrentOTP;
            var response = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/otps", accountId).Result;
            if (response.IsSuccessStatusCode)
            {
                CurrentOTP = response.Content.ReadAsAsync<string>().Result;
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            return CurrentOTP;
        }
    }
}