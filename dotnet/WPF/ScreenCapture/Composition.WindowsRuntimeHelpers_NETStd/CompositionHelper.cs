﻿//using InteropCompositor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace Composition.WindowsRuntimeHelpers_NETStd
{
    public static class CompositionHelper
    {
        [ComImport]
        [Guid("25297D5C-3AD4-4C9C-B5CF-E36A38512330")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        interface ICompositorInterop
        {
            ICompositionSurface CreateCompositionSurfaceForHandle(IntPtr swapChain);

            ICompositionSurface CreateCompositionSurfaceForSwapChain(IntPtr swapChain);

            CompositionGraphicsDevice CreateGraphicsDevice(IntPtr renderingDevice);
        }

        [ComImport]
        [Guid("29E691FA-4567-4DCA-B319-D0F207EB6807")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        interface ICompositorDesktopInterop
        {
            Windows.UI.Composition.Desktop.DesktopWindowTarget CreateDesktopWindowTarget(IntPtr hwnd, bool isTopmost);
            //void CreateDesktopWindowTarget(IntPtr hwndTarget, bool isTopmost, out ICompositionTarget test);
        }

        public static CompositionTarget CreateDesktopWindowTarget(this Compositor compositor, IntPtr hwnd, bool isTopmost)
        {
            //interop.CreateDesktopWindowTarget(hwnd, true, out var target).ThrowOnError();
            //ICompositionTarget compositionTarget = (ICompositionTarget)target;

            //var desktopInterop = compositor.TryAs<ICompositorDesktopInterop>();
            //return desktopInterop.CreateDesktopWindowTarget(hwnd, isTopmost);

            var desktopInterop = (ICompositorDesktopInterop)(object)compositor;
            return desktopInterop.CreateDesktopWindowTarget(hwnd, isTopmost);
        }

        public static ICompositionSurface CreateCompositionSurfaceForSwapChain(this Compositor compositor, SharpDX.DXGI.SwapChain1 swapChain)
        {
            var interop = (ICompositorInterop)(object)compositor;
            return interop.CreateCompositionSurfaceForSwapChain(swapChain.NativePointer);
        }
    }
}
