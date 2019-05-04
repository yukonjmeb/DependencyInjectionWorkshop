namespace DependencyInjectionWorkshop.Repo
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Dapper;

    public interface IProfile
    {
        string GetPassword(string accountId);
    }

    public class Profile : IProfile
    {
        public string GetPassword(string accountId)
        {
            using (var connection = new SqlConnection("my connection string"))
            {
                return connection.Query<string>(
                    "spGetUserPassword",
                    new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
        }
    }
}