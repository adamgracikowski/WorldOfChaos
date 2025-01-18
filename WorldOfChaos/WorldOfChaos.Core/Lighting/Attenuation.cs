namespace WorldOfChaos.Core.Lighting;

public static class Attenuation
{
	public sealed record AttenuationResult(
		float Constant,
		float Linear,
		float Quadratic
	);

	private static readonly Dictionary<int, AttenuationResult> AttenuationData = new()
	{
		{ 3250, new(1.0f, 0.0014f, 0.000007f) },
		{ 600,  new(1.0f, 0.007f, 0.0002f) },
		{ 325,  new(1.0f, 0.014f, 0.0007f) },
		{ 200,  new(1.0f, 0.022f, 0.0019f) },
		{ 160,  new(1.0f, 0.027f, 0.0028f) },
		{ 100,  new(1.0f, 0.045f, 0.0075f) },
		{ 65,   new(1.0f, 0.07f, 0.017f) },
		{ 50,   new(1.0f, 0.09f, 0.032f) },
		{ 32,   new(1.0f, 0.14f, 0.07f) },
		{ 20,   new(1.0f, 0.22f, 0.20f) },
		{ 13,   new(1.0f, 0.35f, 0.44f) },
		{ 7,    new(1.0f, 0.7f, 1.8f) }
	};

	public static AttenuationResult GetResultByDistance(int distance)
	{
		return AttenuationData.Select(kvp => new
		{
			Difference = Math.Abs(kvp.Key - distance),
			AttenuationResult = kvp.Value
		}).MinBy(d => d.Difference)!.AttenuationResult;
	}
}