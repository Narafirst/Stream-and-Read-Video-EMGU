using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using AForge.Video;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.UI;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace ReadVideoFile01
{
    public partial class Form1 : Form
    {
        MJPEGStream stream;
        Capture capvideo;
        Mat imgFrame;
        Image<Bgr, Byte> im2Show;
        bool blnFormClosing = false;       
        int onTab = 1;
        //Capture cap;
        List<Point> clickPoints = new List<Point>();

        public Form1()
        {
            InitializeComponent();
        }

        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            im2Show = new Image<Bgr, Byte>(bmp);
            imageBox1.Image = im2Show ;
        }        

        void playVideoFile()
        {
            bool blnFirstFrame = true;

            while (blnFormClosing == false)
            {
                if (onTab == 1)
                {
                    imageBox1.Image = null;
                    return;
                }

                imgFrame = capvideo.QueryFrame();

                if (imgFrame == null & blnFirstFrame == true)
                {
                    txbInfo.AppendText("Unable to Open Video File" + "\r\n");
                    return;
                }
                else if (imgFrame == null & blnFirstFrame == false)
                {
                    txbInfo.AppendText("End of Video" + "\r\n");
                    return;
                }

                im2Show = imgFrame.ToImage<Bgr, Byte>();
                imageBox1.Image = im2Show;

                Application.DoEvents();

                blnFirstFrame = false;
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            blnFormClosing = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {            
            stream = new MJPEGStream();
            stream.Source = "http://";
            stream.Source += txbIP.Text;
            stream.Source += txbPath.Text;
            stream.Login = txbUsername.Text;
            stream.Password = txbPassword.Text;

            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                stream.Stop();
            }
        }       

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            DialogResult drChosenFile;
            drChosenFile = openFileDialog1.ShowDialog();

            if (drChosenFile != DialogResult.OK | openFileDialog1.FileName == "")
            {
                txbInfo.AppendText("File not Chosen!!"+"\r\n");
                return;
            }

            try
            {
                capvideo = new Capture(openFileDialog1.FileName);
            }                
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read video file, ERROR: " + ex.Message);
                return;
            }
            
            if(capvideo == null)
            {                
                txbInfo.AppendText("Unable to read video file"+"\r\n");
            }

            txbPathFile.Text = openFileDialog1.FileName;
            playVideoFile();
               
        }
        
        private void tabPage1_Enter(object sender, EventArgs e)
        {
            onTab = 1;            
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            onTab = 2;                       
            imageBox1.Image = null;
        }

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                stream.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //using (Image<Bgr, byte> nextFrame = cap.QueryFrame())
            //{
            //    if (nextFrame != null)
            //    {
            //        // there's only one channel (greyscale), hence the zero index
            //        //var faces = nextFrame.DetectHaarCascade(haar)[0];
            //        Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
                    
            //    }
            //}
        }

        private void imageBox1_MouseDown(object sender, MouseEventArgs e)
        {
            double scaleX = imageBox1.Width / imageBox1.ClientSize.Width;
            double scaleY = imageBox1.Height / imageBox1.ClientSize.Height;
            String sscaleX = scaleX.ToString();
            String sscaleY = scaleY.ToString();

            int posX = e.X;
            int posY = e.Y;
            String locationX = posX.ToString();
            String locationY = posY.ToString();
           
            System.Diagnostics.Debug.Write(locationX + " , " + locationY + "\r\n");
            System.Diagnostics.Debug.Write(sscaleX + " , " + sscaleY + "\r\n");

            clickPoints.Add(new Point(posX, posY));
            if (clickPoints.Count <= 3)
                return;
            int rectX, rectY, rectWidth, rectHeight;
            Mat mask = Utils.CreateMaskFromPoints(clickPoints.ToArray(), out rectX, out rectY, out rectWidth, out rectHeight);
        }


    }
}
