using OpenTK.Mathematics;

namespace WorldOfChaos.Core.Lighting.Sources;

public sealed class FlashLightSource : PhongLightSource
{
	private Func<Vector3> PositionProvider { get; set; }
	private Func<Vector3> DirectionProvider { get; set; }

	public float CutOff { get; set; } = MathF.Cos(float.DegreesToRadians(12.5f));
	public float OuterCutOff { get; set; } = MathF.Cos(float.DegreesToRadians(17.5f));

	public Vector3 Position => PositionProvider();
	public Vector3 Direction => DirectionProvider();

	public float Constant { get; }
	public float Linear { get; }
	public float Quadratic { get; }

	public FlashLightSource(Func<Vector3> positionProvider, Func<Vector3> directionProvider, float range,
		Vector3 ambient, Vector3 diffuse, Vector3 specular)
		: base(ambient, diffuse, specular)
	{
		PositionProvider = positionProvider;
		DirectionProvider = directionProvider;

		var attenuationResult = Attenuation.GetResultByDistance((int)range);

		Constant = attenuationResult.Constant;
		Linear = attenuationResult.Linear;
		Quadratic = attenuationResult.Quadratic;
	}

	public override void Apply(Shader shader, string? collectionName = null, int? collectionIndex = null)
	{
		var prefix = collectionName is null || collectionIndex is null
			? "flashLight."
			: $"{collectionName}[{collectionIndex}].";

		shader.LoadFloat3($"{prefix}position", Position);
		shader.LoadFloat3($"{prefix}direction", Direction);
		shader.LoadFloat3($"{prefix}ambient", Ambient);
		shader.LoadFloat3($"{prefix}diffuse", Diffuse);
		shader.LoadFloat3($"{prefix}specular", Specular);
		shader.LoadFloat($"{prefix}constant", Constant);
		shader.LoadFloat($"{prefix}linear", Linear);
		shader.LoadFloat($"{prefix}quadratic", Quadratic);
		shader.LoadFloat($"{prefix}cutOff", CutOff);
		shader.LoadFloat($"{prefix}outerCutOff", OuterCutOff);
	}
}