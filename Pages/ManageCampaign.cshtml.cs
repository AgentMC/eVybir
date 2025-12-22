using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class ManageCampaignModel : LoginAwarePageBase
    {
        public ManageCampaignModel() 
        {
            Campaigns = CampaignsDb.GetFutureCampaigns();
        }

        public override string Title => "Призначити кандидатів";

        public bool ShowOnlyList { get; private set; } = true;

        public int? CurrentCampaignId { get; private set; }

        public Campaign? CurrentCampaign { get; private set; }

        public IEnumerable<DbWrapped<int, Campaign>> Campaigns { get; init; }

        public IActionResult OnGet(int? id)
        {
            if (id.HasValue)
            {
                CurrentCampaignId = id.Value;
                CurrentCampaign = Campaigns.FirstOrDefault(c => c.Key == CurrentCampaignId)?.Entity;
                if (CurrentCampaign == null) // wrong, race condition etc.
                {
                    return RedirectToPage(Location<Pages_ManageCampaign>());
                }
                ShowOnlyList = false;
                //TBD load page details for the campaigns
            }
            return Page();
        }
    }
}
