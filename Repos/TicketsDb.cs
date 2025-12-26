using eVybir.Infra;

namespace eVybir.Repos
{
    public static class TicketsDb
    {
        const string TTickets = "Tickets", TCampaigns = "Campaigns";

        public static IEnumerable<CampaignState> GetUserCampaignsStatus(int userId, bool onlyActive)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pUserId = cmd.AddParameter("userId", userId);
            cmd.CommandText = $@"
--     0     1     2       3          4              5                6
select t.Id, c.Id, c.Name, c.EndTime, t.CreatedDate, t.CommittedDate, t.IsOffline
from {TCampaigns} c
    left join {TTickets} t
        on c.Id = t.CampaignId
where c.{(onlyActive? "EndTime" : "StartTime")} <= '{DateTime.Now:O}'
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
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pUserId = cmd.AddParameter("userId", userId);
            var pCampaignId = cmd.AddParameter("campaignId", campaignId);
            var pOffline = cmd.AddParameter("isOffline", isOffline);
            cmd.CommandText = $@"
insert into {TTickets} (Id,                        UserId,     CampaignId,     CreatedDate,               IsOffline) 
                values ('{Guid.CreateVersion7()}', @{pUserId}, @{pCampaignId}, '{DateTime.Now.ToKyiv()}', @{pOffline})";
            cmd.ExecuteNonQuery();
        }

        public static void CancelTicket(Guid ticketId)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pTicketId = cmd.AddParameter("tId", ticketId);
            cmd.CommandText = $"delete from {TTickets} where and Id = @{pTicketId}";
            cmd.ExecuteNonQuery();
        }

        public static void Commit(Guid ticketId)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pTicketId = cmd.AddParameter("ticketId", ticketId);
            cmd.CommandText = $"update {TTickets} set CommittedDate='{DateTime.Now.ToKyiv()}' where Id=@{pTicketId}";
            cmd.ExecuteNonQuery();
        }
    }
}
