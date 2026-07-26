using System;
using System.Drawing;
using System.Windows.Forms;

namespace BnSBuddy2.Packets
{
    public class CircularProgressBar : Control
    {
        private int _value = 0;
        private readonly Timer _timer;

        public CircularProgressBar()
        {
            DoubleBuffered = true;
            Size = new Size(100, 100);

            _timer = new Timer { Interval = 50 };
            _timer.Tick += (s, e) => { _value = (_value + 1) % 100; Invalidate(); };
            _timer.Start();
        }

        public Color ProgressColor { get; set; } = Color.Blue;
        public float LineWidth { get; set; } = 10f;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float padding = LineWidth / 2f; // Half the line width to keep the stroke inside
            float diameter = Math.Min(Width, Height) - LineWidth; // Account for full line width
            RectangleF rect = new RectangleF(padding, padding, diameter, diameter);

            using (var pen = new Pen(ProgressColor, LineWidth)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            })
            {
                e.Graphics.DrawArc(pen, rect, -90, 360f * _value / 100);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Height = Width;
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
