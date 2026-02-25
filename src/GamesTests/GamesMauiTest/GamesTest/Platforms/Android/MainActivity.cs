using Android.Content;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Games;

namespace GamesTest;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        CrossGames.Initialize(this);
        //_ = RequestGamesServerAccessAsync(); Optional
    }

    private async Task RequestGamesServerAccessAsync()
    {        
        string serverID = "28y9458jidfh2879";
        List<GamesAuthScope> scopes = new List<GamesAuthScope>
        {
            GamesAuthScope.Profile,
            GamesAuthScope.Email
        };
        try
        {
            GamesAuthResponse response = await CrossGames.Current.RequestServerSideAccessAsync(serverID, scopes);
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
    }

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
}
