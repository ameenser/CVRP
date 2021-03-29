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
    public partial class RoutesForm : Form
    {
        public Solution sol { get; set; }
        public RoutesForm()
        {
            
            InitializeComponent();
            label2.Text = "Demand :" + "\n# of Customers :";
            this.Text = "Urban Postal Traffic - Routes View";

        }
        public RoutesForm(Solution bestSolution)
        {
            InitializeComponent();
            label2.Text = "Demand :" + "\n# of Customers :";
            this.Text = "Urban Postal Traffic - Routes View";
            sol = bestSolution;
            RouteslistBox.SelectedValueChanged += new EventHandler(Listbox1_SelectedValueChanged);

            for (int i = 0;i < sol.Routes.Count();i++)
            {
                RouteslistBox.Items.Add("Route #" + (i+1) + "  " + Math.Round(sol.Routes[i].get_cost(),2) + "KM");
               
            }
            
        }

       public void Listbox1_SelectedValueChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            if (RouteslistBox.SelectedIndex >= sol.Routes.Count() || RouteslistBox.SelectedIndex < 0) return;
            List<Costumer> Customers = sol.Routes[RouteslistBox.SelectedIndex].Nodes;
            label2.Text = "Demand :" + sol.Routes[RouteslistBox.SelectedIndex].get_demand() + "/" + sol.Graph.vehicle_capacity
                +"\n# of Customers :" + (sol.Routes[RouteslistBox.SelectedIndex].Nodes.Count - 2);

            List<PointF> points = new List<PointF>();
            Dictionary<int, PointF> map = new Dictionary<int, PointF>();
            for (int i = 1; i < Customers.Count - 1; i++)
            {
                ListViewItem Item = new ListViewItem(Customers[i].ID.ToString());
                Item.SubItems.Add(""+Customers[i].X + "  "+ Customers[i].Y);
                Item.SubItems.Add(Customers[i].Demand.ToString());
                listView1.Items.Add(Item);
            }

            Graphics draw = routeDraw.CreateGraphics();
            draw.Clear(Color.Black);
            Brush my_brush = new SolidBrush(Color.Red);
            Pen pen = new Pen(Color.Blue, 2);
            for (int i = 0; i < Customers.Count -1 ; i++)
            {
                map.Add(Customers[i].ID, new PointF((float)Customers[i].X * 20 + routeDraw.Width / 2, (float)Customers[i].Y * 20 + routeDraw.Height / 2));
               
            }
            bool left = false, up = false, down = false, right = false;
            float leftValue = 0, upValue = 0, downValue = 0, rightValue = 0;
            for (int i = 0; i < Customers.Count - 1; i++)
            {
                if (map[Customers[i].ID].X < 0)
                {
                    left = true;
                    leftValue = map[Customers[i].ID].X * (-1) + 10;
                }
                if (map[Customers[i].ID].X > routeDraw.Width)
                {
                    right = true;
                    rightValue = map[Customers[i].ID].X - routeDraw.Width + 10;
                }
                if (map[Customers[i].ID].Y < 0)
                {
                    down = true;
                    downValue = map[Customers[i].ID].Y * (-1) + 10;
                }
                if (map[Customers[i].ID].Y > routeDraw.Height)
                {
                    up = true;
                    upValue = map[Customers[i].ID].Y - routeDraw.Height + 10;

                }
            }

            if (down)
            {
                draw.ScaleTransform(0.8F, 1);
                for (int i = 0; i < Customers.Count - 1; i++)
                {
                    PointF p = new PointF(map[Customers[i].ID].X, map[Customers[i].ID].Y + downValue);
                    map[Customers[i].ID] = p;
                }
            }
            if (up)
            {
                draw.ScaleTransform(0.8F, 1);
                for (int i = 0; i < Customers.Count - 1; i++)
                {
                    PointF p = new PointF(map[Customers[i].ID].X, map[Customers[i].ID].Y - upValue);
                    map[Customers[i].ID] = p;
                }
            }
            if (left)
            {
                draw.ScaleTransform(1, 0.8F);
                for (int i = 0; i < Customers.Count - 1; i++)
                {
                    PointF p = new PointF(map[Customers[i].ID].X + leftValue, map[Customers[i].ID].Y );
                    map[Customers[i].ID] = p;
                }
            }
            if (right)
            {
                draw.ScaleTransform(1, 0.8F);

                for (int i = 0; i < Customers.Count - 1; i++)
                {
                    PointF p = new PointF(map[Customers[i].ID].X - rightValue, map[Customers[i].ID].Y );
                    map[Customers[i].ID] = p;
                }
            }

            for (int i = 0; i < Customers.Count -1; i++)
            {
                draw.FillRectangle(my_brush, map[Customers[i].ID].X, map[Customers[i].ID].Y, 5, 5);
                if(Customers[i].ID == 0)
                    draw.DrawString("Depot", new Font("Arial", 8), my_brush, map[Customers[i].ID].X + 3, map[Customers[i].ID].Y + 3);
                else
                    draw.DrawString("" + Customers[i].ID, new Font("Arial", 8), my_brush, map[Customers[i].ID].X + 3, map[Customers[i].ID].Y + 3);
                draw.DrawLine(pen, map[Customers[i].ID], map[Customers[i+ 1].ID]);
            }


           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void routeDraw_Click(object sender, EventArgs e)
        {

        }
    }
}
