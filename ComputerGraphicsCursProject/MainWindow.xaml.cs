using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;


namespace ComputerGraphicsCursProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // private System.Windows.Forms.Form form = new Form();
       private PictureBox form = new PictureBox();
        public MainWindow()
        {
            InitializeComponent();


            dataPointCount = 0;
            withMarkers = checkBoxOfMerkersEnabled.IsPressed;
            
            //panelOfApproximation.Enabled = checkBoxOfMerkersEnabled.Checked;
            //panelOfDataPoints.Enabled = checkBoxOfMerkersEnabled.Checked;
            textBoxOfNumberOfDrawPoints.Text = Convert.ToString(40);

            dataPoints1 = new Point[4];
            Point point11 = new Point(0, 0, 0, 1); // точки дефолтные 
            Point point12 = new Point(1, -1, 0, 1);
            Point point14 = new Point(3, -1, 0, 1);

            dataPoints1[0] = point11;
            dataPoints1[1] = point12;
            dataPoints1[2] = point14;

            dataPoints2 = new Point[4];
            Point point21 = new Point(0, 0, 1, 1);
            Point point22 = new Point(1, -1, 1, 1);
            Point point24 = new Point(3, -1, 1, 1);

            dataPoints2[0] = point21;
            dataPoints2[1] = point22;
            dataPoints2[2] = point24;

            dataPoints3 = new Point[4];
            Point point31 = new Point(0, 0, 1, 1);
            Point point32 = new Point(1, 1, 1, 1);
            Point point34 = new Point(0, 0, 0, 1);

            dataPoints3[0] = point31;
            dataPoints3[1] = point32;
            dataPoints3[2] = point34;

            dataPoints4 = new Point[4];
            Point point41 = new Point(3, -1, 1, 1);
            Point point42 = new Point(3, 1, 1, 1);
            Point point44 = new Point(3, -1, 0, 1);

            dataPoints4[0] = point41;
            dataPoints4[1] = point42;
            dataPoints4[2] = point44;

            this.CalcCurves();

            mx = 0;
            my = 0;
            cx = 0;
            cy = 0;

            scale = 100;
            mashtabK = 0;

            isMouseDown = false;
        }
        private void HostInitialized(object sender, EventArgs e)
        {
            ((WindowsFormsHost)sender).Child = form;
            Host.Child.Paint += Form1_Paint;
            Host.Child.MouseDown += Form1_MouseDown;
            Host.Child.MouseMove += Form1_MouseMove;
            Host.Child.MouseUp += Form1_MouseUp;
            Host.Child.SizeChanged += Form1_SizeChanged;
            
        }

        private void CalcCurves()
        {
            bezierCurve1 = new BezierCurve(dataPoints1, int.Parse(textBoxOfNumberOfDrawPoints.Text));
            bezierCurve2 = new BezierCurve(dataPoints2, int.Parse(textBoxOfNumberOfDrawPoints.Text));
            bezierCurve3 = new BezierCurve(dataPoints3, int.Parse(textBoxOfNumberOfDrawPoints.Text));
            bezierCurve4 = new BezierCurve(dataPoints4, int.Parse(textBoxOfNumberOfDrawPoints.Text));

            kuntzSurface = new RuledSurface(bezierCurve1, bezierCurve2, bezierCurve3, bezierCurve4);
        }

        private void mashtabMinusButton_Click(object sender, EventArgs e)
        {
            mashtabK--;
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void mashtabPlusButton_Click(object sender, EventArgs e)
        {
            mashtabK++;
            this.Host.Child.Refresh();
            //ExtensionMethods.Refresh(this);
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            double zoomLevel = (scale + mashtabK) / 1000;
            double coeff = Math.Max(e.ClipRectangle.Width, e.ClipRectangle.Height) * zoomLevel;

            ShiftMatrix sh = new ShiftMatrix(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2, 0);
            ScalingMatrix sc = new ScalingMatrix(coeff, coeff, coeff);
            RotationMatrix rtx = new RotationMatrix('X', my * Math.PI / 180.0);
            RotationMatrix rty = new RotationMatrix('Y', -mx * Math.PI / 180.0);
            Matrix preobr = sh * rtx * rty * sc;

            kuntzSurface.Draw(preobr, e.Graphics, dataPointCount, withMarkers);
            // else linearSurface.Draw(preobr, e.Graphics, dataPointCount, withMarkers);

        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown)
            {
                int deltaX = e.X - cx;
                int deltaY = e.Y - cy;
                mx += deltaX;
                my += deltaY;
                cx = e.X;
                cy = e.Y;
                form.Refresh();
            }
        }

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = true;
            cx = e.X;
            cy = e.Y;
        }

        private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //this.Refresh();
            form.Refresh();
        }

        private void buttonOfApply1_Click(object sender, EventArgs e)
        {
            this.CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 3) dataPoints1[dataPointCount].x += 1;
            if (dataPointCount > 2 && dataPointCount < 6) dataPoints2[dataPointCount - 3].x += 1;
            if (dataPointCount > 5 && dataPointCount < 9) dataPoints3[dataPointCount - 6].x += 1;
            if (dataPointCount > 8 && dataPointCount < 12) dataPoints4[dataPointCount - 9].x += 1;

            if (dataPointCount == 0) dataPoints3[dataPointCount + 2].x += 1;
            if (dataPointCount == 2) dataPoints4[dataPointCount].x += 1;
            if (dataPointCount == 5) dataPoints4[dataPointCount - 5].x += 1;
            if (dataPointCount == 3) dataPoints3[dataPointCount - 3].x += 1;

            if (dataPointCount == 8) dataPoints1[dataPointCount - 8].x += 1;
            if (dataPointCount == 6) dataPoints2[dataPointCount - 6].x += 1;
            if (dataPointCount == 9) dataPoints2[dataPointCount - 7].x += 1;

            if (dataPointCount == 11) dataPoints1[dataPointCount - 9].x += 1;

            CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 3) dataPoints1[dataPointCount].x -= 1;
            if (dataPointCount > 2 && dataPointCount < 6) dataPoints2[dataPointCount - 3].x -= 1;
            if (dataPointCount > 5 && dataPointCount < 9) dataPoints3[dataPointCount - 6].x -= 1;
            if (dataPointCount > 8 && dataPointCount < 12) dataPoints4[dataPointCount - 9].x -= 1;

            if (dataPointCount == 0) dataPoints3[dataPointCount + 2].x -= 1;
            if (dataPointCount == 2) dataPoints4[dataPointCount].x -= 1;
            if (dataPointCount == 5) dataPoints4[dataPointCount - 5].x -= 1;
            if (dataPointCount == 3) dataPoints3[dataPointCount - 3].x -= 1;

            if (dataPointCount == 8) dataPoints1[dataPointCount - 8].x -= 1;
            if (dataPointCount == 6) dataPoints2[dataPointCount - 6].x -= 1;
            if (dataPointCount == 9) dataPoints2[dataPointCount - 7].x -= 1;

            if (dataPointCount == 11) dataPoints1[dataPointCount - 9].x -= 1;

            CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void buttonNaNas_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 3) dataPoints1[dataPointCount].z -= 1;
            if (dataPointCount > 2 && dataPointCount < 6) dataPoints2[dataPointCount - 3].z -= 1;
            if (dataPointCount > 5 && dataPointCount < 9) dataPoints3[dataPointCount - 6].z -= 1;
            if (dataPointCount > 8 && dataPointCount < 12) dataPoints4[dataPointCount - 9].z -= 1;

            if (dataPointCount == 0) dataPoints3[dataPointCount + 2].z -= 1;
            if (dataPointCount == 2) dataPoints4[dataPointCount].z -= 1;
            if (dataPointCount == 5) dataPoints4[dataPointCount - 5].z -= 1;
            if (dataPointCount == 3) dataPoints3[dataPointCount - 3].z -= 1;

            if (dataPointCount == 8) dataPoints1[dataPointCount - 8].z -= 1;
            if (dataPointCount == 6) dataPoints2[dataPointCount - 6].z -= 1;
            if (dataPointCount == 9) dataPoints2[dataPointCount - 7].z -= 1;

            if (dataPointCount == 11) dataPoints1[dataPointCount - 9].z -= 1;

            CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void buttonOtNas_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 3) dataPoints1[dataPointCount].z += 1;
            if (dataPointCount > 2 && dataPointCount < 6) dataPoints2[dataPointCount - 3].z += 1;
            if (dataPointCount > 5 && dataPointCount < 9) dataPoints3[dataPointCount - 6].z += 1;
            if (dataPointCount > 8 && dataPointCount < 12) dataPoints4[dataPointCount - 9].z += 1;

            if (dataPointCount == 0) dataPoints3[dataPointCount + 2].z += 1;
            if (dataPointCount == 2) dataPoints4[dataPointCount].z += 1;
            if (dataPointCount == 5) dataPoints4[dataPointCount - 5].z += 1;
            if (dataPointCount == 3) dataPoints3[dataPointCount - 3].z += 1;

            if (dataPointCount == 8) dataPoints1[dataPointCount - 8].z += 1;
            if (dataPointCount == 6) dataPoints2[dataPointCount - 6].z += 1;
            if (dataPointCount == 9) dataPoints2[dataPointCount - 7].z += 1;

            if (dataPointCount == 11) dataPoints1[dataPointCount - 9].z += 1;

            CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 3) dataPoints1[dataPointCount].y -= 1;
            if (dataPointCount > 2 && dataPointCount < 6) dataPoints2[dataPointCount - 3].y -= 1;
            if (dataPointCount > 5 && dataPointCount < 9) dataPoints3[dataPointCount - 6].y -= 1;
            if (dataPointCount > 8 && dataPointCount < 12) dataPoints4[dataPointCount - 9].y -= 1;

            if (dataPointCount == 0) dataPoints3[dataPointCount + 2].y -= 1;
            if (dataPointCount == 2) dataPoints4[dataPointCount].y -= 1;
            if (dataPointCount == 5) dataPoints4[dataPointCount - 5].y -= 1;
            if (dataPointCount == 3) dataPoints3[dataPointCount - 3].y -= 1;

            if (dataPointCount == 8) dataPoints1[dataPointCount - 8].y -= 1;
            if (dataPointCount == 6) dataPoints2[dataPointCount - 6].y -= 1;
            if (dataPointCount == 9) dataPoints2[dataPointCount - 7].y -= 1;

            if (dataPointCount == 11) dataPoints1[dataPointCount - 9].y -= 1;

            CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 3) dataPoints1[dataPointCount].y += 1;
            if (dataPointCount > 2 && dataPointCount < 6) dataPoints2[dataPointCount - 3].y += 1;
            if (dataPointCount > 5 && dataPointCount < 9) dataPoints3[dataPointCount - 6].y += 1;
            if (dataPointCount > 8 && dataPointCount < 12) dataPoints4[dataPointCount - 9].y += 1;

            if (dataPointCount == 0) dataPoints3[dataPointCount + 2].y += 1;
            if (dataPointCount == 2) dataPoints4[dataPointCount].y += 1;
            if (dataPointCount == 5) dataPoints4[dataPointCount - 5].y += 1;
            if (dataPointCount == 3) dataPoints3[dataPointCount - 3].y += 1;

            if (dataPointCount == 8) dataPoints1[dataPointCount - 8].y += 1;
            if (dataPointCount == 6) dataPoints2[dataPointCount - 6].y += 1;
            if (dataPointCount == 9) dataPoints2[dataPointCount - 7].y += 1;

            if (dataPointCount == 11) dataPoints1[dataPointCount - 9].y += 1;

            CalcCurves();
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        // текущие координаты курсора и координаты его предыдущего положения

        int mx, my, cx, cy;

        // масштаб

        float scale;
        double mashtabK;

        Point[] dataPoints1;
        Point[] dataPoints2;
        Point[] dataPoints3;
        Point[] dataPoints4;
        int dataPointCount;

        bool isMouseDown;

        BezierCurve bezierCurve1;
        BezierCurve bezierCurve2;
        BezierCurve bezierCurve3;
        BezierCurve bezierCurve4;


        RuledSurface kuntzSurface;

        // LinearSurface linearSurface;
        bool withMarkers;

        private void buttonOfDataPointCounter_Click(object sender, EventArgs e)
        {
            if (dataPointCount < 12)
            {
                dataPointCount++;
                if (dataPointCount == 12)
                {
                    buttonDown.IsEnabled = false;
                    buttonUp.IsEnabled = false;
                    buttonRight.IsEnabled = false;
                    buttonLeft.IsEnabled = false;
                    buttonNaNas.IsEnabled = false;
                    buttonOtNas.IsEnabled = false;
                }
            }
            else
            {
                dataPointCount = 0;
                buttonDown.IsEnabled = true;
                buttonUp.IsEnabled = true;
                buttonRight.IsEnabled = true;
                buttonLeft.IsEnabled = true;
                buttonNaNas.IsEnabled = true;
                buttonOtNas.IsEnabled = true;
            }
            form.Refresh();
            // ExtensionMethods.Refresh(this);
        }

        private void checkBoxOfMerkersEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOfMerkersEnabled.IsPressed)
            {
                checkBoxOfMerkersEnabled.Background = Brushes.GreenYellow;
            }
            else
            {
                checkBoxOfMerkersEnabled.Background = Brushes.Aquamarine;
            }

            //panelOfApproximation.Enabled = checkBoxOfMerkersEnabled.Checked;
            //panelOfDataPoints.Enabled = checkBoxOfMerkersEnabled.Checked;
            withMarkers = checkBoxOfMerkersEnabled.IsPressed;
            form.Refresh();
            //ExtensionMethods.Refresh(this);
        }

        public static class ExtensionMethods
        {
            private static readonly Action EmptyDelegate = delegate { };

            public static void Refresh(UIElement uiElement)
            {
                uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
        }
    }
}