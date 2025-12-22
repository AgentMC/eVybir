using eVybir.Repos;
using System.Text;
using System.Text.Json;
using static eVybir.Infra.Login;

namespace eVybir.Infra
{
    public record Login(int Id, string Name, AccessLevelCode AccessLevel, bool Enabled)
    {
        public enum AccessLevelCode
        {
            None = 0,
            Admin = 1,
            CEC = 2,
            LEC = 3,
            Voter = 4
        }

        public static string HumanizeAccessLevelCode(AccessLevelCode code)
        {
            return code switch
            {
                AccessLevelCode.Admin => "Admin",
                AccessLevelCode.CEC => "Central Electoral Commission",
                AccessLevelCode.LEC => "Local Electoral Commission",
                AccessLevelCode.Voter => "Voter",
                _ => "Undefined",
            };
        }

        public string Role => HumanizeAccessLevelCode(AccessLevel);


        public const string COOKIE = "eVybirId";

        public static Login? LogIn(string userId)
        {
            if (int.TryParse(userId, out var loginId))
            {
                AccessLevelCode accessLevel = (AccessLevelCode?)PermissionsDb.GetRoleById(loginId) ?? AccessLevelCode.Voter;
                return Create(loginId, accessLevel, true);
            }
            return null;
        }

        public static Login Create(int id, AccessLevelCode accessLevelCode, bool enabled)
        {                
            //the following should be coming from claims with userId
            var userName = IdentityResolver.ResolveName(id);
            return new(id, userName, accessLevelCode, enabled);
        }

        private static readonly JsonSerializerOptions options = new()
        {
            AllowOutOfOrderMetadataProperties = true,
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
        };

        public static Login? Deserialize(string input)
        {
            return JsonSerializer.Deserialize<Login>(Encoding.UTF8.GetString(Convert.FromBase64String(input)), options);
        }

        public static string Serialize(Login login)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(login)));
        }
    }
}
