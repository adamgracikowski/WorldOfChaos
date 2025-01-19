using OpenTK.Mathematics;
using WorldOfChaos.Core.Cameras.Controls;
using WorldOfChaos.Core.Cameras.Projections;

namespace WorldOfChaos.Core.Cameras;

public sealed class Cameraman
{
	private readonly Queue<Camera> CameraQueue = [];

	public Camera ActiveCamera { get; private set; }

	public Cameraman()
	{
		InitializeCameraQueue();

		ActiveCamera = CameraQueue.Dequeue();
	}

	private void InitializeCameraQueue()
	{
		// add default cameras

		var noControl = new NoControl(
			position: new Vector3(0, 4, 3),
			target: new Vector3(0, 1, -10)
		);

		var flyByControl = new FlyByControl(
			position: new Vector3(0, 4, 3),
			focal: new Vector3(0, 1, -10)
		);

		CameraQueue.Enqueue(
			new Camera(control: noControl, projection: new PerspectiveProjection())
		);

		CameraQueue.Enqueue(
			new Camera(control: flyByControl, projection: new PerspectiveProjection())
		);
	}

	public void SwitchCamera()
	{
		if (CameraQueue.Count == 0) return;

		var previouslyActive = ActiveCamera;

		CameraQueue.Enqueue(ActiveCamera);

		ActiveCamera = CameraQueue.Dequeue();
		ActiveCamera.Aspect = previouslyActive.Aspect;
	}

	public void UpdateAspect(float aspect)
	{
		ActiveCamera.Aspect = aspect;
	}

	public void EnqueueCamera(Camera camera)
	{
		CameraQueue.Enqueue(camera);
	}
}