using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicEditor
{
    public partial class Form1 : Form
    {
        // Color of line/border
        Color colorOfLine;
        // Color of filling (background)
        Color colorOfBackground;

        bool draw = false;
        int thickness;
        Tool tool;
        //point to draw figires
        int sx, sy, fx, fy;
        //first click(mouse down) on picturebox
        int initialY, initialX;
        ImageFormat format;
        delegate void MyDelegate(int x, int y);

        Bitmap bmp;
        Bitmap graphicBMP;
        Image basicpicture;
        Image initialPicture;

        public Form1()
        {
            InitializeComponent();

            basicpicture = paintBox.Image;
            graphicBMP = new Bitmap(paintBox.Image);
            initialPicture = basicpicture;

           picSelectedColorOfLine.BackColor = Color.White;
           picSelectedColorOfBackground.BackColor = Color.White;


        }
        public enum Tool
        {
            Pencil, Line, Rectangle, Circle
        }

        // to set color of border/line and show it on picturebox
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SetColor(ref colorOfLine, picSelectedColorOfLine);
        }

        //to set color of filling(basckground) and show it on picturebox
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SetColor(ref colorOfBackground, picSelectedColorOfBackground);
        }

        private void SetColor(ref Color currentColor, PictureBox currentPicture)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog1.Color;
                currentPicture.BackColor = currentColor;

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

        private void BMPToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            SaveImage(ImageFormat.Bmp);            
        }

        private void jPEGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage(ImageFormat.Jpeg);
        }

        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage(ImageFormat.Png);
        }

        private void SaveImage(ImageFormat format)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Images|*.png;*.bmp;*.jpg";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                basicpicture.Save(dialog.FileName, format);
                MessageBox.Show("Image was saved!!!", "Success");
            }
        }

        private void paintBox_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (draw)
            {

                Pen line = new Pen(colorOfLine);
                line.Width = thickness;


                if (tool == Tool.Pencil)
                {
                    bmp = new Bitmap(paintBox.Image);
                }
                else
                {
                    bmp = new Bitmap(basicpicture);
                }

                using (Graphics graphic = Graphics.FromImage(bmp))
                {

                    switch (tool)
                    {
                        case Tool.Rectangle:

                            fx = e.X;
                            fy = e.Y;
                            SwapPoints(ref fx, ref fy);
                            Rectangle rectangle = new Rectangle(sx, sy, fx - sx, fy - sy);
                            graphic.DrawRectangle(line, rectangle);
                            graphic.FillRectangle(new SolidBrush(colorOfBackground), rectangle);
                            //await DrawRectAsync(rectangle, paintBox, tool, basicpicture);
                            paintBox.Image = bmp;
                            break;


                        case Tool.Circle:
                            fx = e.X;
                            fy = e.Y;
                            SwapPoints(ref fx, ref fy);

                            graphic.DrawEllipse(line, sx, sy, fx - sx, fy - sy);
                            graphic.FillEllipse(new SolidBrush(colorOfBackground), sx, sy, fx - sx, fy - sy);
                            paintBox.Image = bmp;
                            break;


                        case Tool.Line:
                            fx = e.X;
                            fy = e.Y;
                            graphic.DrawLine(line, new Point(sx, sy), new Point(fx, fy));
                            paintBox.Image = bmp;
                            break;

                        case Tool.Pencil:
                            fx = e.X;
                            fy = e.Y;

                            graphic.FillEllipse(new SolidBrush(colorOfLine), fx - sx + sx, fy - sy + sy, thickness + 5, thickness + 5);
                            paintBox.Image = bmp;
                            break;
                            //case Item.eraser:
                            //    g.FillEllipse(new SolidBrush(pictureBox1.BackColor), e.X - x + x, e.Y - y + y, Convert.ToInt32(toolStripTextBox1.Text), Convert.ToInt32(toolStripTextBox1.Text));
                            //    break;
                    }

                }
            }

        }

        //Task DrawRectAsync( Rectangle rec, PictureBox paintBox, Tool tool, Image basicpicture)
        //{
        //    Bitmap bmp;
        //    return Task.Factory.StartNew(() =>
        //    {
        //        if (tool == Tool.Pencil)
        //        {
        //            bmp = new Bitmap(paintBox.Image);
        //        }
        //        else
        //        {
        //            using (bmp = new Bitmap(basicpicture)) { }

        //        }

        //        Graphics g = Graphics.FromImage(bmp);
        //        Pen line = new Pen(paintColor);
        //        Rectangle rectangle = new Rectangle(sx, sy, fx - sx, fy - sy);
        //        g.DrawRectangle(line, rectangle);
        //        paintBox.Image = bmp;

        //    });
        //}


        private void paintBox_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;            

            graphicBMP = new Bitmap(paintBox.Image);
            basicpicture = paintBox.Image;         
            
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
