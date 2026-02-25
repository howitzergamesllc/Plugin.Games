using Android.Gms.Games;
using Google.Android.Gms.Games.Gamessignin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Google Play Games
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        private static IGamesSignInClient SignInClient
        {
            get
            {
                EnsureInitialized();
                return PlayGames.GetGamesSignInClient(Activity);
            }
        }

        public override async Task<bool> SignInSilentlyAsync()
        {
            var signIn = SignInClient;

            try
            {
                var isAuthenticated = await signIn.IsAuthenticated().AsTask<AuthenticationResult>();
                return isAuthenticated.IsAuthenticated;
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.AuthenticationFailure, ex);
            }
        }

        public override async Task<bool> SignInAsync()
        {
            var signInclient = SignInClient;

            try
            {
                var isAuthenticated = await signInclient.SignIn().AsTask<AuthenticationResult>();
                return isAuthenticated.IsAuthenticated;
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.AuthenticationFailure, ex);
            }
        }

        public override async Task<GamesAuthResponse> RequestServerSideAccessAsync(string serverClientId, List<GamesAuthScope> authScopes)
        {
            var signInclient = SignInClient;
            
            try
            {
                var authResponse = await signInclient
                    .RequestServerSideAccess(serverClientId, ShouldForceReload, authScopes.ToNativeScopes())
                    .AsTask<AuthResponse>();

                return authResponse.ToGamesAuthResponse();
            }
            catch (Exception ex)
            {
                throw new GamesException(GamesError.AuthenticationFailure, ex);
            }
        }
    }
}