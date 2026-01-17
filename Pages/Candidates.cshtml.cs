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
                if (!CheckCanModify(candId, false, out var fault)) return fault!;
                CandidatesDb.UpdateCandidate(candId, candName, candDate, candDesc, candKind);
            }
            return BackToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!CheckRole(out var failed)) return failed!;
            if (!CheckCanModify(id, true, out var fault)) return fault!;
            CandidatesDb.DeleteCandidate(id);
            return BackToList();
        }

        private bool CheckCanModify(int id, bool deleting, out IActionResult? fault)
        {
            var isUsed = CandidatesDb.GetCandidateHasPastUsesById(id);
            if (isUsed && (deleting || LoginData?.AccessLevel != Login.AccessLevelCode.Admin))
            {
                fault = BadRequest("Unable to edit candidate, used in past Campaigns!");
                return false;
            }
            fault = null;
            return true;
        }
    }
}
