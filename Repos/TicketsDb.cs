using eVybir.Infra;
using System.Text;

namespace eVybir.Repos
{
    public class TicketsDb : DbCore
    {
        public static IEnumerable<CampaignState> GetUserCampaignsStatus(int userId, bool onlyActive)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pUserId = cmd.AddParameter("userId", userId);
            var pNow = cmd.AddParameter("dtNow", DateTime.Now);
            cmd.CommandText = $@"
--     0     1     2       3          4              5                6
select t.Id, c.Id, c.Name, c.EndTime, t.CreatedDate, t.CommittedDate, t.IsOffline
from {TCampaigns} c
    left join {TTickets} t
        on c.Id = t.CampaignId
        and t.UserId = @{pUserId}
where c.StartTime <= @{pNow} 
    {(onlyActive? $"and c.EndTime > @{pNow}" : "")}
    and (t.UserId = @{pUserId} or t.UserId is null)
order by c.StartTime";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var ticketId = reader.As<Guid?>(0);
                yield return new((int)reader[1], 
                                 (string)reader[2], 
                                 ((DateTimeOffset)reader[3]) < DateTime.Now, 
                                 ticketId.HasValue ? new(ticketId.Value, 
                                                         (DateTimeOffset)reader[4], 
                                                         reader.As<DateTimeOffset?>(5), 
                                                         (bool)reader[6]) 
                                                   : null);
            }
        }

        public static void Register(int campaignId, int userId, bool isOffline)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pUserId = cmd.AddParameter("userId", userId);
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            var pOffline = cmd.AddParameter("isOffline", isOffline);
            var pNow = cmd.AddParameter("dtNow", DateTime.UtcNow.AsKyivTimeZone());
            cmd.CommandText = $@"
insert into {TTickets} (Id,                        UserId,     CampaignId,     CreatedDate, IsOffline) 
                values ('{Guid.CreateVersion7()}', @{pUserId}, @{pCampaignId}, @{pNow},     @{pOffline})";
            cmd.ExecuteNonQuery();
        }

        public static void CancelTicket(Guid ticketId)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pTicketId = cmd.AddParameter("tId", ticketId);
            cmd.CommandText = $"delete from {TTickets} where Id=@{pTicketId}";
            cmd.ExecuteNonQuery();
        }

        public static IEnumerable<BulletinEntry> GetBulletinByTicket(Guid ticketId)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pTicketId = cmd.AddParameter("ticketId", ticketId);
            cmd.CommandText = $@"
--     0      1               2?          3                4       5?      6?             7            8
select cc.Id, cc.CandidateId, cc.GroupId, cc.DisplayOrder, c.Name, c.Date, c.Description, c.EntryType, cc.CampaignId
from {TCCandidates} cc
	join {TCandidates} c
		on cc.CandidateId = c.Id
		and cc.CampaignId = (select CampaignId from {TTickets} where Id = @{pTicketId})
order by DisplayOrder";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new((int)reader[0],
                                 new(
                                     (int)reader[1],
                                     reader.As<int?>(2),
                                     (int)reader[3]),
                                 new(
                                     (string)reader[4],
                                     reader.As<DateTime?>(5),
                                     reader.As<string?>(6),
                                     (Candidate.EntryType)reader[7]),
                                 (int)reader[8]);
            }
        }

        public static void Vote(Guid ticketId, int campaignId, int[] voteIds)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            StringBuilder command = new("SET XACT_ABORT ON;\r\nBEGIN TRANSACTION;\r\n");

            command.AppendLine($"insert into {TVotes} (Id, CampaignId, PreferenceIdx, CampaignCandidateId) values");
            for (int i = 0; i < voteIds.Length; i++)
            {
                var pVoteId = cmd.AddParameter("voteId" + i, Guid.CreateVersion7());
                var pPrefIdx = cmd.AddParameter("prefId" + i, i);
                var pParticipant = cmd.AddParameter("partId" + i, voteIds[i]);
                command.Append($"(@{pVoteId}, @{pCampaignId}, @{pPrefIdx}, @{pParticipant})");
                command.AppendLine(i < voteIds.Length - 1? "," : ";");
            }

            var pTicketId = cmd.AddParameter("ticketId", ticketId);
            var pNow = cmd.AddParameter("now", DateTime.UtcNow.AsKyivTimeZone());
            command.AppendLine($"update {TTickets} set CommittedDate=@{pNow} where Id=@{pTicketId};");

            command.AppendLine("COMMIT TRANSACTION;");

            cmd.CommandText = command.ToString();
            cmd.ExecuteNonQuery();
        }
    }
}
