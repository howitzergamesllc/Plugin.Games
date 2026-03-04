using Plugin.Games;
using System.Diagnostics;

namespace GamesTest
{
    public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
        }

        private async void ButtonOpenAchievements_Clicked(object sender, EventArgs e)
		{
			try
			{
				await CrossGames.Current.ShowAchievementsAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

        private async void ButtonOpenLeaderBoards_Clicked(object sender, EventArgs e)
		{				
			try
			{
				await CrossGames.Current.ShowLeaderBoardsAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

        private async void ButtonOpenLeaderBoard_Clicked(object sender, EventArgs e)
		{
			try
			{
				await CrossGames.Current.ShowLeaderBoardsAsync("leaderboardTest1");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private async void ButtonIncrementAchievement_Clicked(object sender, EventArgs e)
		{
			string achievementId = "";
			#if ANDROID
			achievementId = "CgkIq93xr-odEAIQBQ";
			#elif IOS || MACCATALYST
			achievementId = "luckyseven";
			#endif
			try
			{
				var success = await CrossGames.Current.IncrementAchievementAsync(achievementId, App.IsAndroid ? 1 : 15);
				Debug.WriteLine($"IncrementAchievementProgressAsync returned: {success}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private async void ButtonSetAchievement_Clicked(object sender, EventArgs e)
		{
			string achievementId = "";
			#if ANDROID
			achievementId = "CgkIq93xr-odEAIQBQ";
			#elif IOS || MACCATALYST
			achievementId = "luckyseven";
			#endif
			try
			{
				var success = await CrossGames.Current.SetAchievementProgressAsync(achievementId, App.IsAndroid ? 6 : 90);
				await DisplayAlertAsync("Lucky Seven", $"Unlocked? {success}", "Close");
				Debug.WriteLine($"SetAchievementProgressAsync returned: {success}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private async void ButtonGetAchievements_Clicked(object sender, EventArgs e)
		{
			try
			{
				var achievements = await CrossGames.Current.GetAchievementsAsync();
				if (achievements == null || achievements.Count() == 0)
				{
					Debug.WriteLine("No achievements returned.");
				}
				else
				{
					foreach (var achievement in achievements)
					{
						Debug.WriteLine($"Achievement: {achievement.AchievementId}, IsUnlocked: {achievement.State}, Apple Progress: {achievement.Apple?.PercentComplete}, Android Progress: {achievement.Android?.CurrentSteps}/{achievement.Android?.TotalSteps}");
					}
					await DisplayAlertAsync("Achievements", $"Achievment Count {achievements.Count}", "Close");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

        private async void ButtonRevealAchievement_Clicked(object sender, EventArgs e)
        {			
			try
			{
				string achievementId = "";
				#if ANDROID
				achievementId = "CgkIq93xr-odEAIQBQ";
				#elif IOS || MACCATALYST
				achievementId = "luckyseven";
				#endif
				await CrossGames.Current.RevealAchievementAsync(achievementId);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
        }

        private async void ButtonUnlockAchievement_Clicked(object sender, EventArgs e)
        {
			try
			{
				string achievementId = "";
				#if ANDROID
				achievementId = "CgkIq93xr-odEAIQBQ";
				#elif IOS || MACCATALYST
				achievementId = "luckyseven";
				#endif
				await CrossGames.Current.UnlockAchievementAsync(achievementId);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
        }

        private async void ButtonGetSpecificLeaderBoardData_Clicked(object sender, EventArgs e)
		{
			try
			{
				var leaderboard = await CrossGames.Current.GetLeaderBoardDataAsync("leaverboardTest1");
				if(leaderboard == null)
				{
					Debug.WriteLine("No leaderboard data returned.");
					return;
				}
				Debug.WriteLine($"Leaderboard: {leaderboard.LeaderboardId}, Name: {leaderboard.DisplayName}, Apple Type: {leaderboard.Apple.LeaderboardType}, Android Total Scores on Leaderboard: {leaderboard.Android.NumScores}");
				await DisplayPromptAsync("Success, Retrieved leaderboard specific data.", "OK");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
        }

        private async void ButtonGetTopScores_Clicked(object sender, EventArgs e)
		{
 			try
			{
				var scores = await CrossGames.Current.GetTopScoresAsync("leaderboardTest1", GamesLeaderBoardTimeSpan.AllTime, GamesLeaderboardCollection.Public, App.IsAndroid ? 10 : 50);
				if (scores == null || scores.Count() == 0)
				{
					Debug.WriteLine("No top scores returned.");
				}
				else
				{
					foreach (var score in scores)
					{
						Debug.WriteLine($"Score: {score.DisplayScore}, Name: {score.DisplayName}, Rank: {score.Rank}");
					}
					await DisplayAlertAsync("Top Scores", $"Retrieved {scores.Count()} top scores.", "OK");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
        }

        private async void ButtonGetPlayerCenteredScores_Clicked(object sender, EventArgs e)
		{
			try
			{
				var scores = await CrossGames.Current.GetPlayerCenteredScoresAsync("leaderboardTest1", GamesLeaderBoardTimeSpan.AllTime, GamesLeaderboardCollection.Public, App.IsAndroid ? 10 : 50);
				if (scores == null || scores.Count() == 0)
				{
					Debug.WriteLine("No player-centered scores returned.");
				}
				else
				{
					foreach (var score in scores)
					{
						Debug.WriteLine($"Score: {score.DisplayScore}, Name: {score.DisplayName}, Rank: {score.Rank}");
					}
					await DisplayAlertAsync("Player Centered Scores", $"Retrieved {scores.Count()} player-centered scores.", "OK");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
        }

		private async void ButtonSubmitScore_Clicked(object sender, EventArgs e)
		{
			var scoreValue = App.IsAndroid ? 12345L : 67890L;
			var metaData = new GamesScoreMetadata
				{
					GoogleScoreTag = "ckJdlm3j5ls",
					AppleContext = 123456
				};
			try
			{
				var result = await CrossGames.Current.SubmitScoreAsync("leaderboardTest1", scoreValue, metaData);
				if(result == null)
				{
					Debug.WriteLine("No result returned from SubmitScoreAsync.");
					return;
				}
				else
				{
					Debug.WriteLine($"Submitted score: {scoreValue}, Leaderboard ID: {result.LeaderboardId}");
				}
				await DisplayAlertAsync("Score Submitted", $"Submitted score: {scoreValue}, Leaderboard ID: {result.LeaderboardId}", "OK");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private async void ButtonBackgroundAuthentication_Clicked(object sender, EventArgs e)
		{
			try
			{
				var authenticated = await CrossGames.Current.SignInSilentlyAsync();
				Debug.WriteLine($"Background authentication authenticated: {authenticated}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Background authentication failed: {ex.Message}");
			}
		}
	}
}
