using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class CampaignsModel : LoginAwareCrudPage
    {
        public override Type PageType => typeof(Pages_Campaigns);

        public override string Title => "Campaigns";

        private static readonly TimeZoneInfo UkraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        private static DateTimeOffset ToKyiv(DateTime dt) => new(dt, UkraineTimeZone.GetUtcOffset(dt));

        public IActionResult OnPost(int camId, string camName, DateTime startDate, DateTime endDate)
        {
            if (camId == DEFAULT_ID)
            {
                CampaignsDb.AddCampaign(camName, ToKyiv(startDate), ToKyiv(endDate));
            }
            else
            {
                CampaignsDb.UpdateCampaign(camId, camName, ToKyiv(startDate), ToKyiv(endDate));
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
