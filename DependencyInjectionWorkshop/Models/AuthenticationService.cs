namespace DependencyInjectionWorkshop.Models
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    using Dapper;

    using SlackAPI;

    public class AuthenticationService
    {
        public bool Verify(string accountId, string password, string otp)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };

            CheckAccountIsLocked(accountId, httpClient);

            var passwordFromDB = GetPasswordFromDB(accountId);

            var hashPassword = GetHashPassword(password);

            var CurrentOTP = GetCurrentOtp(accountId, httpClient);

            if (passwordFromDB == hashPassword && CurrentOTP == otp)
            {
                ResetFailedCounter(accountId, httpClient);
                return true;
            }
            else
            {
                AddFailedCount(accountId, httpClient);

                var failedCountResponse = FailedCount(accountId, httpClient);

                LogFailedCount(accountId, failedCountResponse);

                NotifyAuthFailed();

                return false;
            }
        }

        private static void NotifyAuthFailed()
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(slackResponse => { }, "my channel", "my message", "my bot name");
        }

        private static void LogFailedCount(string accountId, HttpResponseMessage failedCountResponse)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"accountid:{accountId}, failedCount:{failedCountResponse.Content.ReadAsAsync<int>().Result}");
        }

        private static HttpResponseMessage FailedCount(string accountId, HttpClient httpClient)
        {
            var failedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
            failedCountResponse.EnsureSuccessStatusCode();
            return failedCountResponse;
        }

        private static HttpResponseMessage AddFailedCount(string accountId, HttpClient httpClient)
        {
            var addFailedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
            return addFailedCountResponse;
        }

        private static void ResetFailedCounter(string accountId, HttpClient httpClient)
        {
            var resetResponse = httpClient.PostAsJsonAsync("api/failedCounter/ReSet", accountId).Result;
            resetResponse.EnsureSuccessStatusCode();
        }

        private static string GetCurrentOtp(string accountId, HttpClient httpClient)
        {
            string CurrentOTP;
            var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
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

        private static string GetHashPassword(string password)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }

        private static string GetPasswordFromDB(string accountId)
        {
            using (var connection = new SqlConnection("my connection string"))
            {
                return connection.Query<string>(
                    "spGetUserPassword",
                    new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
        }

        private static void CheckAccountIsLocked(string accountId, HttpClient httpClient)
        {
            var isLockedResponse = httpClient.PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;
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