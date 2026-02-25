
using System;
using System.Threading.Tasks;
using UIKit;
using GameKit;
using Microsoft.Maui.ApplicationModel;
using System.Linq;

namespace Plugin.Games
{
    /// <summary>
    /// Implementation for Apple Games Center
    /// </summary>
    public partial class GamesImplementation : BaseGames
    {
        private TaskCompletionSource<bool> signInTcs;
        private bool handlerAttached;

        /// <inheritdoc />
        public override Task<bool> SignInSilentlyAsync()
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    if (signInTcs == null || signInTcs.Task.IsCompleted)
                        signInTcs = new TaskCompletionSource<bool>();

                    var player = GKLocalPlayer.LocalPlayer;

                    AttachAuthenticationHandler(player);

                    return signInTcs.Task;
                }
                catch (Exception ex)
                {
                    throw new GamesException(GamesError.AuthenticationFailure, ex);
                }
            });
        }

        private void AttachAuthenticationHandler(GKLocalPlayer player)
        {
            if (handlerAttached)
            {
                if (player.Authenticated)
                {
                    ApplyAppleRestrictions(player);
                }
                signInTcs?.TrySetResult(player.Authenticated);
                return;
            }

            handlerAttached = true;

            player.AuthenticateHandler = (viewController, error) =>
            {
                if (viewController != null)
                {    
                    var window = UIApplication.SharedApplication
                        .ConnectedScenes
                        .OfType<UIWindowScene>()
                        .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive)?
                        .Windows
                        .FirstOrDefault(w => w.IsKeyWindow);
                    var rootViewController = window?.RootViewController;
                    rootViewController?.PresentViewController(viewController, true, null);
                    return;
                }

                bool success = error == null && player.Authenticated;

                if (success)
                    ApplyAppleRestrictions(player);

                signInTcs?.TrySetResult(success);
            };
        }

        /// <inheritdoc />
        public override Task<bool> SignInAsync() => SignInSilentlyAsync();

        private void ApplyAppleRestrictions(GKLocalPlayer player)
        {
            if (player.IsUnderage)
            {
                RaiseCapabilityUnavailable(GamesCapability.ExplicitContent);
            }

            if (player.MultiplayerGamingRestricted)
            {
                RaiseCapabilityUnavailable(GamesCapability.Multiplayer);
            }

            if (player.PersonalizedCommunicationRestricted)
            {
                RaiseCapabilityUnavailable(GamesCapability.InGameCommunication);
            }
        }

        /// <inheritdoc />
        public override Task<GamesAuthResponse> RequestServerSideAccessAsync()
        {
            var tcs = new TaskCompletionSource<GamesAuthResponse>();

            GKFetchItemsForIdentityVerificationSignatureCompletionHandler handler = (publicKeyURL, signature, salt, timestamp, error) => 
            {
                if (error != null)
                {
                    tcs.TrySetException(new Exception(error.LocalizedDescription));
                    return;
                }

                if (error != null || publicKeyURL is null || signature is null || salt is null)
                {
                    tcs.TrySetResult(null);
                    return;
                }

                var uri = new Uri(publicKeyURL.AbsoluteString);
                var sig = signature.ToArray();
                var sal = salt.ToArray();
                var authResponse = GamesAuthResponse.FromApple(new GamesAuthResponseApple(uri, sig, sal, timestamp));

                tcs.TrySetResult(authResponse);
            };

            GKLocalPlayer.LocalPlayer.FetchItemsForIdentityVerificationSignature(handler);

            return tcs.Task;
        }
    }
}