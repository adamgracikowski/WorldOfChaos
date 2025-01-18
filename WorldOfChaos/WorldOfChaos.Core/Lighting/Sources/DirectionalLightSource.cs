using OpenTK.Mathematics;

namespace WorldOfChaos.Core.Lighting.Sources;

public sealed class DirectionalLightSource : PhongLightSource
{
	public Vector3 Direction { get; set; }

	public DirectionalLightSource(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular)
		: base(ambient, diffuse, specular)
	{
		Direction = direction;
	}

	public override void Apply(Shader shader, string? collectionName = null, int? collectionIndex = null)
	{
		var prefix = collectionName != null && collectionIndex != null
			? $"{collectionName}[{collectionIndex}]."
			: "directionLight.";

		shader.LoadFloat4($"{prefix}direction", new Vector4(Direction, 0.0f));
		shader.LoadFloat3($"{prefix}ambient", Ambient);
		shader.LoadFloat3($"{prefix}diffuse", Diffuse);
		shader.LoadFloat3($"{prefix}specular", Specular);
	}
}