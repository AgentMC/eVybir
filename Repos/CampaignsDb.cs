using eVybir.Infra;
using DbCampaigns = System.Collections.Generic.IEnumerable<eVybir.Repos.DbWrapped<int, eVybir.Infra.Campaign>>;

namespace eVybir.Repos
{
    public class CampaignsDb : DbCore
    {
        private static DbCampaigns GetCampaigns(string filter = "")
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"select Id, Name, StartTime, EndTime from {TCampaigns} {filter} order by StartTime";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new((int)reader[0], new((string)reader[1], (DateTimeOffset)reader[2], (DateTimeOffset)reader[3]));
            }
        }
        
        public static DbCampaigns GetAllCampaigns() => GetCampaigns();

        public static DbCampaigns GetPastOrCurrentCampaigns() => GetCampaigns($"where StartTime < '{DateTime.Now:O}'");

        public static Campaign GetCampaignById(int campaignId) => GetCampaigns($"where Id={campaignId}").First().Entity;

        public static void AddCampaign(string name, DateTimeOffset start, DateTimeOffset end)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pName = cmd.AddParameter("name", name);
            var pDateStart = cmd.AddParameter("start", start);
            var pDateEnd = cmd.AddParameter("end", end);
            cmd.CommandText = $"insert into {TCampaigns} (Name, StartTime, EndTime) values (@{pName}, @{pDateStart}, @{pDateEnd})";
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCampaign(int id, string name, DateTimeOffset start, DateTimeOffset end)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            var pName = cmd.AddParameter("name", name);
            var pDateStart = cmd.AddParameter("start", start);
            var pDateEnd = cmd.AddParameter("end", end);
            cmd.CommandText = $"update {TCampaigns} set Name=@{pName}, StartTime=@{pDateStart}, EndTime=@{pDateEnd} where id=@{pId}";
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCampaign(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            cmd.CommandText = $"delete from {TCampaigns} where id=@{pId}";
            cmd.ExecuteNonQuery();
        }
    }
}
