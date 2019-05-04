namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net.Http;

    using Dapper;

    public class AuthenticationService
    {
        private readonly ProfileRepo _profileRepo = new ProfileRepo();

        private readonly SHA256Adapter _sha256Adapter = new SHA256Adapter();

        private readonly OtpService _otpService = new OtpService();

        private readonly FailedCounter _failedCounter = new FailedCounter();

        private readonly NLogAdapter _nLogAdapter = new NLogAdapter();

        private readonly SlackAdapter _slackAdapter = new SlackAdapter();

        public bool Verify(string accountId, string password, string otp)
        {
            CheckAccountIsLocked(accountId);

            var passwordFromDB = _profileRepo.GetPasswordFromDB(accountId);

            var hashPassword = _sha256Adapter.GetHashPassword(password);

            var CurrentOTP = _otpService.GetCurrentOtp(accountId);

            if (passwordFromDB == hashPassword && CurrentOTP == otp)
            {
                ResetFailedCounter(accountId);
                return true;
            }
            else
            {
                _failedCounter.AddFailedCount(accountId);

                var failedCountResponse = FailedCount(accountId);

                _nLogAdapter.LogFailedCount(accountId, failedCountResponse);

                _slackAdapter.NotifyAuthFailed();

                return false;
            }
        }

        private static HttpResponseMessage FailedCount(string accountId)
        {
            var failedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
            failedCountResponse.EnsureSuccessStatusCode();
            return failedCountResponse;
        }

        private static void ResetFailedCounter(string accountId)
        {
            var resetResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/ReSet", accountId).Result;
            resetResponse.EnsureSuccessStatusCode();
        }

        private static void CheckAccountIsLocked(string accountId)
        {
            var isLockedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();

            if (isLockedResponse.Content.ReadAsAsync<bool>().Result)
            {
                throw new FailedTooManyTimesException();
            }
        }
    }

    public class FailedTooManyTimesException : Exception
    {
        public FailedTooManyTimesException()
        {
        }
    }
}