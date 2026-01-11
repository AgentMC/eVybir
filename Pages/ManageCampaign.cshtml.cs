using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eVybir.Pages
{
    public class ManageCampaignModel : LoginAwarePageBase
    {
        public override string Title => "Призначити кандидатів";

        public IEnumerable<DbWrapped<int, Campaign>> Campaigns { get; private set; }

        public bool ShowOnlyList { get; private set; } = true;

        public int CurrentCampaignId { get; private set; }

        public List<DbWrapped<int, Candidate>> Candidates { get; private set; } 

        public List<Participant> ParticipantsRoot { get; private set; } 

        public int[] IncludedCandidateIds { get; private set; }

        public int[] SortedCandidateIds { get; private set; }

        public Campaign CurrentCampaign{ get; private set; }


        public IActionResult OnGet(int? id)
        {
            if (!CheckRole<Pages_ManageCampaign>(out var failed)) return failed!;
            Campaigns = CampaignsDb.GetAllCampaigns();
            if (id.HasValue)
            {
                CurrentCampaignId = id.Value;
                CurrentCampaign = Campaigns.FirstOrDefault(c => c.Key == CurrentCampaignId)?.Entity;
                if (CurrentCampaign == null) // wrong, race condition etc.
                {
                    return RedirectToPage(Location<Pages_ManageCampaign>());
                }

                ShowOnlyList = false;
                Candidates = CandidatesDb.GetCandidates().OrderBy(c => c.Entity.Name).ToList();
                ParticipantsRoot = CampaignCandidatesDb.GetParticipantsByCampaignFlat(CurrentCampaignId).ToList();
                IncludedCandidateIds = ParticipantsRoot.Select(p => p.CandidateId).ToArray();
                SortedCandidateIds = Candidates.Select(c => c.Key).ToArray();
                for (int i = ParticipantsRoot.Count - 1; i >= 0; i--)
                {
                    var participant = ParticipantsRoot[i];
                    if (participant.GroupId.HasValue)
                    {
                        ParticipantsRoot.RemoveAt(i);
                        ParticipantsRoot.First(p => p.CandidateId == participant.GroupId.Value).Children.Insert(0, participant);
                    }
                }
            }
            return Page();
        }

        public IActionResult OnPost(int? id, string inclusionModel)
        {
            if (!CheckRole<Pages_ManageCampaign>(out var failed)) return failed!;
            if (!id.HasValue) return NotFound();
            var c = CampaignsDb.GetCampaignById(id.Value);
            if (c.State != Campaign.CampaignState.Future) return BadRequest("Unable to edit, campaign state is " + c.UfState);
            var updateModel = JsonSerializer.Deserialize<List<Participant>>(inclusionModel)!;
            for (int i = updateModel.Count-1; i >= 0 ; i--)
            {
                var chCol = updateModel[i].Children;
                for (int j = chCol.Count -1; j >= 0; j--)
                {
                    updateModel.Add(chCol[j]);
                    chCol.RemoveAt(j);
                }
            }
            CampaignCandidatesDb.UpdateCampaignData(id.Value, updateModel);
            return RedirectToPage(Location<Pages_ManageCampaign>(), new { id = (int?)null });
        }
    }
}
