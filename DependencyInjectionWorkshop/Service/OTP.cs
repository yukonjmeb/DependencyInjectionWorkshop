namespace DependencyInjectionWorkshop.Service
{
    using System;
    using System.Net.Http;

    public interface IOtp
    {
        string GetCurrentOtp(string accountId);
    }

    public class Otp : IOtp
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