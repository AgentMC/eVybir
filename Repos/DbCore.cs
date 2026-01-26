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

        public static SqlConnection OpenConnection()
        {
            var conn = new SqlConnection { ConnectionString = AppCfg.ConnectionString };
            conn.Open();
            return conn;
        }
    }   
}
