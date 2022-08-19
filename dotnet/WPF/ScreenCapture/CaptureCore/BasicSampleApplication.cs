using Composition.WindowsRuntimeHelpers_NETStd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.UI.Composition;

namespace CaptureCore
{
    public class BasicSampleApplication : IDisposable
    {
        private bool disposedValue;
        private Compositor compositor;
        private ContainerVisual root;

        private SpriteVisual content;
        private CompositionSurfaceBrush brush;

        private IDirect3DDevice device;
        private BasicCapture capture;

        public Visual Visual => root;

        public BasicSampleApplication(Compositor c)
        {
            compositor = c;
            device = Direct3D11Helper.CreateDevice();

            // Setup the root.
            root = compositor.CreateContainerVisual();
            root.RelativeSizeAdjustment = Vector2.One;

            // Setup the content.
            brush = compositor.CreateSurfaceBrush();
            brush.HorizontalAlignmentRatio = 0.5f;
            brush.VerticalAlignmentRatio = 0.5f;
            brush.Stretch = CompositionStretch.Uniform;

            var shadow = compositor.CreateDropShadow();
            shadow.Mask = brush;

            content = compositor.CreateSpriteVisual();
            content.AnchorPoint = new Vector2(0.5f);
            content.RelativeOffsetAdjustment = new Vector3(0.5f, 0.5f, 0);
            content.RelativeSizeAdjustment = Vector2.One;
            content.Size = new Vector2(-80, -80);
            content.Brush = brush;
            content.Shadow = shadow;
            root.Children.InsertAtTop(content);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                    StopCapture();
                    compositor = null;
                    root?.Dispose();
                    content?.Dispose();
                    brush?.Dispose();
                    device?.Dispose();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~BasicSampleApplication()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void StartCaptureFromItem(GraphicsCaptureItem item)
        {
            StopCapture();
            capture = new BasicCapture(device, item);

            var surface = capture.CreateSurface(compositor);
            brush.Surface = surface;

            capture.StartCapture();
        }

        public void StopCapture()
        {
            capture?.Dispose();
            brush.Surface = null;
        }
    }
}
