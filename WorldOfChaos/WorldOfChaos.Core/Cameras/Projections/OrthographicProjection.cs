using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras.Projections;

public class OrthographicProjection : Camera.IProjection
{
	public void Update(Camera camera, float dt)
	{
		ProjectionMatrix = Matrix4.CreateOrthographic(Height * Aspect, Height, Near, Far);
	}

	public void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse)
	{
		if (mouse.ScrollDelta != Vector2.Zero)
		{
			var scroll = mouse.ScrollDelta.Y;
			Height *= MathF.Pow(1 + Sensitivity, scroll);
			Height = float.Clamp(Height, 0, float.PositiveInfinity);
		}
	}

	public float Near { get; set; } = 0f;
	public float Far { get; set; } = 100.0f;
	public float Height { get; set; } = 1.0f;
	public float Aspect { get; set; }
	public Matrix4 ProjectionMatrix { get; private set; }
	public float Sensitivity { get; set; } = 0.002f;
}