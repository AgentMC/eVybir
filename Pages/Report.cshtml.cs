using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVybir.Pages
{
    public class ReportModel : LoginAwarePageBase
    {
        public override string Title => "Виборчі звіти";

        public ReportModel() 
        {
            Campaigns = CampaignsDb.GetPastOrCurrentCampaigns().ToDictionary(db => db.Key, db => db.Entity);
        }

        public Dictionary<int, Campaign> Campaigns { get; init; }

        public int? CurrentCampaignId { get; private set; }

        public bool CampaignIsActive { get; private set; }

        public TicketStats TicketStats { get; private set; }

        public DbWrapped<int, Candidate>[] VoteStats { get; private set; }

        public GroupMembersStats[]? GroupMembersStats { get; set; }

        public int MaxVote { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue && !Request.Path.Value!.EndsWith("/"))
                return Redirect(Location<Pages_Report>() + "/");
            CurrentCampaignId = id;
            if (id.HasValue)
            {
                if (!Campaigns.TryGetValue(id.Value, out Campaign? thisCampaign))
                    return NotFound();
                CampaignIsActive = thisCampaign.End > DateTime.UtcNow;
                TicketStats = ReportDb.GetTicketStats(id.Value);
                VoteStats = ReportDb.GetVoteStats(id.Value).ToArray();
                TicketStats.VotesCast = VoteStats.Sum(v => v.Key);
                MaxVote = VoteStats.Length > 0 ? VoteStats.Max(v => v.Key) : 0;
                if (ReportDb.IsCampaignIncludingGroupMembers(id.Value))
                {
                    GroupMembersStats = ReportDb.GetGroupMemberStats(id.Value).ToArray();
                }
            }
            return Page();
        }
    }
}
