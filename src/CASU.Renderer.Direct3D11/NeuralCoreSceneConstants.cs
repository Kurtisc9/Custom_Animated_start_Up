using System.Numerics;
using System.Runtime.InteropServices;

namespace CASU.Renderer.Direct3D11;

[StructLayout(LayoutKind.Sequential)]
internal struct NeuralCoreSceneConstants
{
    public float Time;
    public float Aspect;
    public Vector2 Padding;

    public NeuralCoreSceneConstants(
        float time,
        float aspect)
    {
        Time = time;
        Aspect = aspect;
        Padding = Vector2.Zero;
    }
}
