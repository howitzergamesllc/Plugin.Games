using Foundation;
using Plugin.Games;
using UIKit;

namespace GamesTest;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
	{		
		CrossGames.Current.OnCapabilityUnavailable += OnGameCenterPrivlagesRevoked;
		//_ = RequestGamesServerAccessAsync(); Optional
		return base.FinishedLaunching(application, launchOptions);
	}
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	private void OnGameCenterPrivlagesRevoked(object? sender, GamesCapability capability)
	{
		switch (capability)
		{
			case GamesCapability.Multiplayer:
				//DisableMultiplayer();
				break;

			case GamesCapability.InGameCommunication:
				//HideChat();
				break;

			case GamesCapability.ExplicitContent:
				//ShowContentWarning();
				break;
		}
	}

	private async Task RequestGamesServerAccessAsync()
	{
		try
		{
        	GamesAuthResponse response = await CrossGames.Current.RequestServerSideAccessAsync();
			if(response is null)
			{
				System.Diagnostics.Debug.WriteLine($"User not signed in.");
				return;
			}
			else
			{
				var publicURI = response.Apple.PublicKeyUrl;
				var signature = Convert.ToBase64String(response.Apple.Signature);
				//Use these values to verify the identity of the player on your backend server.
				//You can also access the Salt and Timestamp from response.Apple.Salt and response.Apple.Timestamp
				//To mitigate replay attacks, make sure the timestamp parameter is recent, and to avoid high network overhead, 
				//respect the cache expiration headers.
				System.Diagnostics.Debug.WriteLine($"User signed in. Public Key URL: {publicURI}, Signature: {signature}");
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Sign-in failed: {ex.Message}");
		}
	}
}
