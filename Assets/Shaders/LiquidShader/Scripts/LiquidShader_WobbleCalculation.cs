using UnityEngine;

/// <summary>
/// Script used to feed rotation and position values to the "LiquidShader"-ShaderGraph, based on the (calculated) velocity of an object.
/// </summary>
/// <remarks>
/// Source: https://www.patreon.com/posts/24996282
/// </remarks>
public class LiquidShader_WobbleCalculation : MonoBehaviour
{
    private Renderer rend;
    private Vector3 lastPos;
    private Vector3 velocity;
    private Vector3 lastRot;
    private Vector3 angularVelocity;

    public float MaxWobble = 0.05f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float pulse;
    private float time = 0.5f;

    private readonly string shaderProp_WobbleX = "_WobbleX";
    private readonly string shaderProp_WobbleZ = "_WobbleZ";

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        rend.material.SetFloat(shaderProp_WobbleX, wobbleAmountX);
        rend.material.SetFloat(shaderProp_WobbleZ, wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
}