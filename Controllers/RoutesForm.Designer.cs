namespace MA_EAX_CVRP_SOLVER.GUI
{
    partial class RoutesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RouteslistBox = new System.Windows.Forms.ListBox();
            this.routeDraw = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.routeDraw)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 283);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(298, 75);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(403, 205);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Costumer ID";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "coordinates";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Demands";
            this.columnHeader3.Width = 150;
            // 
            // RouteslistBox
            // 
            this.RouteslistBox.FormattingEnabled = true;
            this.RouteslistBox.Location = new System.Drawing.Point(25, 75);
            this.RouteslistBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RouteslistBox.Name = "RouteslistBox";
            this.RouteslistBox.Size = new System.Drawing.Size(204, 199);
            this.RouteslistBox.TabIndex = 6;
            // 
            // routeDraw
            // 
            this.routeDraw.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.routeDraw.Location = new System.Drawing.Point(25, 320);
            this.routeDraw.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.routeDraw.Name = "routeDraw";
            this.routeDraw.Size = new System.Drawing.Size(675, 435);
            this.routeDraw.TabIndex = 7;
            this.routeDraw.TabStop = false;
            this.routeDraw.Click += new System.EventHandler(this.routeDraw_Click);
            // 
            // RoutesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 836);
            this.Controls.Add(this.routeDraw);
            this.Controls.Add(this.RouteslistBox);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "RoutesForm";
            this.Text = "RoutesForm";
            ((System.ComponentModel.ISupportInitialize)(this.routeDraw)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListBox RouteslistBox;
        private System.Windows.Forms.PictureBox routeDraw;
    }
}