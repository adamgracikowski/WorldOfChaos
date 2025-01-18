using OpenTK.Mathematics;
using WorldOfChaos.Core.Lighting.Sources;

namespace WorldOfChaos.Core.Lighting;

public sealed class Lightman
{
	public readonly List<DirectionalLightSource> DirectionalLights = [];
	public readonly List<PointLightSource> PointLights = [];
	public readonly List<FlashLightSource> FlashLights = [];
	public Fog? Fog { get; set; }

	public Lightman()
	{
		SetupLights();
	}

	private void SetupLights()
	{
		// configure default lights

		DirectionalLights.Add(new DirectionalLightSource(
			direction: new Vector3(-0.2f, -1.0f, -0.3f),
			ambient: new Vector3(0.2f, 0.2f, 0.2f),
			diffuse: new Vector3(0.5f, 0.5f, 0.5f),
			specular: new Vector3(1.0f, 1.0f, 1.0f)
		));
	}

	public void ApplyFog(Shader shader)
	{
		Fog?.Apply(shader);
	}

	public void ToggleFog(Shader shader)
	{
		if (Fog is null) return;

		Fog.IsUsed = !Fog.IsUsed;

		shader.LoadBool("fog.use", Fog.IsUsed);
	}

	public void ApplyLights(Shader shader)
	{
		ApplyLights(shader, DirectionalLights, nameof(DirectionalLights));
		ApplyLights(shader, PointLights, nameof(PointLights));
		ApplyLights(shader, FlashLights, nameof(FlashLights));
	}

	private static void ApplyLights<T>(Shader shader, List<T> lightSources, string name) where T : PhongLightSource
	{
		shader.LoadInt($"{name}Count", lightSources.Count);

		for (var i = 0; i < lightSources.Count; i++)
		{
			lightSources[i].Apply(shader, collectionName: name, collectionIndex: i);
		}
	}
}