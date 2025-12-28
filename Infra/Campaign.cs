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
    }
}
