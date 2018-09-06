//using Emgu.CV;
//using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedTester;

namespace ZedTester
{
    public partial class ZedUI : Form
    {
        int fps = 0;

        int[][] Resolutions = { new[] { 2560, 1440 }, new[] { 1920, 1080 }, new[] { 1280, 720 }, new [] { 672, 376 } };

        int ResolutionH => Resolutions[Properties.Settings.Default.Resolution][0] / (Properties.Settings.Default.SideBySide ? 1 : 2);
        int ResolutionV => Resolutions[Properties.Settings.Default.Resolution][1] / 2;

        string fpsLabelValue = "0 FPS";
        DateTime currentTime;

        Bitmap finalLeft;
        Bitmap finalRight;
        ImageServer server;

        private bool serverInititalised;
        private Wrapper wrapper;

        bool running;
        int stride;
        Bitmap left;
        Bitmap right;

        // controls

        //private Emgu.CV.UI.ImageBox backgroundViewLeft;
        //private Emgu.CV.UI.ImageBox backgroundViewRight;
        //private Emgu.CV.UI.ImageBox inputViewLeft;
        //private Emgu.CV.UI.ImageBox inputViewRight;
        //private Emgu.CV.UI.ImageBox outputViewLeft;
        //private Emgu.CV.UI.ImageBox outputViewRight;
        //private Emgu.CV.UI.ImageBox maskViewLeft;
        //private Emgu.CV.UI.ImageBox maskViewRight;

        public ZedUI()
        {
            InitializeComponent();

            int[] setup = { 1, 0, 1, 2, 0 };


            // init settings
            this.thresholdTrack.Value = Properties.Settings.Default.Threshold;
            this.erodeTrack.Value = Properties.Settings.Default.Erode;
            this.dilateTrack.Value = Properties.Settings.Default.Dilate;
            this.contrastTrack.Value = (int)Properties.Settings.Default.Contrast;
            this.resolution.SelectedIndex = Properties.Settings.Default.Resolution;
            this.depthQuality.SelectedIndex = Properties.Settings.Default.DepthQuality;
            this.depthMode.SelectedIndex = Properties.Settings.Default.DepthMode;
            this.useCuda.Checked = Properties.Settings.Default.UseCuda;
            this.sideBySideCheck.Checked = Properties.Settings.Default.SideBySide;
            this.previewCheck.Checked = Properties.Settings.Default.Preview;
            this.camera.SelectedIndex = Properties.Settings.Default.Camera;

            this.startZed();

            this.currentTime = DateTime.Now;

            // update tracks 
            //var bm = Bitmap.FromFile("left_color.tiff") as Bitmap;
            //BitmapData bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            //var stride = bmd.Stride;

            this.stride = Properties.Settings.Default.Camera == 0
                ? (4 * ((ResolutionH * 32 + 31) / 32))  // ZED Stride is 4 channel image
                : (4 * ((ResolutionH * 32 + 31) / 32)); // Webcamera Stride for 3 channel image
        }


        void LogServerMessage(string message)
        {
            this.statusStrip1.BeginInvoke((MethodInvoker)delegate
            {
                this.serverMessage.Text = message;
            });
        }



        void Start()
        {
            // start an image server

            this.running = true;

            // start with stored SVO
            // var zed = new Zed(@"c:\Projects\files\HD720_SN17600_11-04-52.svo");

            //////////////////////////////////////////////////////////
            // CHOOSE YOUR CAMERA                                   //
            //////////////////////////////////////////////////////////

            new Thread(() =>
            {
                while (running)
                {
                    if (wrapper.grab())
                    {
                        if (!this.serverInititalised)
                        {
                            this.server = new ImageServer(this, this.LogServerMessage, Properties.Settings.Default.SideBySide ? 1 : 2);
                            this.serverInititalised = true;
                        }

                        if (Properties.Settings.Default.SideBySide)
                        {
                            IntPtr leftPointer = wrapper.GetSbs();
                            if (leftPointer != IntPtr.Zero)
                            {
                                left = new Bitmap(ResolutionH, ResolutionV, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, leftPointer);
                                // img = new Mat(new System.Drawing.Size(960, 540), DepthType.Cv8U, 4, color, stride);
                            }
                        }
                        else
                        {

                            IntPtr leftPointer = wrapper.GetLeft();
                            if (leftPointer != IntPtr.Zero)
                            {
                                left = new Bitmap(ResolutionH, ResolutionV, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, leftPointer);
                                // img = new Mat(new System.Drawing.Size(960, 540), DepthType.Cv8U, 4, color, stride);
                            }

                            IntPtr rightPointer = wrapper.GetRight();
                            if (rightPointer != IntPtr.Zero)
                            {
                                right = new Bitmap(ResolutionH, ResolutionV, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, rightPointer);
                                // img = new Mat(new System.Drawing.Size(960, 540), DepthType.Cv8U, 4, color, stride);
                            }
                        }

                        if (left != null && right != null)
                        {
                            this.AcquireImages(left, right);
                        }
                        else if (left != null)
                        {
                            this.AcquireImages(left, null);
                        }
                        // img.Save("test.jpg");
                        // CvInvoke.Imshow(win1, img); //Show the image
                    }
                }

            })
            { IsBackground = true }.Start();



        }

        public void ShowPreview(Bitmap bmp)
        {
            this.pictureBoxRight.BeginInvoke((MethodInvoker)delegate
            {
                this.pictureBoxRight.Image = bmp;
            });
        }

        private byte[] GetBytes(Bitmap img)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        private void AcquireImages(Bitmap finalLeft, Bitmap finalRight)
        {
            //Bitmap bm = new Bitmap()
            try
            {
                this.finalLeft = finalLeft;
                this.finalRight = finalRight;

                if (Properties.Settings.Default.SideBySide)
                {
                    this.server.ReplaceImage(finalLeft, GetBytes(finalLeft));
                }
                else
                {
                    this.server.ReplaceImages(GetBytes(finalLeft), GetBytes(finalRight));
                }



                // calculate FPS
                if ((DateTime.Now - this.currentTime).TotalMilliseconds > 1000)
                {
                    fpsLabelValue = this.fps + " FPS";
                    this.fps = 0;
                    this.currentTime = DateTime.Now;
                }
                else
                {
                    this.fps++;
                }

                // imageLeft = imageLeft.WarpAffine(mat, Emgu.CV.CvEnum.Inter.Linear, Emgu.CV.CvEnum.Warp.FillOutliers, Emgu.CV.CvEnum.BorderType.Constant, new Bgra(0, 0, 0, 0));
                

                this.pictureBoxLeft.BeginInvoke((MethodInvoker)delegate
                {
                    //this.inputViewLeft.Image = imageLeft;
                    //this.inputViewRight.Image = imageRight;
                    if (this.previewCheck.Checked)
                    {
                        if (finalLeft != null)
                        {
                            // Bitmap clone = finalLeft.Clone() as Bitmap;
                            this.pictureBoxLeft.Image = finalLeft;
                        }
                        if (finalRight != null)
                        {
                            //this.pictureBoxRight.Image = finalRight;
                        }
                    }
                    //this.depthViewLeft.Image = depth;

                    this.fpsLabel.Text = this.fpsLabelValue;


                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }



        private void clearBackground_Click(object sender, EventArgs e)
        {
            this.wrapper.ResetBackground();
        }

        private void setup()
        {
            var config = new int[]
            {
                Properties.Settings.Default.Threshold,
                Properties.Settings.Default.Depth,
                Properties.Settings.Default.Erode,
                Properties.Settings.Default.Dilate,
                Properties.Settings.Default.Contrast,
                1,
                Properties.Settings.Default.Cleanup
            };

            Properties.Settings.Default.Save();

            if (this.wrapper != null)
            {
                this.wrapper.Setup(config);
            }

        }

        private void thresholdValueBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Threshold = this.thresholdTrack.Value;
            this.toolTipThreshold.SetToolTip(this.thresholdTrack, this.thresholdTrack.Value.ToString());

            this.setup();
        }

        private void cleanupTrackbar_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Cleanup = this.cleanupTrackbar.Value;
            this.cleanupToolTip.SetToolTip(this.cleanupTrackbar, this.cleanupTrackbar.Value.ToString());

            this.setup();
        }

        private void depthValueBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Depth = this.depthTrack.Value;
            this.depthToolTip.SetToolTip(this.depthTrack, this.depthTrack.Value.ToString());

            this.setup();
        }

        private void erodeBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Erode = this.erodeTrack.Value;
            this.erodeToolTip.SetToolTip(this.erodeTrack, this.erodeTrack.Value.ToString());

            this.setup();
        }

        private void dilateBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Dilate = this.dilateTrack.Value;
            this.dilateToolTip.SetToolTip(this.dilateTrack, this.dilateTrack.Value.ToString());

            this.setup();
        }

        private void contrast_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Contrast = this.contrastTrack.Value;
            this.contrastToolTip.SetToolTip(this.contrastTrack, this.contrastTrack.Value.ToString());

            this.setup();
        }

        private void SaveSettings(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }



        private void sourceSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
        }



        private void startZed()
        {
            int[] setup = {
                Properties.Settings.Default.Resolution,
                Properties.Settings.Default.DepthQuality,
                Properties.Settings.Default.DepthMode,
                Properties.Settings.Default.UseCuda ? 1 : 0,
                Properties.Settings.Default.SideBySide ? 1 : 0,
                Properties.Settings.Default.Camera
            };

            this.wrapper = new Wrapper(setup, null);
            this.Start();
        }

        private void RestartZed()
        {
            if (this.wrapper == null)
            {
                return;
            }

            //this.running = false;
            //this.serverMessage.Text = "Stopping Zed";
            //new Thread(() =>
            //{
            //    Thread.Sleep(1000);
            //    if (this.wrapper != null)
            //    {
            //        this.wrapper.cleanup();
            //    }
            //    this.LogServerMessage("Starting Zed");
            //    Thread.Sleep(1000);
            //    this.startZed();
            //}).Start();
            MessageBox.Show("Please Restart Your Application to Apply Changes");
        }

        private void resolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Resolution = this.resolution.SelectedIndex;
            Properties.Settings.Default.Save();

            if (File.Exists("left_color.tif"))
            {
                File.Delete("left_color.tif");
            }
            if (File.Exists("right_color.tif"))
            {
                File.Delete("right_color.tif");
            }

            this.RestartZed();
        }

        private void depthQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DepthQuality = this.depthQuality.SelectedIndex;
            Properties.Settings.Default.Save();

            this.RestartZed();
        }

        private void depthMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DepthMode = this.depthMode.SelectedIndex;
            Properties.Settings.Default.Save();

            this.RestartZed();
        }

        private void useCuda_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseCuda = this.useCuda.Checked;
            Properties.Settings.Default.Save();

            this.RestartZed();
        }

        private void sideBySideCheck_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SideBySide = this.sideBySideCheck.Checked;
            Properties.Settings.Default.Save();

            this.RestartZed();
        }

        private void previewCheck_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Preview = this.previewCheck.Checked;
            Properties.Settings.Default.Save();
        }

        private void camera_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Camera = this.camera.SelectedIndex;
            Properties.Settings.Default.Save();

            this.RestartZed();
        }
    }
}
