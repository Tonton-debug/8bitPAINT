using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для MyCell.xaml
    /// </summary>
    public partial class MyCell : UserControl
    {
        public MyCell()
        {
            InitializeComponent();
        }
        public void ChangeCell(int size,float get_StrokeThickness)
        {
            myRectangle.StrokeThickness = get_StrokeThickness;
            myPen.Thickness = get_StrokeThickness;
            DrawingBrushCell.Viewport = new Rect(0, 0, size, size);
            FirstLineGeometry.EndPoint = new Point(0,size);
            TwoLineGeometry.StartPoint = new Point(0, size);
            TwoLineGeometry.EndPoint = new Point(size, size);

        }
    }
}
