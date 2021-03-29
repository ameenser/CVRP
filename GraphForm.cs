using CVRP_SOLVER;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class GraphForm : Form
    {
        public List<Costumer> nodes { get; set; }
        List<PointF> points;
        double vehicle_capacity, cost;
        public Solution sol { get; set; }
        int Height, Width;
        float zoom = 1.0F;
        public GraphForm()
        {
            InitializeComponent();
            points = new List<PointF>();
            
        }
        public void setVariables(Solution solution,List<Costumer> Nodes)
        {

            sol = solution;
            nodes = Nodes;
            canvas.Invalidate();
        }
        //public GraphForm()
        //{

        //   // cost = 0;
        //    InitializeComponent();
        //    canvas.AutoScroll = false;


           
            
        //    Height = canvas.Height;
        //    Width = canvas.Width;
        //    nodes = new List<Costumer>();
        //    Graph g1 = new Graph();   // Build a new graph 
        //    g1.parse_file(@"C:\Users\Ameen\Documents\visual studio 2015\Projects\WindowsFormsApplication2\G1.txt",0);
            
        //    points = new List<PointF>();
        //    nodes = g1.getNodes();
        //    foreach (Costumer c in nodes)
        //    {
        //        c.X *= 20;
        //        c.Y *= 20;
        //        c.X += canvas.Width / 2;
        //        c.Y += canvas.Height / 2;
        //        if (c.X < 0)
        //            c.X = 0 + 10;
        //        if (c.X > canvas.Width)
        //            c.X = canvas.Width - 10;

        //        if (c.Y < 0)
        //            c.Y = 0 + 10;
        //        if (c.Y > canvas.Height)
        //            c.Y = canvas.Height - 10;
        //        points.Add(new PointF(((float)c.X), ((float)c.Y)));
        //    }
        //    g1.nodes = nodes;
        //    sol = g1.random_solver();
        //    sol = Solution.opt_2_inter(sol);
        //    sol = Solution.opt_2(sol);
        //    sol = Solution.opt_2_inter(sol);
        //    sol = Solution.opt_2(sol);
        //    sol = Solution.opt_2_inter(sol);



        //}

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (nodes == null)
                return;
            Brush my_brush = new SolidBrush(Color.Red);
            Pen pen = new Pen(Color.Blue, 2);
            Graphics g = e.Graphics;
            //g.ScaleTransform(zoom, zoom);
          //  g.DrawLine(pen, new Point(0, 0), new Point(500, 500));
            foreach (Costumer c in nodes)
            {
                if (c.ID == 0)
                {
                    g.DrawString("Depot", new Font("Arial", 8), my_brush, new PointF((float)c.X, (float)c.Y));
                    RectangleF rect1 = new RectangleF(((float)c.X - 25), (float)(c.Y - 25), 50, 50);
                    g.DrawEllipse(pen, rect1);
                    continue;
                }
                g.DrawString(c.ID.ToString(), new Font("Arial", 8), my_brush, new PointF((float)c.X, (float)c.Y));
                RectangleF rect = new RectangleF((float)(c.X - 5), (float)(c.Y - 5), 10, 10);
                g.DrawEllipse(pen, rect);
            }
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
                        float X2 = (float)r.Nodes[i + 1].X;
                        float Y2 = (float)r.Nodes[i + 1].Y;
                        float X1 = (float)r.Nodes[i ].X;
                        float Y1 = (float)r.Nodes[i ].Y;
                         X2 *= 20;
                         Y2 *= 20;
                         X2 += 1200 / 2;
                         Y2 += 660 / 2;
                         if (X2 < 0)
                             X2 = 0 + 10;
                         if (X2 > 1200)
                             X2 = 1200 - 10;

                         if (Y2 < 0)
                             Y2 = 0 + 10;
                         if (Y2 > 660)
                             Y2 = 660 - 10;

                    if (i == 0)
                    {   
                        g.DrawLine(pen, new PointF((float)((X2 / Math.Sqrt(X2 * X2 + Y2 * Y2)) * 25) + canvas.Width / 2,
                             (float)((Y2 / Math.Sqrt(X2 * X2 + Y2 * Y2)) * 25) + canvas.Height / 2), new PointF(X1, Y1));
                        continue;
                    }
                    //if (i == r.Nodes.Count - 2)
                    //{
                    //    g.DrawLine(pen, new PointF(r.Nodes[i].X, r.Nodes[i].Y),
                    //        new PointF(25 * (float)(r.Nodes[i].X / (Math.Sqrt(Math.Pow(r.Nodes[i].X, 2) + Math.Pow(r.Nodes[i].Y, 2)))),
                    //        25 * (float)(r.Nodes[i].Y / (Math.Sqrt(Math.Pow(r.Nodes[i].X, 2) + Math.Pow(r.Nodes[i].Y, 2)))))
                    //        );
                    //    continue;
                    //}
                    g.DrawLine(pen, new PointF(X1, Y1), new PointF(X2, Y2));

                }
                j++;
            }
        }
    }
}
