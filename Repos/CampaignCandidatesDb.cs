using eVybir.Infra;
using System.Text;

namespace eVybir.Repos
{
    public static class CampaignCandidatesDb
    {
        const string Table = "CampaignCandidates";
        public static IEnumerable<Participant> GetParticipantsByCampaignFlat(int campaignId)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", campaignId);
            cmd.CommandText = $"select Id, CandidateId, GroupId, DisplayOrder from {Table} where CampaignId = @{pId}";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new((int)reader[1], reader.As<int?>(2), (int)reader[3]);
            }
        }

        public static void UpdateCampaignData(int campaignId, IList<Participant> participants)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", campaignId);
            StringBuilder command = new($"delete from {Table} where CampaignId=@{pId};\r\n");
            if (participants.Count > 0)
            {
                command.AppendLine($"insert into {Table} (CampaignId, CandidateId, GroupId, DisplayOrder) values");
                for (int i = 0; i < participants.Count; i++)
                {
                    var participant = participants[i];
                    var pCandId = cmd.AddParameter("candId" + i, participant.CandidateId);
                    var pGrpId = cmd.AddParameterNullable("grpId" + i, participant.GroupId);
                    var pDisp = cmd.AddParameter("dispOrd" + i, participant.DisplayOrder);
                    command.AppendLine($"(@{pId}, @{pCandId}, @{pGrpId}, @{pDisp}),");
                }
            }
            while (command[^1] != ')') command.Length--;
            cmd.CommandText = command.ToString();
            cmd.ExecuteNonQuery();
        }
    }
}
