using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class ReportModel : LoginAwarePageBase
    {
        public override string Title => "Виборчі звіти";

        public Dictionary<int, Campaign> Campaigns { get; private set; }

        public int? CurrentCampaignId { get; private set; }

        public bool CampaignIsActive { get; private set; }

        public TicketStats TicketStats { get; private set; }

        public List<DbWrapped<int, Candidate>> VoteStats { get; private set; }

        public List<GroupMembersStats>? GroupMembersStats { get; set; }

        public int MaxVote { get; set; }

        public async Task <IActionResult> OnGet(int? id)
        {
            if (!id.HasValue && !Request.Path.Value!.EndsWith('/'))
                return Redirect(Location<Pages_Report>() + "/");
            Campaigns = await CampaignsDb.GetPastOrCurrentCampaigns().ToDictionaryAsync(db => db.Key, db => db.Entity);
            CurrentCampaignId = id;
            if (id.HasValue)
            {
                if (!Campaigns.TryGetValue(id.Value, out Campaign? thisCampaign))
                    return NotFound();
                CampaignIsActive = thisCampaign.End > DateTime.UtcNow; //.state == Ongoing does 2 datetime comparisons, this line does it in 1
                TicketStats = await ReportDb.GetTicketStats(id.Value);
                VoteStats = await ReportDb.GetVoteStats(id.Value).ToListAsync();
                TicketStats.VotesCast = VoteStats.Sum(v => v.Key);
                MaxVote = VoteStats.Count > 0 ? VoteStats.Max(v => v.Key) : 0;
                if (await ReportDb.IsCampaignIncludingGroupMembers(id.Value))
                {
                    GroupMembersStats = await ReportDb.GetGroupMemberStats(id.Value).ToListAsync();
                }
            }
            return Page();
        }
    }
}
