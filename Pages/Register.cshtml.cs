using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class RegisterModel : LoginAwareCrudPage
    {
        public override Type PageType => typeof(Pages_Register);

        public override string Title => "Бюлетені";

        public async Task<IActionResult> OnPostRegister(int campaignId) 
        {
            if (!CheckRole(out var failed)) return failed!;
            await TicketsDb.Register(campaignId, LoginData!.Id, false);
            return BackToList();
        }

        public async Task<IActionResult> OnPostCancel(Guid ticketId) 
        {
            if (!CheckRole(out var failed))
                return failed!;
            else if (await TicketsDb.CancelTicket(ticketId, LoginData!.Id))
                return BackToList();
            else
                return BadRequest("Ticket was not cancelled, try again");
        }
    }
}
