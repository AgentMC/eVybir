using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVybir.Pages
{
    public class ExperimentModel() : PageModel
    {
        
        public string Message { get; set; }
        public void OnGet(int id)
        {
            Message = id.ToString();
        }

        public void OnPost(int id, string abc)
        {
            Message = id.ToString() + " ??? " + abc;

        }
    }
}
