using eVybir.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVybir.Pages
{
    public class RegisterModel : LoginAwareCrudPage
    {
        public override Type PageType => typeof(Pages_Register);

        public override string Title => "Бюлетені";

        public void OnGet()
        {
        }
    }
}
