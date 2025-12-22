using eVybir.Infra;

namespace eVybir.Repos
{
    public static class CampaignsDb
    {
        public static IEnumerable<DbWrapped<int, Campaign>> GetCampaigns()
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"select Id, Name, StartTime, EndTime from Campaigns";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new((int)reader[0], new((string)reader[1], (DateTimeOffset)reader[2], (DateTimeOffset)reader[3]));
            }
        }

        public static void AddCampaign(string name, DateTimeOffset start, DateTimeOffset end)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pName = cmd.AddParameter("name", name);
            var pDateStart = cmd.AddParameter("start", start);
            var pDateEnd = cmd.AddParameter("end", end);
            cmd.CommandText = $"insert into Campaigns (Name, StartTime, EndTime) values (@{pName}, @{pDateStart}, @{pDateEnd})";
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCampaign(int id, string name, DateTimeOffset start, DateTimeOffset end)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            var pName = cmd.AddParameter("name", name);
            var pDateStart = cmd.AddParameter("start", start);
            var pDateEnd = cmd.AddParameter("end", end);
            cmd.CommandText = $"update Campaigns set Name=@{pName}, StartTime=@{pDateStart}, EndTime=@{pDateEnd} where id=@{pId}";
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCampaign(int id)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            cmd.CommandText = $"delete from Campaigns where id=@{pId}";
            cmd.ExecuteNonQuery();
        }
    }
}
