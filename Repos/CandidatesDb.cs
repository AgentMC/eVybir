using eVybir.Infra;

namespace eVybir.Repos
{
    public class CandidatesDb : DbCore
    {

        public static async IAsyncEnumerable<DbWrapped<int, Candidate>> GetCandidates()
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"select Id, Name, Date, Description, EntryType from {TCandidates} order by Name";
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                yield return new((int)reader[0], new((string)reader[1], reader.As<DateTime?>(2), reader.As<string?>(3), (Candidate.EntryType)reader[4]));
            }
        }

        public static async Task<bool> GetCandidateHasPastUsesById(int id)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("candId", id);
            cmd.CommandText = $@"
select count(cc.Id)
from CampaignCandidates cc
	join Campaigns cs
		on cc.CampaignId = cs.Id
where cs.StartTime <= dateadd(day, 1, CURRENT_TIMESTAMP)
	and cc.CandidateId = @{pId}";
            return (int)(await cmd.ExecuteScalarAsync())! > 0;
        }

        public static async IAsyncEnumerable<DbWrapped<Tuple<int, int>, Candidate>> GetCandidatesWithUse()
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
select Id, Name, Date, Description, EntryType, count(Uid) as CampaignsUsed
from {TCandidates} c
	left join (
		select cc.Id as Uid, cc.CandidateId
		from {TCCandidates} cc
		    join {TCampaigns} cs
			    on cc.CampaignId = cs.Id
		where cs.StartTime <= dateadd(day, 1, CURRENT_TIMESTAMP)
	) t
	on Id = CandidateId
group by Id, Name, Date, Description, EntryType";
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                yield return new(Tuple.Create((int)reader[0], (int)reader[5]), new((string)reader[1], reader.As<DateTime?>(2), reader.As<string?>(3), (Candidate.EntryType)reader[4]));
            }
        }

        public static async Task AddCandidate(string name, DateTime? date, string? description, Candidate.EntryType entryKind)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pName = cmd.AddParameter("name", name);
            var pDate = cmd.AddParameterNullable("date", date);
            var pDescr = cmd.AddParameterNullable("descr", description);
            var pKind = cmd.AddParameter("kind", entryKind);
            cmd.CommandText = $"insert into {TCandidates} (Name, Date, Description, EntryType) values (@{pName}, @{pDate}, @{pDescr}, @{pKind})";
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateCandidate(int id, string name, DateTime? date, string? description, Candidate.EntryType entryKind)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            var pName = cmd.AddParameter("name", name);
            var pDate = cmd.AddParameterNullable("date", date);
            var pDescr = cmd.AddParameterNullable("descr", description);
            var pKind = cmd.AddParameter("kind", entryKind);
            cmd.CommandText = $"update {TCandidates} set Name=@{pName}, Date=@{pDate}, Description=@{pDescr}, EntryType=@{pKind} where id=@{pId}";
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteCandidate(int id)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            cmd.CommandText = $"delete from {TCandidates} where id=@{pId}";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
