
using System.Diagnostics;
using Plugin.Games;

namespace GamesTest;

public partial class App : Application
{
	public static bool IsAndroid => DeviceInfo.Platform == DevicePlatform.Android;

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
				_ = RequestGamesServerAccessAsync();
				System.Diagnostics.Debug.WriteLine($"User Signed In: {signedIn}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Sign-in failed: {ex.Message}, stack trace: {ex.StackTrace}");
			}
		};
		return window;
	}
	
	private async Task RequestGamesServerAccessAsync()
    {        
        string serverID = "yourserverid";
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
#if ANDROID
                string token = response.Android.AuthorizationCode;
                //Pass the oauthToken to your backend server for verification and further processing.
		        System.Diagnostics.Debug.WriteLine($"Server authenticated. Access Token: {token}");
#endif
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Server Authentication failed: {ex.Message}");
        }
    }
}