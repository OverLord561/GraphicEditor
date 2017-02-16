using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        Color colorOfLine = Color.White; // Color of line/border
        Color colorOfBackground ; // Color of filling (background)
        Color rememberBackgroundColor;
        int thickness;   // thickness of line
        bool draw = false; // Shows status of drawing        
        Tool tool; // Tool for drawing     
        int sx, sy, fx, fy;  //points for figures drawing       
        int initialY, initialX;  //first click(mouse down) on picturebox

        public delegate void ShowProgressDelegate();// access to load gif
        ShowProgressDelegate DELGif;

        public delegate void InvertImageDelegate(Bitmap image); // access to paintBox
        InvertImageDelegate DELImage;

        Bitmap bmp; // Drawn image in bitmap format during process of drawing
        Bitmap graphicBMP;  // Finally drawn image in bitmap format
        Image basicpicture;     // Finally drawn image     
        Image initialPicture;  // After "clear" operation, all graphics replace to this image.

        public Form1()
        {
            InitializeComponent();

            basicpicture = paintBox.Image;
            graphicBMP = new Bitmap(paintBox.Image);
            initialPicture = basicpicture;

            // show the color of line by default
            picSelectedColorOfLine.BackColor = Color.White;
            // show the color of filling by default
            picSelectedColorOfBackground.BackColor = Color.White;
            
            DELGif = new ShowProgressDelegate(UpdateProgress); // link load gif updater
            DELImage = new InvertImageDelegate(UpdateImage); // link load paintBox updater

            // to enable access to controls from another thread
            // CheckForIllegalCrossThreadCalls = false;


        }
        public enum Tool
        {
            Pencil, Line, Rectangle, Circle, Erase
        }

        #region SetColor and Tickness
        // to set color of border/line and show it on picturebox
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SetColor(ref colorOfLine, picSelectedColorOfLine);
        }

        //to set color of filling(basckground) and show it on picturebox
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            bool IsbgrColorSet = false;
            IsbgrColorSet =  SetColor(ref colorOfBackground, picSelectedColorOfBackground); // if DialogResult.OK, modify checkbox;
            rememberBackgroundColor = colorOfBackground;
            checkBox1.Enabled = IsbgrColorSet;
            checkBox1.Checked = IsbgrColorSet;
        }
        // SET color via ColorDialog
        private bool SetColor(ref Color currentColor, PictureBox currentPictureBOX)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog1.Color;
                currentPictureBOX.BackColor = currentColor;
                return true;
            }
            else
            {
                return false;
            }
           

        }
        // SET thickness of line via trackbar
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            thickness = trackBar1.Value;
            labelThickness.Text = thickness.ToString();
        }
        #endregion


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            label7.Text = "";
            label6.Text = "";
            label5.Text = "";
            label4.Text = "";

            paintBox.Image = initialPicture;
            basicpicture = initialPicture;
        }



        #region SelectedTool
        private void toolStripCircle_Click(object sender, EventArgs e)
        {
            tool = Tool.Circle;
        }

        private void toolStripPencil_Click(object sender, EventArgs e)
        {
            tool = Tool.Pencil;
        }

        private void toolStripLine_Click(object sender, EventArgs e)
        {
            tool = Tool.Line;
        }

        private void toolStripRectangle_Click(object sender, EventArgs e)
        {
            tool = Tool.Rectangle;
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            tool = Tool.Erase;
        }
        #endregion

        #region SaveImage
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

        //Save image according to selected format
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


        #endregion
        #region OpenFile
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = null;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Images|*.png;*.bmp;*.jpg";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                path = openFile.FileName;

            }

            paintBox.Image = Image.FromFile(path);
            basicpicture = paintBox.Image;
            initialPicture = basicpicture;
            graphicBMP = new Bitmap(paintBox.Image);


        }


        #endregion

        #region Drawing
        // First initial click on picturebox
        private void paintBox_MouseDown(object sender, MouseEventArgs e)
        {

            draw = true; // status of drawing
            sx = e.X;
            sy = e.Y;

            initialY = sy;
            initialX = sx;

            //Display points
            label4.Text = initialX.ToString();
            label5.Text = initialY.ToString();
            label6.Text = fx.ToString();
            label7.Text = fy.ToString();


        }
        

        private void paintBox_MouseMove(object sender, MouseEventArgs e)
        {

            if (draw)
            {

                Pen line = new Pen(colorOfLine);
                line.Width = thickness;


                if (tool == Tool.Pencil || tool == Tool.Erase)
                {

                    bmp = new Bitmap(paintBox.Image);
                }
                else
                {
                    /*To see slightly increased figure, it is necessary to remove previous drawn figure
                     by replacing the painted image to basic picture(before we started painting / initial picture)*/
                    bmp = new Bitmap(basicpicture);
                    line.Width = thickness+2;
                }
              
                using (Graphics graphic = Graphics.FromImage(bmp))
                {
                    fx = e.X;
                    fy = e.Y;

                    switch (tool)
                    {
                        case Tool.Rectangle:

                        
                            SwapPoints(ref fx, ref fy);
                            Rectangle rectangle = new Rectangle(sx, sy, fx - sx, fy - sy);
                            graphic.DrawRectangle(line, rectangle);
                            graphic.FillRectangle(new SolidBrush(colorOfBackground), rectangle);
                            paintBox.Image = bmp;
                            break;


                        case Tool.Circle:

                           
                            SwapPoints(ref fx, ref fy);
                            graphic.DrawEllipse(line, sx, sy, fx - sx, fy - sy);
                            graphic.FillEllipse(new SolidBrush(colorOfBackground), sx, sy, fx - sx, fy - sy);
                            paintBox.Image = bmp;
                            break;


                        case Tool.Line:

                            graphic.DrawLine(line, new Point(sx, sy), new Point(fx, fy));
                            paintBox.Image = bmp;
                            break;

                        case Tool.Pencil:

                            graphic.FillEllipse(new SolidBrush(colorOfLine), fx - sx + sx, fy - sy + sy, thickness + 5, thickness + 5);
                            paintBox.Image = bmp;
                            break;

                        case Tool.Erase:

                            graphic.FillEllipse(new SolidBrush(Color.White), fx - sx + sx, fy - sy + sy, thickness + 5, thickness + 5);
                            paintBox.Image = bmp;
                            break;
                    }

                }
            }

        }


        private void paintBox_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;

            graphicBMP = new Bitmap(paintBox.Image); // Drawn image in bitmap format
            basicpicture = paintBox.Image;  //set new background after drawing

        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked == false) // if figure must be empty
            {
                colorOfBackground = new Color(); // back color must be null
            }
            else // else use remembered  color for background filling
            {
                colorOfBackground = rememberBackgroundColor;
            }

        }
        // Swap points to replace point of start and finish
        // cause: drawing figures is possible only from top left to bottom right by default
        // so,  when we try to draw in another direction we have to imitation drawing from top left to bottom right
        private void SwapPoints(ref int fx, ref int fy)
        {
            label4.Text = initialX.ToString();
            label5.Text = initialY.ToString();
            label6.Text = fx.ToString();
            label7.Text = fy.ToString();

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
        #endregion

        #region Inversion

        // Set load gif invisible
        private void UpdateProgress()
        {
            loadGif.Visible = false;
        }
        // Set converted picture
        private void UpdateImage(Bitmap image)
        {
            paintBox.Image = image;
        }

        
        

        Task<Bitmap> InvertAsync(Bitmap source)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);

                return TransformImage(source);

            });
        }

        //Invert image via async programming using TASKS, async & await
        private async void invertAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadGif.Visible = true;       
            graphicBMP = await InvertAsync(this.graphicBMP); // Convertation in new thread. Management will return to this point
            paintBox.Image = graphicBMP; 
            basicpicture = paintBox.Image;  
            UpdateProgress();  
        }

       // Invert image via async programming using Thread, delegate
        private void button1_Click(object sender, EventArgs e)
        {

          
            loadGif.Visible = true; 
            Thread myThread = new Thread(delegate()
            {               
                graphicBMP = TransformImage(graphicBMP);
                //To access painBox  from another thread via delegate */  
                paintBox.Invoke(DELImage, new object[] { graphicBMP });
                basicpicture = paintBox.Image;
                
                //To access load gif from another thread via delegate */                           
                loadGif.Invoke(DELGif); 


            });
            myThread.Start();


        }

        public Bitmap TransformImage(Bitmap source)
        {

            Thread.Sleep(5000);
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(source.Width, source.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            // create the negative color matrix
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                 new float[] {-1, 0, 0, 0, 0},
                 new float[] {0, -1, 0, 0, 0},
                 new float[] {0, 0, -1, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {1, 1, 1, 0, 1}
            });

            // create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                        0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();

            return newBitmap;
        }
        #endregion

    }
}
