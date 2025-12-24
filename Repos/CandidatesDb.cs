using eVybir.Infra;

namespace eVybir.Repos
{
    public static class CandidatesDb
    {
        const string Table = "Candidates";

        public static IEnumerable<DbWrapped<int, Candidate>> GetCandidates()
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"select Id, Name, Date, Description, EntryType from {Table}";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new((int)reader[0], new((string)reader[1], reader.As<DateTime?>(2), reader.As<string?>(3), (Candidate.EntryType)reader[4]));
            }
        }

        public static void AddCandidate(string name, DateTime? date, string? description, Candidate.EntryType entryKind)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pName = cmd.AddParameter("name", name);
            var pDate = cmd.AddParameterNullable("date", date);
            var pDescr = cmd.AddParameterNullable("descr", description);
            var pKind = cmd.AddParameter("kind", entryKind);
            cmd.CommandText = $"insert into {Table} (Name, Date, Description, EntryType) values (@{pName}, @{pDate}, @{pDescr}, @{pKind})";
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCandidate(int id, string name, DateTime? date, string? description, Candidate.EntryType entryKind)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            var pName = cmd.AddParameter("name", name);
            var pDate = cmd.AddParameterNullable("date", date);
            var pDescr = cmd.AddParameterNullable("descr", description);
            var pKind = cmd.AddParameter("kind", entryKind);
            cmd.CommandText = $"update {Table} set Name=@{pName}, Date=@{pDate}, Description=@{pDescr}, EntryType=@{pKind} where id=@{pId}";
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCandidate(int id)
        {
            using var conn = DbCore.OpenConnection();
            using var cmd = conn.CreateCommand();
            var pId = cmd.AddParameter("id", id);
            cmd.CommandText = $"delete from {Table} where id=@{pId}";
            cmd.ExecuteNonQuery();
        }
    }
}
