using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages.Shared
{
    public abstract class LoginAwareCrudPage : LoginAwarePageBase
    {
        public abstract Type PageType { get; }

        public RedirectToPageResult BackToList()
        {
            return RedirectToPage(Location(PageType));
        }

        public const int DEFAULT_ID = -1;

        protected bool CheckRole(out IActionResult? failed) => CheckRole(PageType, out failed);

        public virtual IActionResult OnGet()
        {
            return CheckRole(out var failed) ? Page() : failed!;
        }
    }
}
