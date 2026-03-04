# Games plugin for .NET MAUI

_This project has no affiliation with Microsoft or the Maui/Xamarin teams._

```
dotnet add package Plugin.Games
```

# Nuget.org
[![NuGet](https://img.shields.io/nuget/v/Plugin.Games.svg)](https://www.nuget.org/packages/Plugin.Games/)

## Package Reference
```xml
<PropertyGroup>
	<PackageReference Include="Plugin.Games" Version="10.0.40" />
</PropertyGroup>
```

## Features

|             | Achievements | Leaderboards |
| ----------- | ------------ | ------------ |
|   Android   |       ✅     |       ✅      |
|     iOS     |       ✅     |       ✅      |
| MacCatalyst |       ✅     |       ✅      |
|   Windows   |              |              |

### Google Features Omitted
Events, SavedGames, Recall, PlayerStats and Friends were omitted due to limitations of supporting package or because Apple doesn't provide
a feature that could be abstracted in a similar way to provide anything of use. Additionaly packages like Plugin.Firebase
already achieve many of these same features with a little more setup.

### Apple Features Omitted
Matchmaking, VoiceChat, GameActivities and Challenges were omitted simply because Google doesn't provide equivalent services 
that could be used to abstract similar features. If these features are desired, they already ship with .net Maui apps under the namespace
Microsoft.Maui.Platform.GameKit.

## Table of Contents

/Readme.md
/docs/
	Achievements.md
	Leaderboard.md
/src/
	/GamesTest/
		GamesTest.csproj
	/Plugin.Games/
		Plugin.Games.csproj


## Setup
Before starting, make sure to set up a google cloud project, create an OAuth2.0 client, and register it with your published app. See below for the instructions:
https://developer.android.com/games/pgs/console/setup

1. First initialize the project by adding the package to your project.
```
dotnet add package Plugin.Games
```

2. Override the Windows.OnCreated event in the App class for your project and call "await CrossGames.Current.SignInSilently" to sign into the Games SDK for both Android or iOS.

```C#
using Plugin.Games;

namespace GamesTest;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
		window.Created += async (s, e) =>
		{
			//Personal sign in, no server access. See AppDelegate or MainActivity for server access sign in examples.
			try
			{
				var signedIn = await CrossGames.Current.SignInSilentlyAsync();
				System.Diagnostics.Debug.WriteLine($"User Signed In: {signedIn}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Sign-in failed: {ex.Message}");
			}
		};
		return window;
	}
}
```

Players who are authenticated already will just sign in silently in the background with maybe a toast appearing in the user interface. For players who haven't authenticated, a sign in user interface action will prompt the user to sign in.


### Android Backend Server Access and Platform Specific Properties

3. Setup the AndroidManifest.xml to accept your Google Play Games project id.
```xml
<manifest>
  <application>
    <meta-data android:name="com.google.android.gms.games.APP_ID"
               android:value="@string/game_services_project_id"/>
  </application>
</manifest>
```

- Create a strings.xml resource file in the "./Platforms/Android/Resources/values" folder with the following
```xml
<?xml version="1.0" encoding="utf-8" ?>
<resources>  
 	<!-- Replace 0000000000 with your game’s project id. Example value shown above. -->	
	<string name="game_services_project_id" translatable="false">1024987000491</string>
</resources>
```

4. (Optional) To authenticate the user with your back end server, use "await CrossGames.Current.RequestServerSideAccessAsync" in the 
MainActivity in the Platforms/Android folder and overriding OnCreate(Budnle savedInstanceState).

- On Andorid, get the server OAuth2.0 identifier from your google cloud project.
```C#
string serverID = "28y9458jidfh2879";
```

- Specify the scopes to which your server authentication applies
```C#
List<GamesAuthScope> scopes = new List<GamesAuthScope>
{
	GamesAuthScope.Profile,
	GamesAuthScope.Email
};
```

- Request server authication by calling RequestServerSideAccessAsync and specify whether or not you want the the access token to refresh.
The method can be awaited for a GamesAuthResponse which contains the access token.
```C#        
try
{
	GamesAuthResponse response = await CrossGames.Current.RequestServerSideAccessAsync(serverID, false, scopes);
	if(response is null)
	{
		System.Diagnostics.Debug.WriteLine($"Server auth response is null.");
		return;
	}
	else
	{
		string token = response.Android.AuthorizationCode;
		//Pass the oauthToken to your backend server for verification and further processing.
		System.Diagnostics.Debug.WriteLine($"Server authenticated. Access Token: {token}");
	}
}
catch (Exception ex)
{
	System.Diagnostics.Debug.WriteLine($"Authentication failed: {ex.Message}");
}
```

5. (Optional) On Android, there is a data caching property you can override by setting. The default of the property is false
and the recommendation is to leave it false to take advantage of data caching properties.
```C#
GamesImplementation.ShouldForceReload = true;
```

6. (Optional) On Android, there are request code integers that can be used to specify what integer code to use for the following: 
	- FriendsResolutionRequiredException is received from a leaderboard method being called. The default code value is 1001.
	- ShowLeaderboardsCode is received from the ShowLeaderBoardsAsync methods being called. The default code value is 2002.
	- ShowAchievementsCode is received from the ShowAchievementsAsync method being called. The default code value is 3003.
Simply change the codes
```C#
GamesImplementation.ShowLeaderboardsCode = 1234;
GamesImplementation.FriendRequestCode = 5678;
GamesImplementation.ShowAchievementsCode = 1337;
```
Then verify and handle the right intents later in the main activity by overriding OnActivityResult
```c#
protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
{
    base.OnActivityResult(requestCode, resultCode, data);

    if (requestCode == GamesImplementation.FriendRequestCode)
    {
        if (resultCode == Result.Ok)
        {
            // User accepted → retry the friends leaderboard call
        }
        else
        {
            // User declined → fall back to public leaderboard
        }
    }
}
```

### Apple

7. Address access to specific content in your app based on player specific capabilities such as
whether or not the player has a parental permission to access explicit content. The Apple Game Center event
fires when a player should *NOT* have access to content.

- First create an action event handler:
```C#
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
```

- Then subscribe to the "OnCapabilityUnavailable" event in the AppDelegate for iOS and MacCatalyst by overriding FinishedLaunching(UIApplication application, NSDictionary? launchOptions).
```C#
	public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
	{		
		CrossGames.Current.OnCapabilityUnavailable += OnGameCenterPrivlagesRevoked;
		return base.FinishedLaunching(application, launchOptions);
	}
```

8. (Optional) To authenticate the user with your back end server, use "await CrossGames.Current.RequestServerSideAccessAsync" in the 
AppDelegate for iOS and MacCatalyst by overriding FinishedLaunching(UIApplication application, NSDictionary? launchOptions).
```C#
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
```


## Supporting Packages

| Platform |            Package Name             |  Version  |       SDK Name      | Version |
|----------|-------------------------------------|-----------|---------------------|---------|
| Android  | Xamarin.GooglePlayServices.Games.V2 | 121.0.0.1 | Play Games Services |  21.0.0 |
| iOS      |  Microsoft.Maui.Platform.GameKit    |           | Game Center         |         |


## Release notes
- Version 10.0.40
	- Made calls thread safe.
	- Fixed obsolete calls.
- Version 10.0.30
	- Release abstracting apple game center.
- Version 10.0.20
	- Initial release abstracting Play Games Services from android.