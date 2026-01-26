namespace eVybir.Infra
{
    public static class AppCfg
    {
        private static IConfiguration? _configuration = null;

        private static IConfiguration Configuration { get { if (_configuration != null) { return _configuration; } else { throw new InvalidOperationException("Not initialized"); } } }

        public static void SetUp(IConfiguration config) => _configuration = config;

        public static string ConnectionString { get => field ??= Configuration.GetValue<string>("sql_connstr")!; }

        public static byte[] EncryptionKey { get => field ??= Configuration.GetValue<string>("encryption_key")!
                                                                           .Chunk(2)
                                                                           .Select(c => byte.Parse($"{c[0]}{c[1]}", System.Globalization.NumberStyles.HexNumber))
                                                                           .ToArray(); }
    }
}
