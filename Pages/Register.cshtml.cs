using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class RegisterModel : LoginAwareCrudPage
    {
        public override Type PageType => typeof(Pages_Register);

        public override string Title => "Бюлетені";

        public void OnGet()
        {
        }

        public IActionResult OnPostRegister(int campaignId) 
        {
            TicketsDb.Register(campaignId, LoginData!.Id, false);
            return BackToList();
        }

        public IActionResult OnPostCancel(Guid ticketId) 
        {
            TicketsDb.CancelTicket(ticketId);
            return BackToList();
        }
    }
}
