using eVybir.Infra;

namespace eVybir.Repos
{
    public class ReportDb : DbCore
    {
        public static async Task<TicketStats> GetTicketStats(int campaignId)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            cmd.CommandText = $@"
select IsOffline, count(Id) as TicketsTotal, count(CommittedDate) as TicketsCommitted
from {TTickets}
where CampaignId = @{pCampaignId}
group by IsOffline";
            int offline = 0, online = 0, onlineCommitted = 0;
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                if ((bool)reader[0]) //offline
                {
                    offline = (int)reader[1];
                    //we cannot track offline tickets committed
                }
                else //online
                {
                    online = (int)reader[1];
                    onlineCommitted = (int)reader[2];
                }
            }
            return new(offline, online, onlineCommitted);
        }

        public static async IAsyncEnumerable<DbWrapped<int, Candidate>> GetVoteStats(int campaignId)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            cmd.CommandText = $@"
select count(*) as Count, c3.Name, c3.Date
from (
	select coalesce(c2.Id, c.Id) as Id
	from {TVotes} v
		join {TCCandidates} cc
			on v.CampaignCandidateId = cc.Id
			and v.CampaignId = @{pCampaignId}
		join {TCandidates} c
			on cc.CandidateId = c.Id
		left join {TCandidates} c2
			on cc.GroupId = c2.Id
) t
	join {TCandidates} c3
		on t.Id = c3.Id
group by t.Id, c3.Name, c3.Date
order by Count desc";
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                yield return new((int)reader[0], new((string)reader[1], reader.As<DateTime?>(2), null, 0));
            }
        }

        public static async Task<bool> IsCampaignIncludingGroupMembers(int campaignId)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            cmd.CommandText = $"select top 1 1 from {TCCandidates} where CampaignId = @{pCampaignId} and GroupId is not null";
            return await cmd.ExecuteScalarAsync() != null;
        }

        public static async IAsyncEnumerable<GroupMembersStats> GetGroupMemberStats(int campaignId)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            cmd.CommandText = $@"
select VotedName, VotedCandidateDate, GroupName, count(*) as Count
from (
	select c.Id, c.Name as VotedName, c.Date as VotedCandidateDate, c2.Name as GroupName
	from {TVotes} v
		join {TCCandidates} cc
			on v.CampaignCandidateId = cc.Id
			and v.CampaignId = @{pCampaignId}
			and cc.GroupId is not null
		join {TCandidates} c
			on cc.CandidateId = c.Id
		left join {TCandidates} c2
			on cc.GroupId = c2.Id
) t
group by Id, VotedName, VotedCandidateDate, GroupName
order by GroupName asc, Count desc";
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                yield return new((string)reader[0], reader.As<DateTime?>(1), (string)reader[2], (int)reader[3]);
            }
        }
    }
}
