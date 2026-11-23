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
