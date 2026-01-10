using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class CheckVoterModel : LoginAwarePageBase
    {
        public override string Title => "Бюлетені громадянина";

        public int? VoterId { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (!CheckRole<Pages_CheckVoter>(out var failed)) return failed!;
            VoterId = id;
            return Page();
        }

        public IActionResult OnPostRegister(int id, int campaignId)
        {
            if (!CheckRole<Pages_CheckVoter>(out var failed)) return failed!;
            TicketsDb.Register(campaignId, id, true);
            return RedirectToPage(Location<Pages_CheckVoter>());
        }

        public IActionResult OnPostCancel(int id, Guid ticketId)
        {
            if (!CheckRole<Pages_CheckVoter>(out var failed))
                return failed!;
            else if (TicketsDb.CancelTicket(ticketId, id))
                return RedirectToPage(Location<Pages_CheckVoter>());
            else
                return BadRequest("Ticket was not cancelled, try again");
        }
    }
}
