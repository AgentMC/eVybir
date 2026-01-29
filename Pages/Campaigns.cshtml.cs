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

        public async Task<IActionResult> OnPost(int camId, string camName, DateTime startDate, DateTime endDate)
        {
            if (!CheckRole(out var failed)) return failed!;
            if (camId == DEFAULT_ID)
            {
                await CampaignsDb.AddCampaign(camName, startDate.AsKyivTimeZone(), endDate.AsKyivTimeZone());
            }
            else
            {
                if (!CheckCanModify(await CampaignsDb.GetCampaignById(camId), out var fault)) return fault!;
                await CampaignsDb.UpdateCampaign(camId, camName, startDate.AsKyivTimeZone(), endDate.AsKyivTimeZone());
            }
            return BackToList();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            if (!CheckRole(out var failed)) return failed!;
            if (!CheckCanModify(await CampaignsDb.GetCampaignById(id), out var fault)) return fault!;
            await CampaignsDb.DeleteCampaign(id);
            return BackToList();
        }

        private bool CheckCanModify(Campaign c, out IActionResult? fault)
        {
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
