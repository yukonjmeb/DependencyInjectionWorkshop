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
            var logger = NLog.LogManager.GetCurrentClassLogger();

            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
            var isLockedResponse = httpClient.PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();

            if (isLockedResponse.Content.ReadAsAsync<bool>().Result)
            {
                throw new FailedTooManyTimesException();
            }

            var DBPassword = string.Empty;
            var CurrentOTP = string.Empty;

            using (var connection = new SqlConnection("my connection string"))
            {
                DBPassword = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            var HashPassword = hash.ToString();

            var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
            if (response.IsSuccessStatusCode)
            {
                CurrentOTP = response.Content.ReadAsAsync<string>().Result;
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            if (DBPassword == HashPassword && CurrentOTP == otp)
            {
                var resetResponse = httpClient.PostAsJsonAsync("api/failedCounter/ReSet", accountId).Result;
                resetResponse.EnsureSuccessStatusCode();
                return true;
            }
            else
            {
                var addFailedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
                addFailedCountResponse.EnsureSuccessStatusCode();

                var failedCountResponse =
                    httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
                addFailedCountResponse.EnsureSuccessStatusCode();

                logger.Info($"accountid:{accountId}, failedCount:{failedCountResponse.Content.ReadAsAsync<int>().Result}");

                var slackClient = new SlackClient("my api token");
                slackClient.PostMessage(slackResponse => { }, "my channel", "my message", "my bot name");

                return false;
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