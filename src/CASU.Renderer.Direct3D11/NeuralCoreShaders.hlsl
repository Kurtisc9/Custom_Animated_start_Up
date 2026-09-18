cbuffer SceneConstants : register(b0)
{
    float Time;
    float Aspect;
    float2 Padding;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float2 UV       : TEXCOORD0;
};

VSOutput VSMain(uint vertexId : SV_VertexID)
{
    VSOutput output;

    float2 positions[6] =
    {
        float2(-1.0, -1.0),
        float2(-1.0,  1.0),
        float2( 1.0,  1.0),
        float2(-1.0, -1.0),
        float2( 1.0,  1.0),
        float2( 1.0, -1.0)
    };

    float2 position = positions[vertexId];
    output.Position = float4(position, 0.0, 1.0);
    output.UV = position * float2(0.5, -0.5) + 0.5;
    return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
    float2 uv =
        input.UV * 2.0f - 1.0f;

    uv.x *= Aspect;

    float radius =
        length(uv);

    float angle =
        atan2(uv.y, uv.x);

    float pulse =
        0.5f +
        0.5f * sin(Time * 2.0f);

    float core =
        smoothstep(
            0.48f + pulse * 0.025f,
            0.08f,
            radius
        );

    float shell =
        smoothstep(
            0.045f,
            0.0f,
            abs(radius - 0.50f)
        );

    float orbit1Radius =
        0.72f +
        0.035f *
        sin(angle * 3.0f + Time);

    float orbit1 =
        smoothstep(
            0.012f,
            0.0f,
            abs(radius - orbit1Radius)
        );

    float orbit2Radius =
        0.88f +
        0.025f *
        sin(angle * 5.0f - Time * 0.7f);

    float orbit2 =
        smoothstep(
            0.008f,
            0.0f,
            abs(radius - orbit2Radius)
        );

    float3 cyan =
        float3(
            0.05f,
            0.72f,
            1.00f
        );

    float3 violet =
        float3(
            0.48f,
            0.12f,
            1.00f
        );

    float3 whiteBlue =
        float3(
            0.75f,
            0.92f,
            1.00f
        );

    float3 background =
        float3(
            0.002f,
            0.004f,
            0.015f
        );

    float3 color =
        background;

    color +=
        cyan *
        core *
        (0.55f + pulse * 0.45f);

    color +=
        whiteBlue *
        shell *
        0.9f;

    color +=
        cyan *
        orbit1 *
        0.55f;

    color +=
        violet *
        orbit2 *
        0.60f;

    float glow =
        exp(-radius * 3.5f) *
        (0.15f + pulse * 0.08f);

    color +=
        lerp(cyan, violet, pulse) *
        glow;

    return float4(
        saturate(color),
        1.0f
    );
}
