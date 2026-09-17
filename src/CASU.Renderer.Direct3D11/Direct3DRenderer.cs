using System;
using System.Windows.Forms;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Vortice.Direct3D11.D3D11;
using static Vortice.DXGI.DXGI;

namespace CASU.Renderer.Direct3D11;

internal sealed class Direct3DRenderer : IDisposable
{
    private readonly Form _window;

    private ID3D11Device? _device;
    private ID3D11DeviceContext? _context;
    private IDXGIFactory2? _factory;
    private IDXGISwapChain1? _swapChain;
    private ID3D11RenderTargetView? _renderTarget;

    private bool _disposed;

    public FeatureLevel FeatureLevel { get; private set; }

    public Direct3DRenderer(Form window)
    {
        _window = window ??
            throw new ArgumentNullException(nameof(window));
    }

    public void Initialize()
    {
        FeatureLevel[] requestedLevels =
        {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0
        };

        var deviceResult =
            D3D11CreateDevice(
                IntPtr.Zero,
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                requestedLevels,
                out ID3D11Device device,
                out FeatureLevel selectedFeatureLevel,
                out ID3D11DeviceContext context
            );

        Console.WriteLine(
            $"D3D11_DEVICE_HRESULT=0x{deviceResult.Code:X8}"
        );

        if (deviceResult.Failure ||
            device is null ||
            context is null)
        {
            device?.Dispose();
            context?.Dispose();

            throw new InvalidOperationException(
                $"D3D11CreateDevice failed: 0x{deviceResult.Code:X8}"
            );
        }

        _device = device;
        _context = context;
        FeatureLevel = selectedFeatureLevel;

        Console.WriteLine("DIRECT3D11_DEVICE_CREATE=PASS");
        Console.WriteLine($"DIRECT3D_FEATURE_LEVEL={FeatureLevel}");

        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);

        if (_factory is null)
        {
            throw new InvalidOperationException(
                "CreateDXGIFactory2 returned null."
            );
        }

        Console.WriteLine("DXGI_FACTORY_CREATE=PASS");

        uint width =
            (uint)Math.Max(1, _window.ClientSize.Width);

        uint height =
            (uint)Math.Max(1, _window.ClientSize.Height);

        var description =
            new SwapChainDescription1
            {
                Width = width,
                Height = height,
                Format = Format.R8G8B8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(1, 0),
                BufferUsage = Usage.RenderTargetOutput,
                BufferCount = 2,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.FlipDiscard,
                AlphaMode = AlphaMode.Ignore,
                Flags = SwapChainFlags.None
            };

        _swapChain =
            _factory.CreateSwapChainForHwnd(
                _device,
                _window.Handle,
                description,
                null,
                null
            );

        if (_swapChain is null)
        {
            throw new InvalidOperationException(
                "CreateSwapChainForHwnd returned null."
            );
        }

        Console.WriteLine("DIRECT3D11_SWAPCHAIN_CREATE=PASS");

        CreateRenderTarget();

        Console.WriteLine("DIRECT3D11_RENDER_TARGET_CREATE=PASS");
    }

    private void CreateRenderTarget()
    {
        if (_swapChain is null || _device is null)
        {
            throw new InvalidOperationException(
                "Renderer is not initialized."
            );
        }

        _renderTarget?.Dispose();
        _renderTarget = null;

        using ID3D11Texture2D backBuffer =
            _swapChain.GetBuffer<ID3D11Texture2D>(0);

        _renderTarget =
            _device.CreateRenderTargetView(backBuffer);

        if (_renderTarget is null)
        {
            throw new InvalidOperationException(
                "Render-target creation returned null."
            );
        }
    }

    public void Resize()
    {
        if (_disposed ||
            _swapChain is null ||
            _context is null ||
            _window.ClientSize.Width <= 0 ||
            _window.ClientSize.Height <= 0)
        {
            return;
        }

        uint width = (uint)_window.ClientSize.Width;
        uint height = (uint)_window.ClientSize.Height;

        _context.OMSetRenderTargets(
            Array.Empty<ID3D11RenderTargetView>()
        );

        _renderTarget?.Dispose();
        _renderTarget = null;

        _swapChain.ResizeBuffers(
            2,
            width,
            height,
            Format.R8G8B8A8_UNorm,
            SwapChainFlags.None
        );

        CreateRenderTarget();

        Console.WriteLine(
            $"DIRECT3D11_RESIZE=PASS {width}x{height}"
        );
    }

    public void Render(float time)
    {
        if (_context is null ||
            _renderTarget is null ||
            _swapChain is null)
        {
            return;
        }

        float pulse =
            0.5f +
            (0.5f * MathF.Sin(time * 2.0f));

        var clearColor =
            new Color4(
                0.005f + (0.01f * pulse),
                0.02f  + (0.06f * pulse),
                0.08f  + (0.12f * pulse),
                1.0f
            );

        _context.OMSetRenderTargets(_renderTarget);

        _context.ClearRenderTargetView(
            _renderTarget,
            clearColor
        );

        _swapChain.Present(
            1,
            PresentFlags.None
        );
    }

    public void Dispose()
    {
        if (_disposed) {
            return;
        }

        _disposed = true;

        if (_context is not null)
        {
            _context.ClearState();
            _context.Flush();
        }

        _renderTarget?.Dispose();
        _swapChain?.Dispose();
        _factory?.Dispose();
        _context?.Dispose();
        _device?.Dispose();

        _renderTarget = null;
        _swapChain = null;
        _factory = null;
        _context = null;
        _device = null;

        Console.WriteLine(
            "DIRECT3D11_RENDERER_DISPOSE=PASS"
        );
    }
}
