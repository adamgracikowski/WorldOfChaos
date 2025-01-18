using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras.Projections;

public class PerspectiveProjection : Camera.IProjection
{
	public void Update(Camera camera, float dt)
	{
		ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FovY, Aspect, Near, Far);
	}

	public void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse)
	{
		var scroll = mouse.ScrollDelta.Y;
		FovY *= MathF.Pow(1 + Sensitivity, scroll);
		FovY = float.Clamp(FovY, float.MinValue, float.Pi);
	}

	public float FovY { get; set; } = MathF.PI / 4;
	public float Aspect { get; set; }
	public float Near { get; set; } = 0.1f;
	public float Far { get; set; } = 100.0f;
	public Matrix4 ProjectionMatrix { get; private set; }
	public float Sensitivity { get; set; } = 0.002f;
}
