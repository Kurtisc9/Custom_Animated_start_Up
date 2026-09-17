using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using static Vortice.Direct3D11.D3D11;

internal static class Program
{
    private static int Main()
    {
        Console.WriteLine("CASU_DIRECT3D11_SMOKE_TEST=START");

        try
        {
            FeatureLevel[] requestedLevels =
            {
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };

            var result = D3D11CreateDevice(
                null,
                DriverType.Hardware,
                DeviceCreationFlags.None,
                requestedLevels,
                out ID3D11Device? device,
                out FeatureLevel featureLevel
            );

            if (result.Failure || device is null)
            {
                Console.WriteLine(
                    $"DIRECT3D11_DEVICE_CREATE=FAIL HRESULT={result.Code}"
                );

                return 1;
            }

            using (device)
            {
                Console.WriteLine("DIRECT3D11_DEVICE_CREATE=PASS");
                Console.WriteLine($"DIRECT3D_FEATURE_LEVEL={featureLevel}");
                Console.WriteLine("GPU_ACCELERATION=PASS");
            }

            Console.WriteLine("CASU_DIRECT3D11_SMOKE_TEST=PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("CASU_DIRECT3D11_SMOKE_TEST=FAIL");
            Console.WriteLine($"ERROR_TYPE={ex.GetType().FullName}");
            Console.WriteLine($"ERROR_MESSAGE={ex.Message}");
            return 2;
        }
    }
}
