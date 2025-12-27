using Microsoft.Data.SqlClient;

namespace eVybir.Infra
{
    public static class Extensions
    {
        extension(string input)
        {
            public string JsEscape() => System.Text.Json.JsonSerializer.Serialize(input);
        }

        extension(SqlDataReader input)
        {
            /// <summary>
            /// Retrieves the value from the <typeparamref name="SqlDataReader"/> and checks for it to be DBNull before casting.
            /// </summary>
            /// <typeparam name="T">Returned type. This is expected to be a type nullable (?) modifier. Non-nullable structs will work too, but this method is not designed to work with them.</typeparam>
            /// <param name="i">Column index of the <typeparamref name="SqlDataReader"/>.</param>
            /// <returns>If the returned value is not null, returns the value cast to the requested type. 
            /// If the value is null, and the requested type T is a reference type, returns null.
            /// If the value is null, and the requested type T is a nullable value type, returns empty (no value) instance.
            /// If the value is null, and the requested type T is a NON-nullable value type, returns default value for the type.</returns>
            public T As<T>(int i)
            {
                object value = input[i];
                if (value is DBNull)
                {
#pragma warning disable CS8603 // Possible null reference return is expected.
                    return default;
#pragma warning restore CS8603
                }
                else
                {
                    return (T)value;
                }
            }
        }

        extension(SqlCommand cmd)
        {
            public SqlParameter AddParameter(string name, object value) => cmd.Parameters.AddWithValue(name, value);
            public SqlParameter AddParameterNullable(string name, object? value) => cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        static readonly TimeZoneInfo UkraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"); 
        extension(DateTime dt)
        {
            public DateTimeOffset AsKyivTimeZone()
            {
                switch (dt.Kind)
                {
                    case DateTimeKind.Unspecified:
                        return new(dt, UkraineTimeZone.GetUtcOffset(dt));
                    case DateTimeKind.Utc:
                        return TimeZoneInfo.ConvertTimeFromUtc(dt, UkraineTimeZone).AsKyivTimeZone();
                    default:
                        throw new ArgumentException("Use UTC with timezone or Unspecified without TZ - not Local!");
                }
            }
        }
    }
}
