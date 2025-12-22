using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class UsersModel : LoginAwareCrudPage
    {
        public override string Title => "Manage Users";

        public override Type PageType => typeof(Pages_Users);

        public IActionResult OnPostToggle(int id, bool currentState) 
        {
            PermissionsDb.SetActive(id, !currentState);
            return BackToList();
        }

        public IActionResult OnPost(int userId, Login.AccessLevelCode userRole)
        {
            PermissionsDb.AddUser(userId, userRole);
            return BackToList();
        }
    }
}
