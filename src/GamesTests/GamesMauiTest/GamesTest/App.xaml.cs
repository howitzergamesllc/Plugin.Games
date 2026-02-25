
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
				System.Diagnostics.Debug.WriteLine($"User Signed In: {signedIn}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Sign-in failed: {ex.Message}, stack trace: {ex.StackTrace}");
			}
		};
		return window;
	}
}