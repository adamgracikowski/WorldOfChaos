using OpenTK.Mathematics;

namespace WorldOfChaos.Core.Lighting.Sources;

public sealed class PointLightSource : PhongLightSource
{
	public float Constant { get; }
	public float Linear { get; }
	public float Quadratic { get; }

	public Vector3 Position { get; set; }

	public PointLightSource(Vector3 position, float range, Vector3 ambient, Vector3 diffuse, Vector3 specular)
		: base(ambient, diffuse, specular)
	{
		Position = position;

		var attenuationResult = Attenuation.GetResultByDistance(distance: (int)range);

		Constant = attenuationResult.Constant;
		Linear = attenuationResult.Linear;
		Quadratic = attenuationResult.Quadratic;
	}

	public override void Apply(Shader shader, string? collectionName = null, int? collectionIndex = null)
	{
		var prefix = collectionName is null || collectionIndex is null
			? "pointLight."
			: $"{collectionName}[{collectionIndex}].";
		
		shader.LoadFloat3($"{prefix}position", Position);
		shader.LoadFloat3($"{prefix}ambient", Ambient);
		shader.LoadFloat3($"{prefix}diffuse", Diffuse);
		shader.LoadFloat3($"{prefix}specular", Specular);
		shader.LoadFloat($"{prefix}constant", Constant);
		shader.LoadFloat($"{prefix}linear", Linear);
		shader.LoadFloat($"{prefix}quadratic", Quadratic);
	}
}