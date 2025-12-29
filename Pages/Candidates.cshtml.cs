using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;

namespace eVybir.Pages
{
    public class CandidatesModel : LoginAwareCrudPage
    {
        public override string Title => "Candidates";

        public override Type PageType => typeof(Pages_Candidates);

        public IActionResult OnPost(int candId, string candName, DateTime? candDate, string? candDesc, Candidate.EntryType candKind) 
        {
            if (!CheckRole(out var failed)) return failed!;
            if (candId == DEFAULT_ID)
            {
                CandidatesDb.AddCandidate(candName, candDate, candDesc, candKind);
            }
            else
            {
                CandidatesDb.UpdateCandidate(candId, candName, candDate, candDesc, candKind);
            }
            return BackToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!CheckRole(out var failed)) return failed!;
            CandidatesDb.DeleteCandidate(id);
            return BackToList();
        }
    }
}
