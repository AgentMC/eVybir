using System.Globalization;

namespace eVybir.Infra
{
    public record Campaign(string Name, DateTimeOffset Start, DateTimeOffset End)
    {
        private static readonly CultureInfo cultureUa = new("uk-UA");
        const string NDASH = "–";

        public string UfDate
        {
            get
            {
                if ((End - Start).TotalDays <= 1.0) return Start.ToString("D", cultureUa);
                if (End.Month == Start.Month) return $"{Start.Day} {NDASH} {End.ToString("D", cultureUa)}";
                if (End.Year == Start.Year) return $"{Start.ToString("dd MMMM", cultureUa)} {NDASH} {End.ToString("D", cultureUa)}";
                return $"{Start.ToString("D", cultureUa)} {NDASH} {End.ToString("D", cultureUa)}";
            }
        }

        public enum CampaignState
        {
            Future,
            OnCooldown,
            Ongoing,
            Completed
        }

        public CampaignState State
        {
            get
            {
                var now = DateTimeOffset.Now;
                return Start > now 
                    ? (Start - now).TotalDays > 1.0 
                        ? CampaignState.Future 
                        : CampaignState.OnCooldown
                    : End > now 
                        ? CampaignState.Ongoing 
                        : CampaignState.Completed;
            }
        }

        public string UfState
        {
            get
            {
                return State switch
                {
                    CampaignState.Future => "Заплановано",
                    CampaignState.OnCooldown => "Тиха доба",
                    CampaignState.Ongoing => "У процесі",
                    CampaignState.Completed => "Завершено",
                    _ => throw new NotImplementedException()
                };
            }
        }
    }
}
