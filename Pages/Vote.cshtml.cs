using eVybir.Infra;
using eVybir.Pages.Shared;
using eVybir.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVybir.Pages
{
    public class VoteModel : LoginAwarePageBase
    {
        public override string Title => "Віддати голос";

        public BulletinHierarchy[] Bulletin { get; set; }

        public void OnGet(Guid id)
        {
            var grp = TicketsDb.GetBulletinByTicket(id)
                               .GroupBy(b => b.Location.GroupId);
            Bulletin = grp.Single(g => g.Key == null)
                          .Select(b => new BulletinHierarchy(b)
                                  {
                                      Children = grp.SingleOrDefault(g => g.Key == b.Location.CandidateId)?.ToArray()
                                  })
                          .ToArray();
        }

        public void OnPost(Guid id, int[] castIds)
        {

        }
    }
}
