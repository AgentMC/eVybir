using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class CampaignsModel : LoginAwareCrudPage
    {
        public override Type PageType => typeof(Pages_Campaigns);

        public override string Title => "Campaigns";

        public IActionResult OnPost(int camId, string camName, DateTime startDate, DateTime endDate)
        {
            if (camId == DEFAULT_ID)
            {
                CampaignsDb.AddCampaign(camName, startDate.AsKyivTimeZone(), endDate.AsKyivTimeZone());
            }
            else
            {
                CampaignsDb.UpdateCampaign(camId, camName, startDate.AsKyivTimeZone(), endDate.AsKyivTimeZone());
            }
            return BackToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            CampaignsDb.DeleteCampaign(id);
            return BackToList();
        }
    }
}
