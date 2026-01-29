using eVybir.Infra;
using Microsoft.Data.SqlClient;

namespace eVybir.Repos
{
    public class DbCore
    {
        public const string TTickets =      "Tickets", 
                            TCampaigns =    "Campaigns", 
                            TCCandidates =  "CampaignCandidates", 
                            TCandidates =   "Candidates",
                            TPermissions =  "Permissions",
                            TVotes =        "Votes";

        public static async Task<SqlConnection> OpenConnection()
        {
            var conn = new SqlConnection { ConnectionString = AppCfg.ConnectionString };
            await conn.OpenAsync();
            return conn;
        }
    }   
}
