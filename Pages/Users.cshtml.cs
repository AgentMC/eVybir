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

        public async Task<IActionResult> OnPostToggle(int id, bool currentState) 
        {
            if (!CheckRole(out var failed)) return failed!;
            await PermissionsDb.SetActive(id, !currentState);
            return BackToList();
        }

        public async Task<IActionResult> OnPost(int userId, Login.AccessLevelCode userRole)
        {
            if (!CheckRole(out var failed)) return failed!;
            await PermissionsDb.AddUser(userId, userRole);
            return BackToList();
        }
    }
}
