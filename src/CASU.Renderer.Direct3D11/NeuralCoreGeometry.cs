using System.Numerics;
using System.Runtime.InteropServices;

namespace CASU.Renderer.Direct3D11;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct NeuralCoreVertex
{
    public readonly Vector2 Position;

    public NeuralCoreVertex(float x, float y)
    {
        Position = new Vector2(x, y);
    }
}

internal static class NeuralCoreGeometry
{
    public static readonly NeuralCoreVertex[] FullscreenQuad =
    {
        new(-1.0f, -1.0f),
        new(-1.0f,  1.0f),
        new( 1.0f, -1.0f),

        new( 1.0f, -1.0f),
        new(-1.0f,  1.0f),
        new( 1.0f,  1.0f)
    };

    public const int VertexCount = 6;
}
