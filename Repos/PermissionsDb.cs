using eVybir.Infra;

namespace eVybir.Repos
{
    public class PermissionsDb : DbCore
    {
        public static int? GetRoleById(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.Parameters.AddWithValue("id", id);
            cmd.CommandText = $"select AccessLevel from {TPermissions} where Active = 1 and UserId = @{pId}";
            var result = (int?)cmd.ExecuteScalar();
            return result;
        }

        public static IEnumerable<DbWrapped<int, Login>> GetUsers()
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"select Id, UserId, AccessLevel, Active from {TPermissions}";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new((int)reader[0], Login.Create((int)reader[1], (Login.AccessLevelCode)reader[2], (bool)reader[3]));
            }
        }

        public static void SetActive(int id, bool active)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.Parameters.AddWithValue("id", id);
            var pActive = cmd.Parameters.AddWithValue("active", active);
            cmd.CommandText = $"update {TPermissions} set Active = @{pActive} where UserId = @{pId}";
            cmd.ExecuteNonQuery();
        }

        public static void AddUser(int userId, Login.AccessLevelCode accessLevel)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.Parameters.AddWithValue("id", userId);
            var pAccess = cmd.Parameters.AddWithValue("access", accessLevel);
            var pActive = cmd.Parameters.AddWithValue("active", true);
            cmd.CommandText = $"insert into {TPermissions} (UserId, AccessLevel, Active) values (@{pId}, @{pAccess}, @{pActive})";
            cmd.ExecuteNonQuery();
        }
    }
}
