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

namespace ReadVideoFile01
{
    public partial class Form1 : Form
    {
        MJPEGStream stream;
        Capture capvideo;
        Mat imgFrame;
        bool blnFormClosing = false;
        int onTab = 1;

        public Form1()
        {
            InitializeComponent();
        }

        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();            
            imageBox1.Image = new Image<Bgr, Byte>(bmp) ;
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
            stream.Stop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            blnFormClosing = true;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            DialogResult drChosenFile;
            drChosenFile = openFileDialog1.ShowDialog();

            if (drChosenFile != DialogResult.OK | openFileDialog1.FileName == "")
            {
                txbPathFile.Text = "File not Chosen!!";
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
                txbInfo.AppendText("Unable to read video file");
            }

            txbPathFile.Text = openFileDialog1.FileName;
            playVideo();
               
        }

        void playVideo()
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
                    txbInfo.AppendText("Unable to Open Video File");
                    return;
                }
                else if (imgFrame == null & blnFirstFrame == false)
                {                    
                    txbInfo.AppendText("End of Video");
                    return;           
                }

                imageBox1.Image = imgFrame;

                Application.DoEvents();

                blnFirstFrame = false;
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            onTab = 1;            
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            onTab = 2;
            stream.Stop();
            imageBox1.Image = null;
        }
    }
}
