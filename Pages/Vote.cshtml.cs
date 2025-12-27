using eVybir.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVybir.Pages
{
    public class VoteModel : LoginAwarePageBase
    {
        public override string Title => "Віддати голос";

        public void OnGet(Guid id)
        {

        }

        public void OnPost(Guid id, int[] castIds)
        {

        }
    }
}
