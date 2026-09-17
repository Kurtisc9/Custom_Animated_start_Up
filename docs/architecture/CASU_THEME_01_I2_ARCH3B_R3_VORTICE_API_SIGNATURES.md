# CASU_THEME_01-I2-ARCH3B-R3
## Vortice 3.8.3 API Signature Discovery

DATE=2026-09-17 05:34:16 -04:00

PROJECT_ROOT=X:\03_Active_Projects\Custom_Animated_Start_Up
BRANCH=phase-02b-r3d-emergency-bypass
SOURCE_HEAD=f094f8e664a413d96a97542562f9fd370950a587

VORTICE_DIRECT3D11_VERSION=3.8.3
VORTICE_DXGI_VERSION=3.8.3

DISCOVERY_METHOD=RUNTIME_REFLECTION
RENDERER_SOURCE_MODIFIED_BY_DISCOVERY=NO

R2_BUILD_FAILURE=CONFIRMED
R2_ASSUMED_D3D11CREATEDEVICE_SIGNATURE=SUPERSEDED

## Installed assembly paths

VORTICE_DIRECT3D11_DLL=C:\Users\kurti\.nuget\packages\vortice.direct3d11\3.8.3\lib\net9.0\Vortice.Direct3D11.dll
VORTICE_DXGI_DLL=C:\Users\kurti\.nuget\packages\vortice.dxgi\3.8.3\lib\net9.0-windows10.0.22621\Vortice.DXGI.dll

## Reflected API evidence

CASU_VORTICE_API_DISCOVERY=START
D3D_ASSEMBLY=Vortice.Direct3D11, Version=3.8.3.0, Culture=neutral, PublicKeyToken=5431ec61a7e925da
DXGI_ASSEMBLY=Vortice.DXGI, Version=3.8.3.0, Culture=neutral, PublicKeyToken=5431ec61a7e925da
D3D11_CREATE_DEVICE_SIGNATURES_BEGIN
TYPE=Vortice.Direct3D11.D3D11
FILTER=D3D11CreateDevice
METHOD_COUNT=10
SIGNATURE=Vortice.Direct3D11.ID3D11Device Vortice.Direct3D11.D3D11.D3D11CreateDevice(Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(Vortice.DXGI.IDXGIAdapter adapter, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(System.IntPtr adapterPtr, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(Vortice.DXGI.IDXGIAdapter adapter, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device, out Vortice.Direct3D.FeatureLevel featureLevel)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(Vortice.DXGI.IDXGIAdapter adapter, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device, out Vortice.Direct3D11.ID3D11DeviceContext immediateContext)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(System.IntPtr adapterPtr, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel featureLevel, out Vortice.Direct3D11.ID3D11Device device, out Vortice.Direct3D11.ID3D11DeviceContext immediateContext)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(System.IntPtr adapterPtr, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device, out Vortice.Direct3D11.ID3D11DeviceContext immediateContext)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(Vortice.DXGI.IDXGIAdapter adapter, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device, out Vortice.Direct3D.FeatureLevel featureLevel, out Vortice.Direct3D11.ID3D11DeviceContext immediateContext)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDevice(System.IntPtr adapterPtr, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, out Vortice.Direct3D11.ID3D11Device device, out Vortice.Direct3D.FeatureLevel featureLevel, out Vortice.Direct3D11.ID3D11DeviceContext immediateContext)
SIGNATURE=SharpGen.Runtime.Result Vortice.Direct3D11.D3D11.D3D11CreateDeviceAndSwapChain(Vortice.DXGI.IDXGIAdapter adapter, Vortice.Direct3D.DriverType driverType, Vortice.Direct3D11.DeviceCreationFlags flags, Vortice.Direct3D.FeatureLevel[] featureLevels, System.Nullable`1[[Vortice.DXGI.SwapChainDescription, Vortice.DXGI, Version=3.8.3.0, Culture=neutral, PublicKeyToken=5431ec61a7e925da]] swapChainDesc, out Vortice.DXGI.IDXGISwapChain swapChain, out Vortice.Direct3D11.ID3D11Device device, out System.Nullable`1[[Vortice.Direct3D.FeatureLevel, Vortice.DirectX, Version=3.8.3.0, Culture=neutral, PublicKeyToken=5431ec61a7e925da]] featureLevel, out Vortice.Direct3D11.ID3D11DeviceContext immediateContext)
D3D11_CREATE_DEVICE_SIGNATURES_END
DXGI_FACTORY_SIGNATURES_BEGIN
TYPE=Vortice.DXGI.DXGI
FILTER=CreateDXGIFactory
METHOD_COUNT=4
SIGNATURE= Vortice.DXGI.DXGI.CreateDXGIFactory1()
SIGNATURE=SharpGen.Runtime.Result Vortice.DXGI.DXGI.CreateDXGIFactory1(out  factory)
SIGNATURE= Vortice.DXGI.DXGI.CreateDXGIFactory2(System.Boolean debug)
SIGNATURE=SharpGen.Runtime.Result Vortice.DXGI.DXGI.CreateDXGIFactory2(System.Boolean debug, out  factory)
DXGI_FACTORY_SIGNATURES_END
DXGI_SWAPCHAIN_SIGNATURES_BEGIN
TYPE=Vortice.DXGI.IDXGIFactory2
FILTER=CreateSwapChainForHwnd
METHOD_COUNT=1
SIGNATURE=Vortice.DXGI.IDXGISwapChain1 Vortice.DXGI.IDXGIFactory2.CreateSwapChainForHwnd(SharpGen.Runtime.IUnknown device, System.IntPtr wnd, Vortice.DXGI.SwapChainDescription1 desc, System.Nullable`1[[Vortice.DXGI.SwapChainFullscreenDescription, Vortice.DXGI, Version=3.8.3.0, Culture=neutral, PublicKeyToken=5431ec61a7e925da]] fullscreenDesc, Vortice.DXGI.IDXGIOutput restrictToOutput)
DXGI_SWAPCHAIN_SIGNATURES_END
CASU_VORTICE_API_DISCOVERY=PASS

API_SIGNATURE_DISCOVERY=PASS

PRODUCTION_DEPLOYMENT=NO
CASU_ORCHESTRATOR_REPLACED=NO
WINDOWS_AUTH_CHANGED=NO
REBOOT=NO

STATUS=PASS
NEXT_TASK=CASU_THEME_01-I2-ARCH3B-R4
