using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;

namespace ZedTester
{
    public class ImageServer
    {
        // constants

        const int SEND_RECEIVE_COUNT = 15;

        // fields

        public bool enableLog = false;
        public int port = 8010;
        public string[] folders;
        private TcpListener listner;
        private bool stop = false;
        private List<TcpClient> clients = new List<TcpClient>();
        private Action<string> LogMessage;

        private Bitmap leftBitmap;
        private byte[] PendingLeftEye;
        private Bitmap rightBitmap;
        private byte[] PendingRightEye;
        private ZedUI zed;

        private int eyes;

        // contructor

        public ImageServer(ZedUI zed, Action<string> logger, int eyes)
        {
            this.zed = zed;
            this.LogMessage = logger;
            this.eyes = eyes;

            // Connect to the server
            listner = new TcpListener(IPAddress.Any, port);
            listner.Start();

            //Start sending coroutine
            new Thread(WaitForClients) { IsBackground = true }.Start();
        }

        // public methods

        public void ReplaceImages(byte[] left, byte[] right)
        {
            this.PendingLeftEye = left;
            this.PendingRightEye = right;
        }

        public void ReplaceImage(Bitmap bmp, byte[] left)
        {
            this.leftBitmap = bmp;
            this.PendingLeftEye = left;
        }

        // private methods


        //Converts the data size to byte array and put result to the fullBytes array
        private void ByteLengthToFrameByteArray(int byteLength, byte[] fullBytes)
        {
            //Clear old data
            Array.Clear(fullBytes, 0, fullBytes.Length);
            //Convert int to bytes
            byte[] bytesToSendCount = BitConverter.GetBytes(byteLength);
            //Copy result to fullBytes
            bytesToSendCount.CopyTo(fullBytes, 0);
        }

        //Converts the byte array to the data size and returns the result
        private int FrameByteArrayToByteLength(byte[] frameBytesLength)
        {
            int byteLength = BitConverter.ToInt32(frameBytesLength, 0);
            return byteLength;
        }

        private void WaitForClients()
        {
            byte[] LeftEye = null;
            byte[] RightEye = null;

            bool isConnected = false;
            TcpClient client = null;
            NetworkStream stream = null;

            // Wait for client to connect in another Thread 
            new Thread(() =>
            {
                while (!stop)
                {
                    this.LogMessage("Waiting for clients at port: " + this.port);

                    // Wait for client connection
                    client = listner.AcceptTcpClient();
                    // We are connected
                    clients.Add(client);

                    isConnected = true;
                    stream = client.GetStream();
                }
            })
            { IsBackground = true }.Start();

            //Wait until client has connected
            while (!isConnected)
            {
                Thread.Sleep(1);
            }

            this.LogMessage("Connected!");

            bool readyToGetFrame = true;
            byte[] frameBytesLength = new byte[SEND_RECEIVE_COUNT];

            byte eye = 0;

            while (!stop)
            {
                //Wait for End of frame
                Thread.Sleep(1);

                // get a fresh pair of eyes 
                if (eye == 0)
                {
                    LeftEye = this.PendingLeftEye;
                    RightEye = this.PendingRightEye;
                }

                if (LeftEye == null)
                {
                    if (this.eyes == 1)
                    {
                        this.LogMessage("Waiting for a new image");
                    }
                    else
                    {
                        this.LogMessage("Waiting for a fresh pair of eyes");
                    }
                    continue;
                }


                // PEFFORMANCE LOW
                // currentTexture.SetPixels(textures[index++].GetPixels());
                // byte[] pngBytes = currentTexture.EncodeToPNG();

                var pngBytes = eye == 0 ? LeftEye : RightEye;

                //Fill total byte length to send. Result is stored in frameBytesLength
                this.ByteLengthToFrameByteArray(pngBytes.Length, frameBytesLength);

                //Set readyToGetFrame false
                readyToGetFrame = false;

                new Thread(() =>
                {
                    try
                    {
                        //Send total byte count first
                        stream.Write(frameBytesLength, 0, frameBytesLength.Length);
                        // this.LogMessage("Sent Image byte Length: " + frameBytesLength.Length);

                        //Send the image bytes
                        stream.Write(pngBytes, 0, pngBytes.Length);

                        this.zed.ShowPreview(this.leftBitmap);

                        // log
                        this.LogMessage("Sending Image: " + pngBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        // reset to start getting new eye
                        eye = 0;
                    }

                    //Sent. Set readyToGetFrame true
                    readyToGetFrame = true;
                }).Start();

                // increase index if we have processed both images
                if (this.eyes == 2)
                {
                    eye = eye == 0 ? (byte)1 : (byte)0;
                }

                //Wait until we are ready to get new frame(Until we are done sending data)
                while (!readyToGetFrame)
                {
                    this.LogMessage("Waiting to get new frame");
                    Thread.Sleep(1);
                }
            }
        }


        //private void Update() {
        //    myImage.texture = texture;
        //}

        // stop everything
        private void OnApplicationQuit()
        {
            stop = true;

            if (listner != null)
            {
                listner.Stop();
            }

            foreach (TcpClient c in clients)
                c.Close();
            //}
        }
    }
}