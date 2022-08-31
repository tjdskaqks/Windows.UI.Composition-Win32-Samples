using CaptureCore_NET6;
using Composition.WindowsRuntimeHelpers_NET6;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Foundation.Metadata;
using Windows.Graphics.Capture;
using Windows.UI.Composition;

namespace ScreenCaptureDemo_NET6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr hwnd = IntPtr.Zero;
        private Compositor compositor;
        private CompositionTarget target;
        private ContainerVisual root;

        private BasicSampleApplication sample;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var interopWindow = new WindowInteropHelper(this);
            hwnd = interopWindow.Handle;

            var presentationSource = PresentationSource.FromVisual(this);
            double dpiX = 1.0;
            double dpiY = 1.0;
            if (presentationSource != null)
            {
                dpiX = presentationSource.CompositionTarget.TransformToDevice.M11;
                dpiY = presentationSource.CompositionTarget.TransformToDevice.M22;
            }
            var controlsWidth = (float)(ControlsGrid.ActualWidth * dpiX);
            var controlsHeight = (float)(ControlsGrid.ActualHeight * dpiY);
            InitCompositionHeight(controlsHeight);
        }

        private void InitCompositionWidth(float controlsWidth)
        {
            // Create the compositor.
            compositor = new Windows.UI.Composition.Compositor();

            // Create a target for the window.
            target = compositor.CreateDesktopWindowTarget((Windows.Win32.Foundation.HWND)hwnd, true);

            // Attach the root visual.
            root = compositor.CreateContainerVisual();
            root.RelativeSizeAdjustment = Vector2.One;
            root.Size = new Vector2(-controlsWidth, 0);
            root.Offset = new Vector3(controlsWidth, 0, 0);
            target.Root = root;

            // Setup the rest of the sample application.
            sample = new BasicSampleApplication(compositor);
            root.Children.InsertAtTop(sample.Visual);
        }

        private void InitCompositionHeight(float controlsHeight)
        {
            // Create the compositor.
            compositor = new Compositor();

            // Create a target for the window.
            target = compositor.CreateDesktopWindowTarget((Windows.Win32.Foundation.HWND)hwnd, true);

            // Attach the root visual.
            root = compositor.CreateContainerVisual();
            root.RelativeSizeAdjustment = Vector2.One;
            root.Size = new Vector2(0, -controlsHeight);
            root.Offset = new Vector3(0, controlsHeight, 0);
            target.Root = root;

            // Setup the rest of the sample application.
            sample = new BasicSampleApplication(compositor);
            root.Children.InsertAtTop(sample.Visual);
        }

        private void Cbb_Processes_DropDownOpened(object sender, EventArgs e)
        {
            cbb_Processes.ItemsSource = GetProcesses();
        }

        private void Cbb_Processes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = cbb_Processes.SelectedIndex;
            if (index > -1)
            {
                var findProcess = cbb_Processes.Items[index] as FindProcess;
                if (findProcess != null)
                {
                    StopCapture();

                    if (findProcess.CaptureType.Equals(CaptureTypeEnum.Not))
                    {

                    }
                    else if (findProcess.CaptureType.Equals(CaptureTypeEnum.Desktop))
                    {
                        var hmon = findProcess.Handle;
                        try
                        {
                            StartHmonCapture(hmon);
                        }
                        catch (Exception)
                        {
                            Debug.WriteLine($"Hmon 0x{hmon.ToInt32():X8} is not valid for capture!");
                        }
                    }
                    else if (findProcess.CaptureType.Equals(CaptureTypeEnum.Program))
                    {
                        var hwnd = findProcess.Handle;
                        try
                        {
                            StartHwndCapture(hwnd);
                        }
                        catch (Exception)
                        {
                            Debug.WriteLine($"Hwnd 0x{hwnd.ToInt32():X8} is not valid for capture!");
                        }
                    }
                }
            }
        }

        public static List<FindProcess> GetProcesses()
        {
            var list = new List<FindProcess>();
            list.Add(new FindProcess() { ProcessNanme = "공유 종료", CaptureType = CaptureTypeEnum.Not });

            if (ApiInformation.IsApiContractPresent(typeof(Windows.Foundation.UniversalApiContract).FullName, 8))
            {
                var monitors = MonitorEnumerationHelper.GetMonitors();
                list.AddRange(monitors.OrderBy(p => p.DeviceName).Select(monitor => new FindProcess() { ProcessNanme = monitor.DeviceName, Handle = monitor.Hmon, CaptureType = CaptureTypeEnum.Desktop }));
            }

            if (ApiInformation.IsApiContractPresent(typeof(Windows.Foundation.UniversalApiContract).FullName, 8))
            {
                string[] notProcess = new string[] { "계산기", "NVIDIA GeForce Overlay", "Microsoft Text Input Application", "설정" };

                try
                {
                    var processesWithWindows = from p in Process.GetProcesses()
                                               where !string.IsNullOrWhiteSpace(p.MainWindowTitle) && WindowEnumerationHelper.IsWindowValidForCapture(p.MainWindowHandle)
                                               select p;

                    list.AddRange(processesWithWindows.Where(process => !notProcess.Contains(process.MainWindowTitle)).Select(process => new FindProcess() { ProcessNanme = process.MainWindowTitle, Handle = process.MainWindowHandle, Pid = process.Id, CaptureType = CaptureTypeEnum.Program }));
                }
                catch (Exception)
                {

                    
                }
                
            }

            return list;
        }

        private async Task StartPickerCaptureAsync()
        {
            var picker = new GraphicsCapturePicker();
            picker.SetWindow((Windows.Win32.Foundation.HWND)hwnd);
            GraphicsCaptureItem item = await picker.PickSingleItemAsync();

            if (item != null)
            {
                sample.StartCaptureFromItem(item);
            }
        }

        private void StartHwndCapture(IntPtr hwnd)
        {
            GraphicsCaptureItem item = CaptureHelper.CreateItemForWindow((Windows.Win32.Foundation.HWND)hwnd);
            if (item != null)
            {
                sample.StartCaptureFromItem(item);
            }
        }

        private void StartHmonCapture(IntPtr hmon)
        {
            GraphicsCaptureItem item = CaptureHelper.CreateItemForMonitor((Windows.Win32.Graphics.Gdi.HMONITOR)hmon);
            if (item != null)
            {
                sample.StartCaptureFromItem(item);
            }
        }

        private void StartPrimaryMonitorCapture()
        {
            MonitorInfo monitor = (from m in MonitorEnumerationHelper.GetMonitors()
                                   where m.IsPrimary
                                   select m).First();
            StartHmonCapture(monitor.Hmon);
        }

        private void StopCapture()
        {
            sample.StopCapture();
        }
    }


    public class FindProcess
    {
        public IntPtr Handle;
        public int Pid;
        public string ProcessNanme;
        public CaptureTypeEnum CaptureType;

        public override string ToString() => $"{ProcessNanme}";
    }

    public enum CaptureTypeEnum
    {
        Not,
        Desktop,
        Program
    }
}
