using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace PamukkyDesktopClient.Views
{
	public partial class MediaViewer : Window
	{
		bool dragging = false;
		Point oldpos = new Point(0, 0);
		double zoomPrct = 1;
		double x = 0;
        double y = 0;
		private bool _mouseDownForWindowMoving = false;
		private PointerPoint _originalPoint;
		public MediaViewer()
		{
			InitializeComponent();
            closebtn.Click += (e, a) => {
                Close();
            };
			
			tb.PointerPressed += (a,e) => {
				if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen) return;

				_mouseDownForWindowMoving = true;
				_originalPoint = e.GetCurrentPoint(this);
			};
			
			tb.PointerMoved += (a,e) => {
				if (!_mouseDownForWindowMoving) return;

				PointerPoint currentPoint = e.GetCurrentPoint(this);
				Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
					Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
			};
			tb.PointerReleased += (e,a) => {
				_mouseDownForWindowMoving = false;
			};
			
			LayoutUpdated += (e,a) => {try {tick();}catch{}};
			
			mxbtn.Click += (e,a) => {
				if (WindowState == WindowState.FullScreen) {
					WindowState = WindowState.Normal;
				}else {
					WindowState = WindowState.FullScreen;
				}
			};

            cv.PointerWheelChanged += (object? sender, Avalonia.Input.PointerWheelEventArgs e) => {
				if (e.Delta.Y > 0)
				{
					zoomPrct = zoomPrct + 0.1;
				}
				else if (e.Delta.Y < 0)
				{
					zoomPrct = zoomPrct - 0.1;
                    if (zoomPrct < 0.1)
                    {
                        zoomPrct = 0.1;
                    }
				}
                tick();
            };
			cv.PointerPressed += (s, e) => { dragging = true; oldpos = e.GetPosition(cv); };
			cv.PointerReleased += (s, e) => dragging = false;
            cv.PointerMoved += (object? sender, Avalonia.Input.PointerEventArgs e) =>
            {
                Point currentpos = e.GetPosition(cv);
                if (dragging)
                {
                    var diff = oldpos - currentpos;
                    x = x - diff.X;
                    y = y - diff.Y;
					tick();
                }
                oldpos = currentpos;
            };
        }

		public void tick()
		{
            Size imagesize = image.Source.Size;
            if (imagesize.Width * (double)zoomPrct < cv.Bounds.Width)
            {
                x = (cv.Bounds.Width / 2) - ((imagesize.Width * (double)zoomPrct) / 2);
            }else {
				//if (x < 0) {x = 0;}
				//if (x > imagesize.Width - Width) {
				//	x = imagesize.Width - Width;
				//}
			}
            if (imagesize.Height * (double)zoomPrct < cv.Bounds.Height)
            {
                y = (cv.Bounds.Height / 2) - ((imagesize.Height * (double)zoomPrct) / 2);
            }else {
				//if (y < 0) {y = 0;}
				//if (x > imagesize.Height - Height) {
				//	x = imagesize.Height - Height;
				//}
			}
            posImg();
        }
        void posImg()
        {
            Size imagesize = image.Source.Size;
            image.Width = imagesize.Width * (double)zoomPrct;
            image.Height = imagesize.Height * (double)zoomPrct;
            Canvas.SetLeft(image, (double)x);
            Canvas.SetTop(image, (double)y);
        }
	}
}
