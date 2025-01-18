using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras.Controls;

public class ThirdPersonControl : Camera.IControl
{
	private readonly Func<Vector3> _targetProvider;

	public ThirdPersonControl(Func<Vector3> targetProvider)
	{
		_targetProvider = targetProvider;
		Pitch = Yaw = 0;
		Distance = 10;
		FocalPoint = _targetProvider();
	}

	public void Update(Camera camera, float dt)
	{
		FocalPoint = _targetProvider();
		Position = FocalPoint - Distance * Forward;
		ViewMatrix = Matrix4.LookAt(Position, FocalPoint, Up);
	}

	public void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse)
	{
		var delta = mouse.Delta * dt;
		if (delta == Vector2.Zero) return;
		if (mouse.IsButtonDown(MouseButton.Button1))
		{
			Rotate(delta);
		}
		else if (mouse.IsButtonDown(MouseButton.Button2))
		{
			Zoom(delta);
		}
	}

	public void Rotate(Vector2 delta)
	{
		var sign = Up.Y < 0 ? -1 : 1;
		Yaw += sign * delta.X * RotationSpeed;
		Pitch -= delta.Y * RotationSpeed;
	}

	public void Zoom(Vector2 delta)
	{
		Distance = MathF.Max(1.0f, Distance * MathF.Pow(1 + ZoomSpeed, delta.Y));
	}

	public Vector3 Position { get; private set; }
	public Vector3 FocalPoint { get; private set; }
	public Vector3 Forward => Orientation * -Vector3.UnitZ;
	public Vector3 Right => Orientation * Vector3.UnitX;
	public Vector3 Up => Orientation * Vector3.UnitY;
	public Matrix4 ViewMatrix { get; private set; }

	public float ZoomSpeed { get; set; } = 1;
	public float RotationSpeed { get; set; } = 1;

	private Quaternion Orientation =>
		Quaternion.FromAxisAngle(Vector3.UnitY, Yaw) *
		Quaternion.FromAxisAngle(Vector3.UnitX, Pitch);

	private float Pitch { get; set; }
	private float Yaw { get; set; }
	private float Distance { get; set; }
}