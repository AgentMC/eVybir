using eVybir.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class IndexModel : LoginAwarePageBase
    {
        public override string Title => "Home page";

        public async Task<IActionResult> OnPostLogin(string userId)
        {
            await LoginUser(userId);
            return RedirectToPage(Location<Pages_Index>());
        }
    }
}
