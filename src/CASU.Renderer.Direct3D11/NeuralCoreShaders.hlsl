cbuffer SceneConstants : register(b0)
{
    float Time;
    float Aspect;
    float2 Padding;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
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

float hash21(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return frac(p.x * p.y);
}

float ring(float2 p, float radius, float width)
{
    return 1.0 - smoothstep(width, width + 0.008, abs(length(p) - radius));
}

float4 PSMain(VSOutput input) : SV_TARGET
{
    float2 uv = input.UV * 2.0 - 1.0;
    uv.x *= Aspect;

    float t = Time;

    float3 color = float3(0.002, 0.006, 0.018);

    float haze =
        exp(-2.5 * length(uv)) *
        (0.15 + 0.08 * sin(t * 0.4));

    color += float3(0.02, 0.05, 0.12) * haze;

    float stars = 0.0;

    float2 grid = floor((uv + 4.0) * 45.0);
    float rnd = hash21(grid);

    if (rnd > 0.985)
    {
        float twinkle =
            0.35 +
            0.65 *
            sin(t * (1.0 + rnd * 3.0) + rnd * 20.0);

        stars = saturate(twinkle);
    }

    color += stars * float3(0.25, 0.55, 1.0);

    float2 coreUv = uv;

    float radius = length(coreUv);

    float core =
        1.0 -
        smoothstep(
            0.18,
            0.46,
            radius
        );

    float shell =
        ring(
            coreUv,
            0.48 +
            0.015 * sin(t * 0.7),
            0.018
        );

    float pulse =
        0.55 +
        0.45 *
        sin(t * 1.6 - radius * 18.0);

    float3 cyan =
        float3(
            0.05,
            0.70,
            1.00
        );

    float3 violet =
        float3(
            0.48,
            0.12,
            1.00
        );

    color +=
        core *
        lerp(
            violet,
            cyan,
            pulse
        ) *
        1.5;

    color +=
        shell *
        cyan *
        1.8;

    float angle =
        atan2(
            coreUv.y,
            coreUv.x
        );

    float neural =
        sin(
            angle * 14.0 +
            radius * 32.0 -
            t * 2.2
        );

    neural =
        smoothstep(
            0.72,
            1.0,
            neural
        );

    neural *=
        1.0 -
        smoothstep(
            0.18,
            0.52,
            radius
        );

    color +=
        neural *
        violet *
        0.8;

    float2 orbitUv = uv;

    orbitUv.y *= 2.5;

    float orbit1 =
        ring(
            orbitUv,
            0.72,
            0.008
        );

    orbitUv.x =
        uv.x * cos(0.55) -
        uv.y * sin(0.55);

    orbitUv.y =
        (
            uv.x * sin(0.55) +
            uv.y * cos(0.55)
        ) * 2.8;

    float orbit2 =
        ring(
            orbitUv,
            0.84,
            0.007
        );

    color +=
        orbit1 *
        cyan *
        0.55;

    color +=
        orbit2 *
        violet *
        0.45;

    float sweep =
        exp(
            -40.0 *
            abs(
                uv.y -
                sin(
                    uv.x * 2.0 +
                    t * 0.8
                ) * 0.15
            )
        );

    color +=
        sweep *
        float3(
            0.03,
            0.18,
            0.35
        ) *
        0.35;

    float vignette =
        smoothstep(
            1.65,
            0.30,
            length(uv)
        );

    color *=
        0.35 +
        0.65 * vignette;

    return float4(
        saturate(color),
        1.0
    );
}
