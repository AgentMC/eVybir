using Microsoft.Data.SqlClient;

namespace eVybir.Infra
{
    public static class Extensions
    {
        extension(string input)
        {
            public string JsEscape() => System.Text.Json.JsonSerializer.Serialize(input);
        }

        extension(object input)
        {
            public T? As<T>() where T:class
            {
                if (input == DBNull.Value) return null;
                return (T)input;
            }

            public T? AsNS<T>() where T : struct
            {
                if (input == DBNull.Value) return null;
                return (T)input;
            }
        }

        extension(SqlCommand cmd)
        {
            public SqlParameter AddParameter(string name, object value) => cmd.Parameters.AddWithValue(name, value);
            public SqlParameter AddParameterNullable(string name, object? value) => cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }
    }
}
