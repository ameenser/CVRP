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
using CVRP_SOLVER.CODE;
using MA_EAX_CVRP_SOLVER.Singletons;
using System.Threading;

namespace MA_EAX_CVRP_SOLVER.GUI
{
    
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        List<Costumer> nodes;
        string Path = ".";
        Solution best;
        int cantWork = 0;
        int Capacity = 0;
        double progress = 0;
        bool Zeroprecentflag = false;

        private void button1_Click(object sender, EventArgs e)
        {
            
             cantWork = 1;
            //Console.WriteLine(message.Length);

            if (Path.Length == 1)
            {
                MessageBox.Show("Please choose a file !!");
            }
            else
            {

                Graph g1 = new Graph();   // Build a new graph 
                if(checkBox1.CheckState == CheckState.Checked)
                {
                    if(!string.IsNullOrEmpty(textBox2.Text))
                    {
                        MessageBox.Show("if you entered a vehicle capacity please uncheck getting capacity from file");
                        return;
                    }
                    if (!g1.parse_file(Path, Capacity, this))  // parse the info
                        return;
                    nodes = g1.nodes;
                    MapFormSingleton.getInstance(nodes);
                }
                else
                {

                    if (String.IsNullOrEmpty(textBox2.Text))
                    {
                        cantWork = 0;
                    }
                    else
                    {
                        try
                        {
                         
                            Capacity = int.Parse(textBox2.Text);
                            if (Capacity <= 0)
                            {
                                MessageBox.Show("vehicle capacity have to be a positive number !!");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("vehicle capacity have to be a positive number !!");
                            return;

                        }
                        if (!g1.parse_file(Path, Capacity, this))  // parse the info
                            return;
                       nodes = g1.nodes;
                        MapFormSingleton.getInstance(nodes);
                    }
                   
                    
                }
                if (cantWork == 0)
                {
                    MessageBox.Show("enter vehicle capacity  or enable to read it from the file");
                }
                else
                {
                    algUpdate.Left -= 10;
                    button1.Enabled = false;
                    MainForm form = this;
                    var task = Task.Run(() =>
                    {
                        best = MA_EAX.solve(g1,this);
                        MessageBox.Show(best.ToString());
                        form.reEnableRunButton();
                    });
                }             
            }
               
            
            
        }
        
        OpenFileDialog ofd = new OpenFileDialog();

        private void button3_Click(object sender, EventArgs e)
        {
            if(ofd.ShowDialog() == DialogResult.OK)
            {
               textBox1.Text  = Path = ofd.FileName;
                
            }
        }

 

        private void openRoutesForm(object sender, EventArgs e)
        {
            RoutesForm routesForm;
            if (best == null)
            {
                routesForm = RouteFormSingleton.getInstance();
            }
            else
            {
                RouteFormSingleton.updateInstance(best);
                routesForm = RouteFormSingleton.getInstance(best);
            }
            routesForm.Show();
          
           
        }




        private void openMapForm(object sender, EventArgs e)
        {
            if (Path.Length == 1)
            {
                MessageBox.Show("Please choose a file !!");
            }
            else
            {
                if(nodes == null || nodes.Count == 0)
                {
                    MessageBox.Show("Please run the algorithm first!");
                    return;
                }
                MapForm mapForm;
                if (best == null)
                {
                    mapForm = MapFormSingleton.getInstance(nodes);
                }
                else
                {
                    MapFormSingleton.updatetInstance(best);
                    mapForm = MapFormSingleton.getInstance(best, nodes);
                }

                mapForm.Show();
            }
         }

      

        private void MainForm_Load(object sender, EventArgs e)
        {
            best = null;
            
        }

        public void sendNewBestSolution(Solution currBest)
        {
            this.Invoke(new MethodInvoker(() =>
             newSolutionIndicator(currBest)
            ));
            
        }

        public void sendWrongFileFormat()
        {
            this.Invoke(new MethodInvoker(() =>
            MessageBox.Show("wrong file format!!")
            ));

        }
        public void reEnableRunButton()
        {
            this.Invoke(new MethodInvoker(() =>
            button1.Enabled = true

            ));

        }

        public void newSolutionIndicator(Solution currBest)
        {
            best = currBest;
            algUpdate.Text = "found new solution!!!";
            algUpdate.ForeColor = Color.AliceBlue;
            var task = Task.Run(() =>
            {
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(true);
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(false);
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(true);
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(false);
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(true);
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(false);
                Thread.Sleep(1000);
                turnALgUpdateLabelOnOff(true);
            });
        }


        public void update(bool isVisible)
        {
            algUpdate.Visible = isVisible;
        }
        public void turnALgUpdateLabelOnOff(bool isVisible)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            update(isVisible)
            ));

        }
        public void sendNewPrgressBarValues(string newText, int currGen, int lastGen)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            updateProgressBar(newText,  currGen,  lastGen)
            ));
            
        }
        public void updateProgressBar(string newText, int currGen, int lastGen)
        {
            double percentage = Math.Round((currGen / (double)lastGen * 100), 2);

            Graphics draw = progressbarDraw.CreateGraphics();
            Pen pen = new Pen(Color.Black, 2);
            Brush my_brush = new SolidBrush(Color.Green);
            Brush text_brush = new SolidBrush(Color.Black);
            
            string text = percentage.ToString() + "%";
            if (percentage > progress)
            {
                progress = percentage;
                draw.Clear(this.BackColor);
                draw.FillRectangle(my_brush, 0, 0, (float)(currGen / (double)lastGen * progressbarDraw.Width), progressbarDraw.Height);
                draw.DrawRectangle(pen, 0, 0, progressbarDraw.Width, progressbarDraw.Height);
                draw.DrawString(text, new Font("Arial", 10), text_brush, (float)progressbarDraw.Width / 2 - 10, (float)progressbarDraw.Height / 2 - 8);
                return;
            }
            if (percentage == 0 && !Zeroprecentflag)
            {
                Zeroprecentflag = true;
                draw.Clear(this.BackColor);
                draw.FillRectangle(my_brush, 0, 0, (float)(currGen / (double)lastGen * progressbarDraw.Width), progressbarDraw.Height);
                draw.DrawRectangle(pen, 0, 0, progressbarDraw.Width, progressbarDraw.Height);
                draw.DrawString("0.00%", new Font("Arial", 10), text_brush, (float)progressbarDraw.Width / 2 - 10, (float)progressbarDraw.Height / 2 - 8);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    public static class MatrixExtensions
    {
        /// <summary>
        /// Returns the row with number 'row' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength(1);
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[row, i];

            return rowVector;
        }



        /// <summary>
        /// Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
        /// </summary>
        public static void SetRow<T>(this T[,] matrix, int row, T[] rowVector)
        {
            var rowLength = matrix.GetLength(1);

            for (var i = 0; i < rowLength; i++)
                matrix[row, i] = rowVector[i];
        }



        /// <summary>
        /// Returns the column with number 'col' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetCol<T>(this T[,] matrix, int col)
        {
            var colLength = matrix.GetLength(0);
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[i, col];

            return colVector;
        }


        /// <summary>
        /// Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
        /// </summary>
        public static void SetCol<T>(this T[,] matrix, int col, T[] colVector)
        {
            var colLength = matrix.GetLength(0);

            for (var i = 0; i < colLength; i++)
                matrix[i, col] = colVector[i];
        }
    }
}
