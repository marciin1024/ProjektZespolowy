using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektZespolowy.Entities;

namespace Microsoft.AspNetCore.Routing
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions {
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints) {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Account");

            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                SignInManager<User> signInManager,
                [FromForm] string returnUrl) => {
                    await signInManager.SignOutAsync();
                    return TypedResults.LocalRedirect($"~/{returnUrl}");
                });

            accountGroup.MapGet("/Logout", async (
                ClaimsPrincipal user,
                SignInManager<User> signInManager
                ) => {
                    await signInManager.SignOutAsync();
                    return TypedResults.LocalRedirect($"~/");
                });

            return accountGroup;
        }
    }
}