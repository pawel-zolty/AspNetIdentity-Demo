namespace AspNetIdentitydemo.KontoInsert
{
    public static class KontoInsertDefaults
    {
        public const string AuthenticationScheme = "KontoInsert";

        public static readonly string DisplayName = "Konto Insert";

        public static readonly string AuthorizationEndpoint = "https://kontodev.chmura.insert.pl/oauth/authorize";

        public static readonly string TokenEndpoint = "https://kontodev.chmura.insert.pl/oauth/token";

        //public static readonly string UserInformationEndpoint = "https://kontodev.chmura.insert.pl/oauth/resources/getCredentials";
        public static readonly string UserInformationEndpoint = "https://kontodev.chmura.insert.pl/oauth/resources/user/data";

        public const string ContextIdPropertyName = "context_id"; // KI
    }
}
