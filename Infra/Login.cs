using eVybir.Pages;
using eVybir.Repos;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static eVybir.Infra.Login;

namespace eVybir.Infra
{
    public record Login(int Id, string Name, AccessLevelCode AccessLevel, bool Enabled, DateTime expire)
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

            return new(id, userName, accessLevelCode.Value, enabled, DateTime.UtcNow.AddMinutes(30));
        }

        private static readonly JsonSerializerOptions options = new()
        {
            AllowOutOfOrderMetadataProperties = true,
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
        };

        private static readonly byte[] key = AppCfg.EncryptionKey;


        public static Login? Deserialize(string input)
        {
            var deBase64ed = Convert.FromBase64String(input);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = deBase64ed[..16];
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(deBase64ed[16..]);
            using var ds = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(ds, Encoding.UTF8);
            return JsonSerializer.Deserialize<Login>(sr.ReadToEnd(), options);
        }

        public static string Serialize(Login login)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(aes.IV);
            using (var es = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(es, Encoding.UTF8);
                sw.Write(JsonSerializer.Serialize(login));
            }//required to FlushFinalBlock() otherwise must be called manually.
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
