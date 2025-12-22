using eVybir.Infra;
using eVybir.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class IndexModel : LoginAwarePageBase
    {
        public override string Title => "Home page";

        public IActionResult OnPostLogin(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var identity = Login.LogIn(userId);
                if (identity != null)
                {
                    HttpContext.Response.Cookies.Append(Login.COOKIE, Login.Serialize(identity));
                }
            }
            else
            {
                HttpContext.Response.Cookies.Delete(Login.COOKIE);
            }
            return RedirectToPage(Location<Pages_Index>());
        }
    }
}
