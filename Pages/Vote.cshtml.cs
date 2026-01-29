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

        public async Task<IActionResult> OnGet(Guid id)
        {
            if (!CheckRole<Pages_Vote>(out var failed)) return failed!;
            var grp = await TicketsDb.GetBulletinByTicket(id)
                                     .GroupBy(b => b.Location.GroupId)
                                     .ToListAsync();
            CampaignId = grp[0].First().CampaignId;
            Bulletin = new(await CampaignsDb.GetCampaignById(CampaignId),
                           grp.Single(g => g.Key == null)
                              .Select(b => new BulletinHierarchy(b,
                                                                 grp.FirstOrDefault(g => g.Key == b.Location.CandidateId)?.ToArray() ?? []))
                              .ToArray());
            IsPartyListBasedElection = Bulletin.PrimaryEntries.Any(e => e.Children.Length > 0);
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid id, int campaignId, string castIds)
        {
            if (!CheckRole<Pages_Vote>(out var failed)) 
                return failed!;
            else if (await TicketsDb.Vote(id, campaignId, JsonSerializer.Deserialize<int[]>(castIds)!))
                return Redirect(Location<Pages_Register>());
            else
                return BadRequest("The vote was not cast, try again");
        }
    }
}
