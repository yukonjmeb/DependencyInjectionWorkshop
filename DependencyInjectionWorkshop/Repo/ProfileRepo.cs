namespace DependencyInjectionWorkshop.Repo
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Dapper;

    public class ProfileRepo
    {
        public string GetPasswordFromDB(string accountId)
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