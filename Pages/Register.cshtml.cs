using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class RegisterModel : LoginAwareCrudPage
    {
        public override Type PageType => typeof(Pages_Register);

        public override string Title => "Бюлетені";

        public IActionResult OnPostRegister(int campaignId) 
        {
            if (!CheckRole(out var failed)) return failed!;
            TicketsDb.Register(campaignId, LoginData!.Id, false);
            return BackToList();
        }

        public IActionResult OnPostCancel(Guid ticketId) 
        {
            if (!CheckRole(out var failed))
                return failed!;
            else if (TicketsDb.CancelTicket(ticketId, LoginData!.Id))
                return BackToList();
            else
                return BadRequest("Ticket was not cancelled, try again");
        }
    }
}
