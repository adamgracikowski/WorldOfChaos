using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldOfChaos.Core.Cameras;

public class Camera
{
	public IControl Control { get; set; }
	public IProjection Projection { get; set; }
	public Vector3 Position => Control.Position;
	public Vector3 Front => Control.Forward;
	public Vector3 Right => Control.Right;
	public Vector3 Up => Control.Up;

	public float Aspect
	{
		set => Projection.Aspect = value;
		get => Projection.Aspect;
	}

	public Matrix4 ProjectionMatrix => Projection.ProjectionMatrix;
	public Matrix4 ViewMatrix => Control.ViewMatrix;
	public Matrix4 ProjectionViewMatrix => ViewMatrix * ProjectionMatrix;

	public Camera(IControl control, IProjection projection)
	{
		Control = control;
		Projection = projection;
	}

	public interface IControl
	{
		void Update(Camera camera, float dt);
		void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse);
		Vector3 Position { get; }
		Vector3 Forward { get; }
		Vector3 Right { get; }
		Vector3 Up { get; }
		Matrix4 ViewMatrix { get; }
	}

	public interface IProjection
	{
		void Update(Camera camera, float dt);
		void HandleInput(Camera camera, float dt, KeyboardState keyboard, MouseState mouse);
		float Aspect { get; set; }
		Matrix4 ProjectionMatrix { get; }
	}

	public void Update(float dt)
	{
		Control.Update(this, dt);
		Projection.Update(this, dt);
	}

	public void HandleInput(float dt, KeyboardState keyboard, MouseState mouse)
	{
		Control.HandleInput(this, dt, keyboard, mouse);
		Projection.HandleInput(this, dt, keyboard, mouse);
	}
}