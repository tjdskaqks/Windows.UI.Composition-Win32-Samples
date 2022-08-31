using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.Win32.System.WinRT;
using static Windows.Win32.PInvoke;

namespace Composition.WindowsRuntimeHelpers_NET6
{
    public static class CoreMessagingHelper
    {
        public static DispatcherQueueController CreateDispatcherQueueControllerForCurrentThread()
        {
            var options = new Windows.Win32.System.WinRT.DispatcherQueueOptions
            {
                dwSize = (uint)Marshal.SizeOf<Windows.Win32.System.WinRT.DispatcherQueueOptions>(),
                threadType = DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT,
                apartmentType = DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_NONE
            };
            CreateDispatcherQueueController(options, out DispatcherQueueController controller);
            return controller;
        }
    }
}
