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

        public async Task<IActionResult> OnPost(int candId, string candName, DateTime? candDate, string? candDesc, Candidate.EntryType candKind) 
        {
            if (!CheckRole(out var failed)) return failed!;
            if (candId == DEFAULT_ID)
            {
                await CandidatesDb.AddCandidate(candName, candDate, candDesc, candKind);
            }
            else
            {
                if (!CheckCanModify(await CandidatesDb.GetCandidateHasPastUsesById(candId), false, out var fault)) return fault!;
                await CandidatesDb.UpdateCandidate(candId, candName, candDate, candDesc, candKind);
            }
            return BackToList();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            if (!CheckRole(out var failed)) return failed!;
            if (!CheckCanModify(await CandidatesDb.GetCandidateHasPastUsesById(id), true, out var fault)) return fault!;
            await CandidatesDb.DeleteCandidate(id);
            return BackToList();
        }

        private bool CheckCanModify(bool isUsed, bool deleting, out IActionResult? fault)
        {
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
