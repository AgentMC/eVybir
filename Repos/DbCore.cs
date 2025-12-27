using Microsoft.Data.SqlClient;

namespace eVybir.Repos
{
    public class DbCore
    {
        public const string TTickets = "Tickets", 
                            TCampaigns = "Campaigns", 
                            TCCandidates = "CampaignCandidates", 
                            TCandidates = "Candidates",
                            TPermissions = "Permissions";

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
