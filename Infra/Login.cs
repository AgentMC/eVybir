using eVybir.Pages;
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
                AccessLevelCode.Admin => "Адмін",
                AccessLevelCode.CEC => "Центральна виборча комісія",
                AccessLevelCode.LEC => "Територіальна виборча комісія",
                AccessLevelCode.Voter => "Виборець",
                _ => "Undefined",
            };
        }

        public string Role => HumanizeAccessLevelCode(AccessLevel);

        private static readonly Type[] Authenticated = [typeof(Pages_Register), typeof(Pages_Vote)],
                                       LecAndAdmin = [typeof(Pages_CheckVoter)],
                                       CecAndAdmin = [typeof(Pages_Campaigns), typeof(Pages_Candidates), typeof(Pages_ManageCampaign)],
                                       Admin = [typeof(Pages_Users)];

        public bool Granted(Type t)
        {
            if (Authenticated.Contains(t)) return AccessLevel != AccessLevelCode.None;
            if (LecAndAdmin.Contains(t)) return AccessLevel == AccessLevelCode.Admin || AccessLevel == AccessLevelCode.LEC;
            if (CecAndAdmin.Contains(t)) return AccessLevel == AccessLevelCode.Admin || AccessLevel == AccessLevelCode.CEC;
            if (Admin.Contains(t)) return AccessLevel == AccessLevelCode.Admin;
            return false;
        }

        public const string COOKIE = "eVybirId";

        public static Login? LogIn(string userId)
        {
            if (int.TryParse(userId, out var loginId))
            {
                return Create(loginId, (AccessLevelCode?)PermissionsDb.GetRoleById(loginId), true);
            }
            return null;
        }

        public static Login Create(int id, AccessLevelCode? accessLevelCode, bool enabled)
        {                
            //the following two should be coming from claims with userId
            var userName = IdentityResolver.ResolveName(id);
            accessLevelCode ??= IdentityResolver.ResolveAge(id) >= 18 ? AccessLevelCode.Voter : AccessLevelCode.None;

            return new(id, userName, accessLevelCode.Value, enabled);
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
