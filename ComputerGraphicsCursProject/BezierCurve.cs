using System.Drawing;

namespace ComputerGraphicsCursProject
{
    internal class BezierCurve
    {
        private Point[] dataPoints;
        private readonly int NumberOfDrawPoints;

        public BezierCurve(Point[] points, int numberOfDrawPoints)
        {
            NumberOfDrawPoints = numberOfDrawPoints;
            dataPoints = points;
            Invalidate();
        }

        public Point[] DrawingPoints { get; private set; }

        public Point[] DataPoints //4 штуки
        {
            get => dataPoints;
            set
            {
                dataPoints = value;
                Invalidate();
            }
        }

        public Point this[int i]
        {
            get => dataPoints[i];
            set
            {
                dataPoints[i] = value;
                Invalidate();
            }
        }

        public static BezierCurve operator *(Matrix m, BezierCurve b)
        {
            var dataPoints1 = new Point[3];
            for (var i = 0; i < 3; i++)
                dataPoints1[i] = m * b.DataPoints[i];

            return new BezierCurve(dataPoints1, b.NumberOfDrawPoints);
        }

        public void Invalidate()
        {
            DrawingPoints = new Point[NumberOfDrawPoints + 1];
            double dt = 1f / NumberOfDrawPoints;
            double t = 0f;
            for (var i = 0; i <= NumberOfDrawPoints; i++)
            {
                DrawingPoints[i] = B(t);
                t += dt;
            }
        }

        public Point B(double t)
        {
            var c0 = (1 - t) * (1 - t);
            var c1 = (1 - t) * 2 * t;
            var c2 = t * t;
            var x = c0 * dataPoints[0].x + c1 * dataPoints[1].x + c2 * dataPoints[2].x;
            var y = c0 * dataPoints[0].y + c1 * dataPoints[1].y + c2 * dataPoints[2].y;
            var z = c0 * dataPoints[0].z + c1 * dataPoints[1].z + c2 * dataPoints[2].z;
            return new Point(x, y, z, 1);
        }

        public void Draw(Matrix preobr, Graphics g)
        {
            var pen = new Pen(Color.Red, 2f);
            BezierCurve b;
            b = preobr * this;

            for (var i = 0; i < DrawingPoints.Length - 1; i++)
                g.DrawLine(pen, (int) b.DrawingPoints[i].x, (int) b.DrawingPoints[i].y, (int) b.DrawingPoints[i + 1].x,
                    (int) b.DrawingPoints[i + 1].y);
        }

        public void DrawMarkers(Matrix preobr, Graphics g)
        {
            var b = preobr * this;

            for (var i = 0; i < 3; i++)
            {
                var rectangle = new RectangleF((int) b.dataPoints[i].x - 5, (int) b.dataPoints[i].y - 5,
                    10, 10);
                g.FillEllipse(Brushes.Black, rectangle);
            }
        }
    }
}