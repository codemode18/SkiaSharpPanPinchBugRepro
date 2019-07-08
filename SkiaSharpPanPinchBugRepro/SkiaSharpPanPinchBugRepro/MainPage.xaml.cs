using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SkiaSharpPanPinchBugRepro
{
    public partial class MainPage : ContentPage
    {
        private readonly Dictionary<long, SKPath> _temporaryPaths = new Dictionary<long, SKPath>();
        private readonly List<SKPath> _paths = new List<SKPath>();

        public MainPage()
        {
            InitializeComponent();
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchGestureOnPinchUpdated;
            pinchPanContentView.GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanGestureOnPanUpdated;
            pinchPanContentView.GestureRecognizers.Add(panGesture);
        }

        private void PanGestureOnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
        }

        private void PinchGestureOnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {

        }

        private void CanvasView_OnTouch(object sender, SKTouchEventArgs e)
        {
            e.Handled = true;

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    _temporaryPaths[e.Id] = p;
                    break;

                case SKTouchAction.Moved:
                    if (e.InContact)
                    {
                        _temporaryPaths[e.Id].QuadTo(_temporaryPaths[e.Id].LastPoint.X, _temporaryPaths[e.Id].LastPoint.Y, (e.Location.X + _temporaryPaths[e.Id].LastPoint.X) / 2, (e.Location.Y + _temporaryPaths[e.Id].LastPoint.Y) / 2);
                    }
                    break;

                case SKTouchAction.Released:
                    _paths.Add(_temporaryPaths[e.Id]);
                    _temporaryPaths.Remove(e.Id);
                    break;

                case SKTouchAction.Cancelled:
                    _temporaryPaths.Remove(e.Id);
                    break;
            }

            canvasView.InvalidateSurface();
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Aqua);

            var pen = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                Color = SKColors.Red,
                StrokeWidth = 8f
            };

            foreach (var touchPath in _temporaryPaths)
            {
                canvas.DrawPath(touchPath.Value, pen);
            }

            foreach (var touchPath in _paths)
            {
                canvas.DrawPath(touchPath, pen);
            }
        }
    }
}
