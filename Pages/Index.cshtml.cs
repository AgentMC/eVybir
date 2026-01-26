using eVybir.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class IndexModel : LoginAwarePageBase
    {
        public override string Title => "Home page";

        public IActionResult OnPostLogin(string userId)
        {
            LoginUser(userId);
            return RedirectToPage(Location<Pages_Index>());
        }
    }
}
