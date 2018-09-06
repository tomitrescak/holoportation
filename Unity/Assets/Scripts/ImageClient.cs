using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using System.Threading;

#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif

public class ImageClient : MonoBehaviour
{
    public RawImage imageLeft;
    public RawImage imageRight;

    public Material materialLeft;
    public Material materialRight;

    public bool enableLog = false;
    
    public Text text;

    public int port = 8010;
    public string IP = "localhost"; //"192.168.1.165";

    public int eyes = 2;

    Texture2D ltex;
    Texture2D rtex;

    private Stream reader;
    private bool stop = false;
    private byte current = 0;

    private bool readyToChange = false;
    private bool readyToReceive = false;

    private byte[] leftBytes;
    private byte[] rightBytes;

    //This must be the-same with SEND_COUNT on the server
    const int SEND_RECEIVE_COUNT = 15;
    private string message;

    private void Log(string message)
    {
        this.message = message;
    }

#if UNITY_EDITOR
    TcpClient client;

    // Use this for initialization
    void Start()
    {
        Loom.Initialize();
        Application.runInBackground = true;

        ltex = new Texture2D(0, 0);
        rtex = new Texture2D(0, 0);
        client = new TcpClient();

        //Connect to server from another Thread
        Loom.RunAsync(() =>
        {
            Log("Connecting to server " + this.IP + ":" + this.port );
            // if on desktop
            bool connected = false;
            while (!connected && !stop)
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(this.IP, port);
                    reader = client.GetStream();
                    connected = true;
                }
                catch (Exception ex)
                {
                    Log("Retrying connection: " + ex.Message);
                    Thread.Sleep(200);
                }
            }

            // if using the IPAD
            //client.Connect(IPAddress.Parse(IP), port);
            Log("");

            imageReceiver();
        });
    }

    void StopExchange()
    {
        if (!Application.isPlaying)
        {
            stop = true;
        }
    }


    void imageReceiver()
    {
        //While loop in another Thread is fine so we don't block main Unity Thread
        Loom.RunAsync(() =>
        {
            while (!stop)
            {
                //Read Image Count
                int imageSize = readImageByteSize(SEND_RECEIVE_COUNT);
                LogWarning("Received Image byte Length: " + imageSize);

                //Read Image Bytes and Display it
                ReadFrameByteArray(imageSize);
            }
        });
    }
    
    void OnApplicationQuit()
    {
        LogWarning("OnApplicationQuit");
        stop = true;

        if (client != null)
        {
            client.Close();
        }
    }

#endif

#if !UNITY_EDITOR
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task exchangeTask;
    private bool exchanging = false;
    private bool exchangeStopRequested = false;
    
    private string errorStatus = null;
    private string warningStatus = null;
    private string successStatus = null;
    private string unknownStatus = null;

    
    void Start()
    {
        Loom.Initialize();

        ltex = new Texture2D(0, 0);
        rtex = new Texture2D(0, 0);

        Connect(this.IP, this.port.ToString());
    }

    private async void Connect(string host, string port)
    {
        try
        {
            
            host = "192.168.137.1";
            if (exchangeTask != null) StopExchange();

            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);

            Log("Connecting to server " + serverHost + ":" + port);

            await socket.ConnectAsync(serverHost, port);

            Log("");
    
            reader = socket.InputStream.AsStreamForRead();
            // reader = new StreamReader(streamIn);

            RestartExchange();
            successStatus = "Connected!";
        }
        catch (Exception e)
        {
            Log("Error: " + e.ToString());
            errorStatus = e.ToString();
        }
    }

    

    public void RestartExchange()
    {

        if (exchangeTask != null) StopExchange();
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ExchangePackets());
    }

    public async void ExchangePackets()
    {
        readyToReceive = true;

        while (!exchangeStopRequested)
        {
            if (!readyToReceive)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                continue;
            }

            if (reader == null) continue;
            exchanging = true;
                
            int imageSize = readImageByteSize(SEND_RECEIVE_COUNT);
            LogWarning("Received Image byte Length: " + imageSize);
    
            //Read Image Bytes and Display it
            ReadFrameByteArray(imageSize);
        }
    }


    public void StopExchange()
    {
        exchangeStopRequested = true;

        if (exchangeTask != null)
        {
            exchangeTask.Wait();
            socket.Dispose();
            reader.Dispose();

            socket = null;
            exchangeTask = null;
        }
        reader = null;
    }

    public void OnDestroy()
    {
        StopExchange();
    }
    
    void OnApplicationQuit()
    {
        LogWarning("OnApplicationQuit");
        StopExchange();
    }

    //private async void DisplayImageOnMainThread(byte[] imageBytes, bool disconnected)
    //{
    //    bool readyToReadAgain = false;

    //    //Display Image
    //    if (!disconnected)
    //    {
    //        //Display Image on the main Thread
    //        Loom.MainAction = (() =>
    //        {
    //            displayReceivedImage(imageBytes);
    //            readyToReadAgain = true;
    //        });
    //    }

    //    //Wait until old Image is displayed
    //    while (!readyToReadAgain)
    //    {
    //        await Task.Delay(TimeSpan.FromMilliseconds(1));
    //    }
    //}

#endif

    public void FixedUpdate()
    {
        if (!Application.isPlaying)
        {
            StopExchange();
        }

        if (readyToChange)
        {
            readyToChange = false;
            
            ltex.LoadImage(leftBytes);

            if (eyes == 2)
            {
                rtex.LoadImage(rightBytes);
            }

            materialLeft.SetTexture("_MainTex", ltex);
            materialRight.SetTexture("_MainTex", eyes == 2 ? rtex : ltex);

            readyToReceive = true;
        }

        this.text.text = this.message;
    }

    //Converts the data size to byte array and put result to the fullBytes array
    void ByteLengthToFrameByteArray(int byteLength, byte[] fullBytes)
    {
        //Clear old data
        Array.Clear(fullBytes, 0, fullBytes.Length);
        //Convert int to bytes
        byte[] bytesToSendCount = BitConverter.GetBytes(byteLength);
        //Copy result to fullBytes
        bytesToSendCount.CopyTo(fullBytes, 0);
    }

    //Converts the byte array to the data size and returns the result
    int FrameByteArrayToByteLength(byte[] frameBytesLength)
    {
        int byteLength = BitConverter.ToInt32(frameBytesLength, 0);
        return byteLength;
    }


    /////////////////////////////////////////////////////Read Image SIZE from Server///////////////////////////////////////////////////
    private int readImageByteSize(int size)
    {
        bool disconnected = false;

        
        byte[] imageBytesCount = new byte[size];
        var total = 0;
        do
        {
            var read = reader.Read(imageBytesCount, total, size - total);
            //Debug.LogFormat("Client recieved {0} bytes", total);
            if (read == 0)
            {
                disconnected = true;
                break; 
            }

            total += read;
        } while (total != size);

        int byteLength;

        if (disconnected)
        {
            byteLength = -1;
        }
        else
        {
            byteLength = FrameByteArrayToByteLength(imageBytesCount);
        }

        return byteLength;
    }

    /////////////////////////////////////////////////////Read Image Data Byte Array from Server///////////////////////////////////////////////////
    private void ReadFrameByteArray(int size)
    {
        byte[] imageBytes = new byte[size];
        var total = 0;
        do
        {
            var read = reader.Read(imageBytes, total, size - total);
            //Debug.LogFormat("Client recieved {0} bytes", total);
            if (read == 0)
            {
                this.message = "Disconnected ...";
                break;
            }

            total += read;
        } while (total != size);

        Debug.Log("Received Image");
       
        // save byte to process on the main thread
        if (eyes == 1)
        {
            leftBytes = imageBytes;
            readyToReceive = false;
            readyToChange = true;
        }
        else if (current == 0)
        {
            leftBytes = imageBytes;
            current++;
        } 
        else
        {
            readyToReceive = false;
            rightBytes = imageBytes;
            readyToChange = true;
            current = 0;
        }
        
    }

    

    //void displayReceivedImage(byte[] receivedImageBytes)
    //{
    //    if (imageLeft != null)
    //    {
    //        ltex.LoadImage(receivedImageBytes);
    //        imageLeft.texture = ltex;
    //        imageRight.texture = ltex;
    //    }

    //    if (materialLeft != null)
    //    {
    //        if (current == 0)
    //        {
    //            ltex.LoadImage(receivedImageBytes);
    //            current = 1;
    //        }
    //        else
    //        {
    //            rtex.LoadImage(receivedImageBytes);
    //            materialLeft.SetTexture("_MainTex", ltex);
    //            materialRight.SetTexture("_MainTex", rtex);
    //            current = 0;
    //        }
    //    }
    //}


    void LOG(string messsage)
    {
        if (enableLog)
            Debug.Log(messsage);
    }

    void LogWarning(string message)
    {
        if (enableLog)
            Debug.LogWarning(message);
    }

    
}