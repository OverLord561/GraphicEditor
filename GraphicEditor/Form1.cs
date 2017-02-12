using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicEditor
{
    public partial class Form1 : Form
    {
        Color paintColor;
        bool draw = false;
        int thickness;
        Tool item;
        int sx, sy, fx, fy ;

        public Form1()
        {
            InitializeComponent();
        }
        public enum Tool
        {
            Pencil, Line, Rectangle, Circle
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                paintColor = colorDialog1.Color;
                rgbA.Text = paintColor.A.ToString();
                rgbR.Text = paintColor.R.ToString();
                rgbG.Text = paintColor.G.ToString();
                rgbB.Text = paintColor.B.ToString();

                picSelectedColor.BackColor = paintColor;

            }
        }



        private void toolStripLine_Click(object sender, EventArgs e)
        {
            item = Tool.Line;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            thickness = trackBar1.Value;
            labelThickness.Text = thickness.ToString();
        }

        private void paintBox_MouseDown(object sender, MouseEventArgs e)
        {
            
            draw = true;
            sx = e.X;
            sy = e.Y;
        }
        private void paintBox_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
            fx = e.X;
            fy = e.Y;

            if (item == Tool.Line)
            {
              
                Graphics g = paintBox.CreateGraphics();
                g.DrawLine(new Pen(new SolidBrush(paintColor), thickness), new Point(sx, sy), new Point(fx, fy));
                g.Dispose();
                

            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
    }
}
