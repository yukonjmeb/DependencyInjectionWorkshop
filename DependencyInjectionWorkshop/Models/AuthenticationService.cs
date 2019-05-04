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

            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
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
                return true;
            }
            else
            {
                var slackClient = new SlackClient("my api token");
                slackClient.PostMessage(slackResponse => { }, "my channel", "my message", "my bot name");

                return false;
            }
        }
    }
}