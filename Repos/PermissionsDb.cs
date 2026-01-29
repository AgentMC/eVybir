using eVybir.Infra;

namespace eVybir.Repos
{
    public class PermissionsDb : DbCore
    {
        public static async Task<int?> GetRoleById(int id)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            cmd.CommandText = $"select AccessLevel from {TPermissions} where Active = 1 and UserId = @{pId}";
            var result = (int?)await cmd.ExecuteScalarAsync();
            return result;
        }

        public static async IAsyncEnumerable<DbWrapped<int, Login>> GetUsers()
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"select Id, UserId, AccessLevel, Active from {TPermissions}";
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                yield return new((int)reader[0], Login.Create((int)reader[1], (Login.AccessLevelCode)reader[2], (bool)reader[3]));
            }
        }

        public static async Task SetActive(int id, bool active)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            var pActive = cmd.AddParameter("active", active);
            cmd.CommandText = $"update {TPermissions} set Active = @{pActive} where UserId = @{pId}";
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task AddUser(int userId, Login.AccessLevelCode accessLevel)
        {
            using var conn = await OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", userId);
            var pAccess = cmd.AddParameter("access", accessLevel);
            var pActive = cmd.AddParameter("active", true);
            cmd.CommandText = $"insert into {TPermissions} (UserId, AccessLevel, Active) values (@{pId}, @{pAccess}, @{pActive})";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
