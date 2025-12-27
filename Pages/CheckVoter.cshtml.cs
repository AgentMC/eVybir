using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class CheckVoterModel : LoginAwarePageBase
    {
        public override string Title => "Бюлетені громадянина";

        public int? VoterId { get; set; }

        public void OnGet(int? id)
        {
            VoterId = id;
        }

        public IActionResult OnPostRegister(int id, int campaignId)
        {
            TicketsDb.Register(campaignId, id, true);
            return RedirectToPage(Location<Pages_CheckVoter>());
        }

        public IActionResult OnPostCancel(Guid ticketId)
        {
            TicketsDb.CancelTicket(ticketId);
            return RedirectToPage(Location<Pages_CheckVoter>());
        }
    }
}
