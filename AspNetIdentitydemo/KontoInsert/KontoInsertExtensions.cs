using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetIdentitydemo.KontoInsert
{
    public static class KontoInsertExtensions
    {
        public static AuthenticationBuilder AddKI(this AuthenticationBuilder builder)
            => builder.AddKI(KontoInsertDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddKI(this AuthenticationBuilder builder, Action<KontoInsertOptions> configureOptions)
            => builder.AddKI(KontoInsertDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddKI(this AuthenticationBuilder builder, string authenticationScheme, Action<KontoInsertOptions> configureOptions)
            => builder.AddKI(authenticationScheme, KontoInsertDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddKI(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<KontoInsertOptions> configureOptions)
            => builder.AddOAuth<KontoInsertOptions, KIHandler>(authenticationScheme, displayName, configureOptions);
    }
}
