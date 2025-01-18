using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras.Controls;

public class TargetFollowingControl : Camera.IControl
{
	private readonly Func<Vector3> _targetProvider;

	public TargetFollowingControl(Func<Vector3> targetProvider, Vector3 position)
	{
		_targetProvider = targetProvider;
		Position = position;
		FocalPoint = _targetProvider();
	}

	public void Update(Camera camera, float dt)
	{
		FocalPoint = _targetProvider();
		ViewMatrix = Matrix4.LookAt(Position, FocalPoint, Up);
	}

	public void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse) { }

	public Vector3 Position { get; }
	public Vector3 FocalPoint { get; private set; }
	public Vector3 Forward => Orientation * -Vector3.UnitZ;
	public Vector3 Right => Orientation * Vector3.UnitX;
	public Vector3 Up => Orientation * Vector3.UnitY;
	public Matrix4 ViewMatrix { get; private set; }

	private Quaternion Orientation =>
		Quaternion.FromAxisAngle(Vector3.UnitY, Yaw) *
		Quaternion.FromAxisAngle(Vector3.UnitX, Pitch);

	private float Pitch { get; set; } = 0;
	private float Yaw { get; set; } = 0;

	public float RotationSpeed { get; set; } = 1;

	public float Distance { get; set; } = 10;

	public Vector3 Target => FocalPoint;

	public float ZoomSpeed { get; set; } = 1;
}
