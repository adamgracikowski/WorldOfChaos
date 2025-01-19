using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using System.Runtime.InteropServices;
using WorldOfChaos.Core;
using WorldOfChaos.Core.Buffers;
using WorldOfChaos.Core.Cameras;
using WorldOfChaos.Core.Cameras.Controls;
using WorldOfChaos.Core.Cameras.Projections;
using WorldOfChaos.Core.Lighting;
using WorldOfChaos.Core.Lighting.Sources;
using WorldOfChaos.Core.Models;

namespace WorldOfChaos;

public sealed class WorldOfChaosWindow : GameWindow
{
	// Meshes:
	private Mesh PointLightMesh = null!;
	private Mesh CubeMesh = null!;
	private Mesh SphereMesh = null!;
	private Mesh PlaneMesh = null!;

	// Shaders:
	private Shader PointLightShader = null!;
	private Shader CubeShader = null!;
	private Shader MirrorShader = null!;

	// Textures:
	private Texture DiffuseCubeTexture = null!;
	private Texture SpecularCubeTexture = null!;
	private Texture DiffuseSphereTexture = null!;
	private Texture SpecularSphereTexture = null!;
	private Texture MirrorTexture = null!;
	private Texture DiffuseMirrorTexture = null!;
	private Texture SpecularMirrorTexture = null!;
	private Texture SpecularRockTexture = null!;
	private Texture DiffuseRockTexture = null!;

	// Materials:
	private Material CubeMaterial = null!;
	private Material SphereMaterial = null!;
	private Material RockMaterial = null!;
	private Material MirrorMaterial = null!;

	// Buffers:
	private FrameBuffer MirrorFramebuffer = null!;
	private RenderBuffer MirrorDepthBuffer = null!;

	// Models:
	private PointLight[] PointLights = [];
	private Cube[] Cubes = [];
	private Sphere Sphere = null!;
	private Sphere[] Spheres = [];
	private Plane Plane = null!;

	// Other:
	private const string IconFile = "world-of-chaos.png";
	private readonly Cameraman Cameraman = new();
	private readonly Lightman Lightman = new();
	private bool IsNight;
	private bool UseBlinn;
	private Vector3 DefaultBackgroundColor = Vector3.Zero;
	private readonly Vector3[] PointLightPositions =
	[
		new(0.7f, 0.2f, 2.0f),
		new(2.3f, -3.3f, -4.0f),
		new(-4.0f, 2.0f, -12.0f),
		new(0.0f, 0.0f, -3.0f)
	];
	private readonly Vector3[] CubePositions =
	[
		new(0.0f, 0.0f, 0.0f),
		new(2.0f, 5.0f, -15.0f),
		new(-1.5f, -2.2f, -2.5f),
		new(-3.8f, -2.0f, -12.3f),
		new(2.4f, -0.4f, -3.5f),
		new(-1.7f, 3.0f, -7.5f),
		new(1.3f, -2.0f, -2.5f),
		new(1.5f, 2.0f, -2.5f),
		new(1.5f, 0.2f, -1.5f),
		new(-1.3f, 1.0f, -1.5f)
	];
	private readonly Vector3[] SpherePositions =
	[
		new(-2.0f, 0.0f, 0.0f),
		new(3.0f, -1.0f, 3.0f),
		new(2.0f, 2.0f, -5.0f),
		new(5.0f, 0.0f, -7.3f),
		new(-1.5f, 2.0f, -4.0f),
	];
	
	private DebugProc DebugProcCallback { get; } = OnDebugMessage;

	private readonly Dictionary<Keys, Action> KeyDownBindings = [];

	public WorldOfChaosWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
	: base(gameWindowSettings, nativeWindowSettings)
	{
		LoadIcon();
		InitializeKeyBindings();
	}

	private void LoadIcon()
	{
		var iconPath = Path.Combine("Resources", "Icons", IconFile);

		if (!File.Exists(iconPath))
		{
			Console.WriteLine($"Icon file not found: {iconPath}");
			return;
		}

		using var stream = File.OpenRead(iconPath);
		var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

		var icons = new[]
		{
			new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, image.Data)
		};

		Icon = new WindowIcon(icons);
	}

	private void InitializeKeyBindings()
	{
		KeyDownBindings[Keys.Escape] = Close;

		KeyDownBindings[Keys.Space] = () =>
		{
			Cameraman.SwitchCamera();

			var controlType = Cameraman.ActiveCamera.Control.GetType().Name;
			var projectionType = Cameraman.ActiveCamera.Projection.GetType().Name;

			Console.WriteLine($"Camera switched to ({controlType}, {projectionType})");
		};

		KeyDownBindings[Keys.N] = () =>
		{
			IsNight = !IsNight;
			Console.WriteLine($"Night: {IsNight}");
		};

		KeyDownBindings[Keys.B] = () =>
		{
			UseBlinn = !UseBlinn;
			Console.WriteLine($"Blinn: {UseBlinn}");
		};

		KeyDownBindings[Keys.F] = () =>
		{
			Lightman.ToggleFog(CubeShader);
			Console.WriteLine($"Fog: {Lightman.Fog?.IsUsed}");
		};
	}

	private void ConfigureLighting()
	{
		// Lighting:
		var pointlights = PointLightPositions.Select(
			p => new PointLightSource(
					position: p,
					range: 50.0f,
					ambient: new Vector3(0.05f, 0.05f, 0.05f),
					diffuse: new Vector3(0.8f, 0.8f, 0.8f),
					specular: new Vector3(1.0f, 1.0f, 1.0f)
				 )
		);

		Lightman.PointLights.AddRange(pointlights);

		Lightman.FlashLights.Add(new FlashLightSource(
			positionProvider: () => Cameraman.ActiveCamera.Position,
			directionProvider: () => Cameraman.ActiveCamera.Front.Normalized(),
			range: 100.0f,
			ambient: new Vector3(0.0f, 0.0f, 0.0f),
			diffuse: new Vector3(1.0f, 1.0f, 1.0f),
			specular: new Vector3(1.0f, 1.0f, 1.0f)
		));

		Lightman.FlashLights.Add(new FlashLightSource(
			positionProvider: () => Sphere.Position,
			directionProvider: () => -Vector3.UnitY,
			range: 100.0f,
			ambient: new Vector3(0.0f, 0.0f, 0.0f),
			diffuse: new Vector3(1.0f, 1.0f, 1.0f),
			specular: new Vector3(1.0f, 1.0f, 1.0f)
		));

		Cameraman.EnqueueCamera(
			new Camera(
				new TargetFollowingControl(() => Sphere.Position, new(10, 10, 10)),
				new PerspectiveProjection()
			)
		);

		Cameraman.EnqueueCamera(
			new Camera(
				new ThirdPersonControl(() => Sphere.Position),
				new PerspectiveProjection()
			)
		);

		// Fog:
		Lightman.Fog = new Fog(start: 10, end: 50);
	}

	private void ConfigureResources()
	{
		// Meshes:
		PointLightMesh = PointLight.GenerateMesh();
		CubeMesh = Cube.GenerateMesh();
		PlaneMesh = Plane.GenerateMesh();
		SphereMesh = Sphere.GenerateMesh();

		// Shaders:
		PointLightShader = new Shader(
			("WorldOfChaos.Resources.Shaders.pointlight.vert.glsl", ShaderType.VertexShader),
			("WorldOfChaos.Resources.Shaders.pointlight.frag.glsl", ShaderType.FragmentShader)
		);

		CubeShader = new Shader(
			("WorldOfChaos.Resources.Shaders.phong.vert.glsl", ShaderType.VertexShader),
			("WorldOfChaos.Resources.Shaders.phong.frag.glsl", ShaderType.FragmentShader)
		);

		// Textures:
		DiffuseCubeTexture = new Texture("WorldOfChaos.Resources.Textures.box-diffuse.png");
		SpecularCubeTexture = new Texture("WorldOfChaos.Resources.Textures.box-specular.png");
		CubeMaterial = new Material(DiffuseCubeTexture, SpecularCubeTexture, shininess: 32.0f);

		SpecularSphereTexture = new Texture("WorldOfChaos.Resources.Textures.sphere-specular.png");
		DiffuseSphereTexture = new Texture("WorldOfChaos.Resources.Textures.sphere-diffuse.jpg");
		SphereMaterial = new Material(DiffuseSphereTexture, SpecularSphereTexture, shininess: 32.0f);

		SpecularMirrorTexture = new Texture("WorldOfChaos.Resources.Textures.stone-specular.png");
		DiffuseMirrorTexture = new Texture("WorldOfChaos.Resources.Textures.stone-diffuse.jpg");
		MirrorMaterial = new Material(SpecularMirrorTexture, DiffuseMirrorTexture, shininess: 32.0f);

		SpecularRockTexture = new Texture("WorldOfChaos.Resources.Textures.rock-specular.png");
		DiffuseRockTexture = new Texture("WorldOfChaos.Resources.Textures.rock-diffuse.jpg");
		RockMaterial = new Material(DiffuseRockTexture, SpecularRockTexture, shininess: 32.0f);
	}

	private void ConfigureModels()
	{
		// Models:
		PointLights = PointLightPositions.Select(p => new PointLight(PointLightMesh, new(1.0f, 1.0f, 1.0f))
		{
			Position = p,
			Scale = new(0.2f, 0.2f, 0.2f)
		}).ToArray();

		Cubes = CubePositions.Select(c => new Cube(CubeMesh, CubeMaterial)
		{
			Position = c,
			Scale = new(1.2f, 1.2f, 1.2f)
		}).ToArray();

		Sphere = new Sphere(SphereMesh, SphereMaterial)
		{
			Position = new Vector3(5, 5, 5)
		};

		Spheres = SpherePositions.Select(p => new Sphere(SphereMesh, RockMaterial)
		{
			Position = p,
			Scale = new Vector3(0.5f, 0.5f, 0.5f)
		}).ToArray();

		Plane = new Plane(PlaneMesh, material: null)
		{
			Position = new(0.0f, 0.0f, -20.0f),
			Scale = new(5.0f, 5.0f, 5.0f)
		};
	}

	// Methods from GameWindow:
	protected override void OnLoad()
	{
		base.OnLoad();

		Console.WriteLine("Hello from the World of Chaos!");

		GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		GL.DebugMessageCallback(DebugProcCallback, IntPtr.Zero);
		GL.Enable(EnableCap.DebugOutput);

#if DEBUG
		GL.Enable(EnableCap.DebugOutputSynchronous);
#endif
		GL.Enable(EnableCap.DepthTest);
		GL.DepthFunc(DepthFunction.Lequal);

		ConfigureResources();
		ConfigureModels();
		ConfigureLighting();
		ConfigureMirror();
	}
	
	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		base.OnUpdateFrame(args);

		var keyboard = KeyboardState.GetSnapshot();
		var mouse = MouseState.GetSnapshot();

		var dt = (float)args.Time;

		Cameraman.ActiveCamera.Update(dt);
		Cameraman.ActiveCamera.HandleInput(dt, keyboard, mouse);

		var fog = Lightman.Fog;

		if (fog is not null && fog.IsUsed)
		{
			GL.ClearColor(fog.Color.X, fog.Color.Y, fog.Color.Z, 0.0f);
		}
		else
		{
			GL.ClearColor(DefaultBackgroundColor.X, DefaultBackgroundColor.Y, DefaultBackgroundColor.Z, 0.0f);
		}

		Sphere.UpdatePositionAndRotation(dt, 1.0f);
	}

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);

		RenderSceneOnMirrorTexture();

		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		RenderScene(Cameraman.ActiveCamera.ViewMatrix, Cameraman.ActiveCamera.Position, Cameraman.ActiveCamera.ProjectionMatrix);

		RenderMirror();

		SwapBuffers();
	}

	protected override void OnResize(ResizeEventArgs e)
	{
		base.OnResize(e);

		Cameraman.ActiveCamera.Aspect = Size.X / (float)Size.Y;

		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
	}

	protected override void OnKeyDown(KeyboardKeyEventArgs e)
	{
		base.OnKeyDown(e);

		if (KeyDownBindings.TryGetValue(e.Key, out var action))
		{
			action.Invoke();
		}
	}

	protected override void OnUnload()
	{
		base.OnUnload();

		// Meshes:
		PointLightMesh.Dispose();
		CubeMesh.Dispose();
		SphereMesh.Dispose();
		
		// Shaders:
		PointLightShader.Dispose();
		CubeShader.Dispose();
		PlaneMesh.Dispose();
		MirrorShader.Dispose();

		// Textures:
		DiffuseCubeTexture.Dispose();
		SpecularCubeTexture.Dispose();
		SpecularSphereTexture.Dispose();
		DiffuseSphereTexture.Dispose();
		DiffuseMirrorTexture.Dispose();
		SpecularMirrorTexture.Dispose();

		// Buffers:
		MirrorFramebuffer.Dispose();
	}

	// Rendering methods:
	private void RenderScene(Matrix4 viewMatrix, Vector3 position, Matrix4 projectionMatrix)
	{
		RenderPointLights(viewMatrix, position, projectionMatrix);
		RenderCubes(viewMatrix, position, projectionMatrix);
		RenderSpheres();
	}

	private void RenderPointLights(Matrix4 viewMatrix, Vector3 position, Matrix4 projectionMatrix)
	{
		PointLightShader.Use();

		Lightman.ApplyFog(PointLightShader);

		PointLightShader.LoadFloat3("viewPos", position);
		PointLightShader.LoadMatrix4("view", viewMatrix);
		PointLightShader.LoadMatrix4("projection", projectionMatrix);

		PointLightMesh.Bind();

		foreach (var pointLight in PointLights)
		{
			PointLightShader.LoadMatrix4("model", pointLight.Model);
			PointLightShader.LoadFloat3("lightColor", pointLight.Color);

			pointLight.Mesh.RenderIndexed();
		}

		PointLightMesh.Unbind();
	}
	
	private void RenderCubes(Matrix4 viewMatrix, Vector3 position, Matrix4 projectionMatrix)
	{
		CubeMaterial.Use();

		CubeShader.Use();
		CubeShader.LoadMaterial(CubeMaterial);

		Lightman.ApplyLights(CubeShader);
		Lightman.ApplyFog(CubeShader);

		CubeShader.LoadMatrix4("view", viewMatrix);
		CubeShader.LoadFloat3("viewPos", position);
		CubeShader.LoadMatrix4("projection", projectionMatrix);
		CubeShader.LoadBool("night", IsNight);
		CubeShader.LoadBool("useBlinn", UseBlinn);

		CubeMesh.Bind();

		foreach (var cube in Cubes)
		{
			CubeShader.LoadMatrix4("model", cube.Model);
			CubeShader.LoadMatrix3("normalMatrix", new Matrix3(cube.NormalMatrix));

			cube.Mesh.Render();
		}

		CubeMesh.Unbind();
	}

	private void RenderSpheres()
	{
		SphereMaterial.Use();

		CubeShader.LoadMaterial(SphereMaterial);
		CubeShader.LoadMatrix4("model", Sphere.Model);
		CubeShader.LoadMatrix3("normalMatrix", new Matrix3(Sphere.NormalMatrix));

		Sphere.Mesh.Bind();
		Sphere.Mesh.RenderIndexed();

		RockMaterial.Use();
		CubeShader.LoadMaterial(RockMaterial);

		foreach (var sphere in Spheres)
		{
			CubeShader.LoadMatrix4("model", sphere.Model);
			CubeShader.LoadMatrix3("normalMatrix", new Matrix3(sphere.NormalMatrix));
			Sphere.Mesh.RenderIndexed();
		}

		Sphere.Mesh.Unbind();
	}

	// Mirror related methods:
	private void ConfigureMirror()
	{
		MirrorFramebuffer = new FrameBuffer();
		MirrorTexture = new Texture();
		MirrorDepthBuffer = new RenderBuffer();
		
		MirrorTexture.Allocate(1024, 1024, SizedInternalFormat.Rgba8);
		MirrorDepthBuffer.Allocate(1024, 1024, RenderbufferStorage.DepthComponent);

		MirrorFramebuffer.AttachTexture(FramebufferAttachment.ColorAttachment0, MirrorTexture);
		MirrorFramebuffer.AttachRenderBuffer(FramebufferAttachment.DepthAttachment, MirrorDepthBuffer);

		MirrorFramebuffer.CheckStatus();

		MirrorShader = new Shader(
			("WorldOfChaos.Resources.Shaders.mirror.vert.glsl", ShaderType.VertexShader),
			("WorldOfChaos.Resources.Shaders.mirror.frag.glsl", ShaderType.FragmentShader)
		);
	}

	private void RenderSceneOnMirrorTexture()
	{
		MirrorFramebuffer.Bind();

		GL.Viewport(0, 0, 1024, 1024);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		GL.Enable(EnableCap.DepthTest);
		GL.DepthFunc(DepthFunction.Lequal);

		var mirrorViewPos = Plane.Position - new Vector3(0, 0, -3);
		var reflectionMatrix = Matrix4.LookAt(mirrorViewPos, Vector3.UnitZ, Vector3.UnitY);
		var mirrorProjectionMatrix = Cameraman.ActiveCamera.ProjectionMatrix;

		RenderScene(reflectionMatrix, mirrorViewPos, mirrorProjectionMatrix);

		MirrorFramebuffer.Unbind();

		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
	}

	private void RenderMirror()
	{
		MirrorShader.Use();
		Lightman.ApplyFog(MirrorShader);

		MirrorTexture.Use(TextureUnit.Texture0);
		MirrorTexture.GenerateMipmaps();
		MirrorShader.LoadInt("diffuse", 0);
		MirrorShader.LoadBool("reflect", true);

		MirrorShader.LoadMatrix4("view", Cameraman.ActiveCamera.ViewMatrix);
		MirrorShader.LoadMatrix4("projection", Cameraman.ActiveCamera.ProjectionMatrix);
		MirrorShader.LoadMatrix4("model", Plane.Model);
		MirrorShader.LoadFloat3("viewPos", Cameraman.ActiveCamera.Position);

		Plane.Mesh.Bind();
		Plane.Mesh.RenderIndexed();

		MirrorShader.LoadMatrix4("model", Plane.Model *
			Matrix4.CreateScale(1.1f, 1.1f, 1.0f) *
			Matrix4.CreateTranslation(0f, 0f, -0.01f)
		);

		DiffuseMirrorTexture.Use(TextureUnit.Texture0);
		MirrorShader.LoadBool("reflect", true);
		Plane.Mesh.RenderIndexed();

		Plane.Mesh.Unbind();
	}

	// Debug:
	private static void OnDebugMessage(
		DebugSource source,
		DebugType type,
		int id,
		DebugSeverity severity,
		int length,
		IntPtr pMessage,
		IntPtr pUserParam)
	{
		var message = Marshal.PtrToStringAnsi(pMessage, length);

		var log = $"[{severity} source={source} type={type} id={id}] {message}";

		Console.WriteLine(log);
	}
}