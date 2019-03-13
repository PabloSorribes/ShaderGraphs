using System.Reflection;
using UnityEditor.ShaderGraph;
using UnityEngine;


/// <summary>
/// Tutorial: AE Tuts - "Refractive glass shader in Unity 3D using shader graph" (https://youtu.be/-7UmfKUb1Zg)
/// </summary>
[Title("Custom", "Refraction Node")]
public class RefractionNode : CodeFunctionNode
{
	public RefractionNode()
	{
		name = "Refraction Node";
	}

	protected override MethodInfo GetFunctionToConvert()
	{
		return GetType().GetMethod("MyCustomFunction",
			BindingFlags.Static | BindingFlags.NonPublic);
	}

	private static string MyCustomFunction(
		[Slot(0, Binding.None)] Vector1 IndexOfRefraction,
		[Slot(1, Binding.None)] Vector3 ViewDirection,
		[Slot(2, Binding.None)] Vector3 NormalDirection,
		[Slot(3, Binding.None)] out Vector3 Out)
	{
		Out = Vector3.zero;
		return
			@"
{
	Out = refract(normalize(ViewDirection), normalize(NormalDirection), IndexOfRefraction);
}
";
	}
}