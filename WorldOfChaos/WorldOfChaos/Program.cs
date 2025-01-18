using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace WorldOfChaos;

public class Program
{
	private const int ClientSizeX = 1280;
	private const int ClientSizeY = 800;
	private const int MinimumlientSizeX = 800;
	private const int MinimnumClientSizeY = 600;

	static void Main()
	{
		var gameWindowSettings = GameWindowSettings.Default;
		var nativeWindowSettings = new NativeWindowSettings
		{
			Title = "World of Chaos",
			ClientSize = new Vector2i(ClientSizeX, ClientSizeY),
			MinimumClientSize = new Vector2i(MinimumlientSizeX, MinimnumClientSizeY),
			WindowBorder = WindowBorder.Resizable
		};

#if DEBUG
		nativeWindowSettings.Flags |= ContextFlags.Debug;
#endif

		var window = new WorldOfChaosWindow(gameWindowSettings, nativeWindowSettings);

		window.Run();
	}
}
