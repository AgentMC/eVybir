using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eVybir.Pages
{
    public class VoteModel : LoginAwarePageBase
    {
        public override string Title => "Віддати голос";

        public Bulletin Bulletin { get; set; }

        public bool IsPartyListBasedElection { get; set; } = false;

        public int CampaignId { get; set; }

        public void OnGet(Guid id)
        {
            var grp = TicketsDb.GetBulletinByTicket(id)
                               .GroupBy(b => b.Location.GroupId);
            CampaignId = grp.First().First().CampaignId;
            Bulletin = new(CampaignsDb.GetCampaignById(CampaignId),
                           grp.Single(g => g.Key == null)
                                     .Select(b => new BulletinHierarchy(b,
                                                                        grp.FirstOrDefault(g => g.Key == b.Location.CandidateId)?.ToArray() ?? []))
                                     .ToArray());
            IsPartyListBasedElection = Bulletin.PrimaryEntries.Any(e => e.Children.Length > 0);
        }

        public IActionResult OnPost(Guid id, int campaignId, string castIds)
        {
            TicketsDb.Vote(id, campaignId, JsonSerializer.Deserialize<int[]>(castIds)!);
            return Redirect(Location<Pages_Register>());
        }
    }
}
