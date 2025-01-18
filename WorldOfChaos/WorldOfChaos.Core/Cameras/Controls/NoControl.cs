using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras.Controls;

public class NoControl : Camera.IControl
{
	public NoControl(Camera.IControl control)
	{
		Position = control.Position;
		Forward = control.Forward;
		Right = control.Right;
		Up = control.Up;
		ViewMatrix = control.ViewMatrix;
	}

	public NoControl(Vector3 position, Vector3 target, Vector3? up = null)
	{
		up ??= Vector3.UnitY;
		Position = position;
		Forward = (target - position).Normalized();
		Right = Vector3.Cross(Forward, up.Value).Normalized();
		Up = Vector3.Cross(Right, Forward).Normalized();
		ViewMatrix = Matrix4.LookAt(position, target, up.Value);
	}

	public void Update(Camera camera, float dt) { }

	public void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse) { }

	public Vector3 Position { get; }
	public Vector3 Forward { get; }
	public Vector3 Right { get; }
	public Vector3 Up { get; }
	public Matrix4 ViewMatrix { get; }
}
