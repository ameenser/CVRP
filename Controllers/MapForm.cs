using CVRP_SOLVER;
using CVRP_SOLVER.CODE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MA_EAX_CVRP_SOLVER.GUI
{
    public partial class MapForm : Form
    {
        public List<Costumer> nodes { get; set; }
        public Solution sol { get; set; }
        Graphics drawArea;
        
        public MapForm()
        {
            InitializeComponent();
        }
        public MapForm(Solution bestSolution, List<Costumer> costumers)
        {
            InitializeComponent();
            this.Text = "Urban Postal Traffic - Map View";
            drawArea = drawingArea.CreateGraphics();
            sol = bestSolution;
            nodes = costumers;
        }

        private void DrawRoutesOnClick(object sender, EventArgs e)
        {
            if (sol != null)
            {
                if (nodes == null || sol == null)
                    return;
                Pen pen = new Pen(Color.Blue, 2);
                Random rand = new Random();
                Color[] colors = new Color[sol.Routes.Count()];
                for (int i = 0; i < sol.Routes.Count(); i++)
                {
                    colors[i] = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
                }
                int j = 0;
                foreach (Route r in sol.Routes)
                {
                    pen.Color = colors[j];
                    for (int i = 0; i < r.Nodes.Count - 1; i++)
                    {
                        float X2 = (drawingArea.Width / 2) + (float)r.Nodes[i + 1].X * 20;
                        float Y2 = (drawingArea.Height / 2) + (float)r.Nodes[i + 1].Y * 20;
                        float X1 = (drawingArea.Width / 2) + (float)r.Nodes[i].X * 20;
                        float Y1 = (drawingArea.Height / 2) + (float)r.Nodes[i].Y * 20;
                        drawArea.DrawLine(pen, new PointF(X1, Y1), new PointF(X2, Y2));

                    }
                    j++;
                }
            }else
            {
                MessageBox.Show("You have to wait until the algorith finishs");

            }
        }
        private void drawCustomersOnClick(object sender, EventArgs e)
        {
            double Xcor,Ycor;

            Brush my_brush = new SolidBrush(Color.Red);
            Pen pen = new Pen(Color.Blue, 2);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
           if (nodes == null)
                return;

            foreach (Costumer c in nodes)
            {
                Xcor = (drawingArea.Width / 2) + (c.X * 20);
                Ycor = (drawingArea.Height / 2) + (c.Y * 20);

                if (c.ID == 0)
                {
                    drawArea.FillEllipse(blueBrush, drawingArea.Width / 2, drawingArea.Height / 2, 10, 10);
                    drawArea.DrawString("Depot", new Font("Arial", 8), my_brush, drawingArea.Width / 2 + 8, drawingArea.Height / 2 + 8);
                }
                else
                {
                    drawArea.FillEllipse(blueBrush, (float)Xcor, (float)Ycor, 5, 5);
                    drawArea.DrawString(c.ID.ToString(), new Font("Arial", 8), my_brush, (float)Xcor + 4, (float)Ycor + 4);
                }
            }
        }

        private void GraphForm_Load(object sender, EventArgs e)
        {

        }
    }
}
