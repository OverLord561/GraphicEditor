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
        Color paintColor = Color.Red;
        bool draw = false;
        int thickness;
        Tool tool = Tool.Rectangle;
        //point to draw figires
        int sx, sy, fx, fy;
        //first click(mouse down) on picturebox
        int initialY, initialX;
        delegate void MyDelegate(int x, int y);
        MyDelegate del;
        Bitmap bmp;
        Bitmap graphicBMP;
        Image basicpicture;
        Image initialPicture;

        public Form1()
        {
            InitializeComponent();
           //  del = new MyDelegate(SwapPoints);
            basicpicture = paintBox.Image;
            graphicBMP = new Bitmap(paintBox.Image);
            initialPicture = basicpicture;

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
            tool = Tool.Line;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            thickness = trackBar1.Value;
            labelThickness.Text = thickness.ToString();
        }

        private void toolStripRectangle_Click(object sender, EventArgs e)
        {
            tool = Tool.Rectangle;
        }

        private void paintBox_MouseDown(object sender, MouseEventArgs e)
        {

            draw = true;
            sx = e.X;
            sy = e.Y;
            //////
            initialY = sy;
            initialX = sx;
            
            label4.Text = initialX.ToString();
            label5.Text = initialY.ToString();
            label6.Text = fx.ToString();
            label7.Text = fy.ToString();

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label8.Text = "";
            label7.Text = "";
            label6.Text = "";
            label5.Text = "";
            label4.Text = "";

            paintBox.Refresh();
          
            paintBox.Image = initialPicture;
            basicpicture = initialPicture;
        }

        private void toolStripCircle_Click(object sender, EventArgs e)
        {
            tool = Tool.Circle;
        }

        private void toolStripPencil_Click(object sender, EventArgs e)
        {
            tool = Tool.Pencil;
        }


       

        

        private void paintBox_MouseMove(object sender, MouseEventArgs e)
        {
            
           
           


            if (draw)
            {
                Graphics g = paintBox.CreateGraphics();
                //   Task task1 = new Task(() => SwapPoints(e.X, e.Y));
                //   task1.Start();
                //   task1.Wait();



                Pen line = new Pen(paintColor);
                line.Width = thickness;


                bmp = new Bitmap(basicpicture);
                Graphics gg = Graphics.FromImage(bmp);
            
              

                switch (tool)
                {
                    case Tool.Rectangle:
                       
                        fx = e.X;
                        fy = e.Y;
                        SwapPoints(ref fx, ref fy);
                       
                        Rectangle rectangle = new Rectangle(sx, sy, fx - sx, fy - sy);
                        gg.DrawRectangle(line, rectangle);
                        //g.Dispose();
                        //  gg.DrawRectangle(line, rectangle);
                          paintBox.Image = bmp;
                        //   gg.Dispose();

                       
                        break;


                    case Tool.Circle:
                       // тут якось працює
                        //  g.FillEllipse(new SolidBrush(paintColor), sx, sy, e.X - sx, e.Y - sy);
                        //gg.FillEllipse(new SolidBrush(paintColor), sx, sy, e.X - sx, e.Y - sy);

                        break;
                    //case Item.Brush:
                    //    g.FillEllipse(new SolidBrush(paintcolor), e.X - x + x, e.Y - y + y, Convert.ToInt32(toolStripTextBox1.Text), Convert.ToInt32(toolStripTextBox1.Text));
                    //    break;
                    case Tool.Pencil:
                        //gg.FillEllipse(new SolidBrush(paintColor), e.X - sx + sx, e.Y - sy + sy, thickness + 5, thickness + 5);
                        break;
                        //case Item.eraser:
                        //    g.FillEllipse(new SolidBrush(pictureBox1.BackColor), e.X - x + x, e.Y - y + y, Convert.ToInt32(toolStripTextBox1.Text), Convert.ToInt32(toolStripTextBox1.Text));
                        //    break;
                }
                
            }
            // bitMap = Bitmap.FromHbitmap(g.GetHdc());
            //intptr = g.GetHdc();

            
            

            //bitMap = new Bitmap(paintBox.Width, paintBox.Height, g);
           
        }

        private void paintBox_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
            fx = e.X;
            fy = e.Y;

            graphicBMP =  new Bitmap(paintBox.Image);
            basicpicture = paintBox.Image;
          //  paintBox.Image = graphicBMP;

            // we have to swap points in cases when we draw not from top left to bottom right (\)
            // SwapPoints(ref fx,ref fy);





            Graphics g = paintBox.CreateGraphics();
            Pen line = new Pen(paintColor);
            line.Width = thickness;


            switch (tool)
            {

                case Tool.Line:

                    g.DrawLine(new Pen(new SolidBrush(paintColor), thickness), new Point(sx, sy), new Point(fx, fy));
                 
                    g.Dispose();

                    break;
                case Tool.Rectangle:
                    Rectangle rectangle = new Rectangle(sx, sy, fx - sx, fy - sy);
                    g.DrawRectangle(line, rectangle);
                    break;

            }







        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SwapPoints(ref int fx, ref int fy)
        {
            label4.Text = initialX.ToString();
            label5.Text = initialY.ToString();
            label6.Text = fx.ToString();
            label7.Text = fy.ToString();

            //paintBox.Refresh();
            // 4 quarter by default
            sy = initialY;
            sx = initialX;

            int interx, intery = 0;
            // 2 quarter
            if (fx < sx && fy < sy)
            {
                
                interx = sx;
                intery = sy;
                sx = fx;
                fx = interx;
                sy = fy;
                fy = intery;

            }
            // 1 quarter
            else if (fx > sx && fy < sy)
            {

                intery = sy;
                sy = fy;
                fy = intery;
            }
            // 3 quarter
            else if (fx < sx && fy > sy)
            {

                intery = sy;
                interx = sx;
                sx = fx;
                fx = interx;
            }
            
         
        }

    }
}
