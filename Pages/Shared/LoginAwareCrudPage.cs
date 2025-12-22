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
    }
}
