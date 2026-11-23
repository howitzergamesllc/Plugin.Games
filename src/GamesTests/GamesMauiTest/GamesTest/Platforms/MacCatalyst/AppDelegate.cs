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
}
