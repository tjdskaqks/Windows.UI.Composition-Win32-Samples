using System.Runtime.InteropServices;
using Windows.UI.Composition;
using WinRT;

namespace Composition.WindowsRuntimeHelpers_NET6
{
    public static class CompositionHelper
    {
        public static CompositionTarget CreateDesktopWindowTarget(this Compositor compositor, Windows.Win32.Foundation.HWND hwnd, bool isTopmost)
        {
            var desktopInterop = compositor.As<Windows.Win32.System.WinRT.Composition.ICompositorDesktopInterop>();
            desktopInterop.CreateDesktopWindowTarget(hwnd, isTopmost, out var target);
            return target;
        }

        public static ICompositionSurface CreateCompositionSurfaceForSwapChain(this Compositor compositor, Windows.Win32.Graphics.Dxgi.IDXGISwapChain1 swapChain)
        {
            var interop = compositor.As<Windows.Win32.System.WinRT.Composition.ICompositorInterop>();
            interop.CreateCompositionSurfaceForSwapChain(swapChain, out var raw);
            var rawPtr = Marshal.GetIUnknownForObject(raw);
            var result = MarshalInterface<ICompositionSurface>.FromAbi(rawPtr);
            Marshal.Release(rawPtr);
            return result;
        }
    }
}