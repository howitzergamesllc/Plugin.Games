
using GameKit;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace Plugin.Games
{
    internal static class Extensions
    {
        internal static Task<GKLocalPlayer> LocalPlayerAsync() => MainThread.InvokeOnMainThreadAsync(() => GKLocalPlayer.LocalPlayer);
    }
}
