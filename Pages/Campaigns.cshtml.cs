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
                CampaignsDb.AddCampaign(camName, startDate.ToKyiv(), endDate.ToKyiv());
            }
            else
            {
                CampaignsDb.UpdateCampaign(camId, camName, startDate.ToKyiv(), endDate.ToKyiv());
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
