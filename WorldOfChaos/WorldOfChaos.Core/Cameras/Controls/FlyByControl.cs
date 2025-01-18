using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras.Controls;

public class FlyByControl : Camera.IControl
{
	public FlyByControl()
	{
		Pitch = Yaw = 0;
		Position = Vector3.Zero;
	}

	public FlyByControl(Camera.IControl control)
	{
		Position = control.Position;
		Pitch = MathF.Asin(control.Forward.Y);
		Yaw = MathF.Atan2(-control.Forward.X, -control.Forward.Z);
		ViewMatrix = Matrix4.LookAt(Position, Position + Forward, Up);
	}

	public FlyByControl(Vector3 position, Vector3 focal)
	{
		Position = position;
		var forward = (focal - position).Normalized();
		Pitch = MathF.Asin(forward.Y);
		Yaw = MathF.Atan2(-forward.X, -forward.Z);
		ViewMatrix = Matrix4.LookAt(Position, Position + Forward, Up);
	}

	public void Update(Camera camera, float dt)
	{
		ViewMatrix = Matrix4.LookAt(Position, Position + Forward, Up);
	}

	public void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse)
	{
		var move = Vector3.Zero;
		if (keyboard.IsKeyDown(Keys.W))
		{
			move += Forward * dt * Speed;
		}
		if (keyboard.IsKeyDown(Keys.S))
		{
			move -= Forward * dt * Speed;
		}
		if (keyboard.IsKeyDown(Keys.A))
		{
			move -= Right * dt * Speed;
		}
		if (keyboard.IsKeyDown(Keys.D))
		{
			move += Right * dt * Speed;
		}
		if (keyboard.IsKeyDown(Keys.Q))
		{
			move -= Up * dt * Speed;
		}
		if (keyboard.IsKeyDown(Keys.E))
		{
			move += Up * dt * Speed;
		}

		Position += move;

		var delta = mouse.Delta * dt;
		if (delta == Vector2.Zero) return;
		if (mouse.IsButtonDown(MouseButton.Button1))
		{
			Rotate(delta);
		}
	}

	public void Rotate(Vector2 delta)
	{
		var sign = Up.Y < 0 ? -1 : 1;
		Yaw -= sign * delta.X * RotationSpeed;
		Pitch -= delta.Y * RotationSpeed;
	}

	public Vector3 Position { get; set; }
	public Vector3 Forward => Orientation * -Vector3.UnitZ;
	public Vector3 Right => Orientation * Vector3.UnitX;
	public Vector3 Up => Orientation * Vector3.UnitY;
	public Matrix4 ViewMatrix { get; set; }

	public float RotationSpeed { get; set; } = 1;
	public float Speed { get; set; } = 1;

	private Quaternion Orientation =>
		Quaternion.FromAxisAngle(Vector3.UnitY, Yaw) *
		Quaternion.FromAxisAngle(Vector3.UnitX, Pitch);
	private float Pitch { get; set; }
	private float Yaw { get; set; }
}
