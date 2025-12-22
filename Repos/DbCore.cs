using Microsoft.Data.SqlClient;

namespace eVybir.Repos
{
    public static class DbCore
    {
        public static SqlConnection OpenConnection()
        {
            var conn = new SqlConnection
            {
                ConnectionString = @"Server=(local);Database=eVybir;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=eVybir;Command Timeout=100"
            };
            conn.Open();
            return conn;
        }
    }   
}
