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
            if (!CheckRole(out var failed)) return failed!;
            if (camId == DEFAULT_ID)
            {
                CampaignsDb.AddCampaign(camName, startDate.AsKyivTimeZone(), endDate.AsKyivTimeZone());
            }
            else
            {
                if (!CheckCanModify(camId, out var fault)) return fault!;
                CampaignsDb.UpdateCampaign(camId, camName, startDate.AsKyivTimeZone(), endDate.AsKyivTimeZone());
            }
            return BackToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!CheckRole(out var failed)) return failed!;
            if (!CheckCanModify(id, out var fault)) return fault!;
            CampaignsDb.DeleteCampaign(id);
            return BackToList();
        }

        private bool CheckCanModify(int id, out IActionResult? fault)
        {
            var c = CampaignsDb.GetCampaignById(id);
            if (!(c.State == Campaign.CampaignState.Future || LoginData?.AccessLevel == Login.AccessLevelCode.Admin))
            {
                fault = BadRequest("Unable to edit campaign, state = " + c.UfState);
                return false;
            }
            fault = null;
            return true;
        }
    }
}
