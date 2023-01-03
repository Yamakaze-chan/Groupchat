using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;

namespace Chat
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        List<string> lsIPport = new List<string>();
        List<Socket> lsSocket = new List<Socket>();
        bool cansend = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = GetLocalIP();
            textBox4.Text = GetLocalIP();
            textBox7.Text = GetLocalIP();
            
        }

        private void pingforIP(string ip, string port)
        {
            if (String.IsNullOrEmpty(ip) == false)
            {
                Ping ping = new Ping();
                PingReply rep = ping.Send(ip, 1000);
                if (rep != null && rep.Status.ToString() != "TimedOut")
                {
                    //rep.Status.ToString() == "Connected"
                    //MessageBox.Show("Connected");
                    //MessageBox.Show("Status :  " + rep.Status + " \n Time : " + rep.RoundtripTime.ToString() + " \n Address : " + rep.Address);

                    Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    //textBox1.Text = GetLocalIP();
                    //textBox4.Text = ip;
                    bool successflag = false;
                    try
                    {
                        epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text));
                        sck.Bind(epLocal);
                        //Connecting to remote IP
                        epRemote = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
                        sck.Connect(epRemote);
                        successflag = true;
                        buffer = new byte[1000000];
                        button1.Text = "Connected";
                        button1.Enabled = false;
                        sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                        
                    }
                    catch (Exception ex)
                    {
                        successflag = false;
                        MessageBox.Show(ex.ToString());
                    }
                    if(successflag)
                    {
                        string temp = ip+":"+ port;
                        lsIPport.Add(temp);
                        lsSocket.Add(sck);
                        cansend = true;
                        listBox1.Items.Add(temp);
                        //lsIPport.ForEach(p => MessageBox.Show(p));
                    }
                }
                else
                {
                    MessageBox.Show("ERR");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string ip = textBox4.Text;
            string port1 = textBox3.Text;
            pingforIP(ip, port1);
            string ip2 = textBox7.Text;
            string port2 = textBox6.Text;
            pingforIP(ip2, port2);
        }

        private void insertpicture(MemoryStream ms)
        {
            System.Windows.Forms.PictureBox picture = new System.Windows.Forms.PictureBox()
            {
                Name = "pictureBox",
                MaximumSize = new Size(1000, 1000),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(100, 100),

            };
            picture.Image = Image.FromStream(ms);
            this.flowLayoutPanel1.Controls.Add(picture);
            //pictureBox1.Image = Image.FromStream(ms);
        }


        private void MessageCallBack(IAsyncResult aResult)
        {
            lsIPport.ForEach(p => MessageBox.Show(p));
            bool flag = true;
            bool isnull = false;
            //Socket temp_socket;
            try
            {
                byte[] receiveData = new byte[1000000];
                bool isimg = false;


                //Socket listener = (Socket)aResult.AsyncState;
                //Socket handler = listener.EndAccept(aResult);
                //MessageBox.Show(listener.ToString());
                //handler.RemoteEndPoint;

                receiveData = (byte[])aResult.AsyncState;
                ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                string receiveMessage = aSCIIEncoding.GetString(receiveData).TrimEnd('\0');
                int index1 = receiveMessage.IndexOf("******");
                string ipport = receiveMessage.Substring(0, index1);
                int ipnum = ipport.IndexOf(':');
                string ipaddress = ipport.Substring(0,ipnum);
                string port = ipport.Substring(ipnum+1);
                //MessageBox.Show(ipaddress + ":" + port);
                
                foreach(Socket s in lsSocket)
                {
                    if(((IPEndPoint)s.RemoteEndPoint).Address.ToString()==ipaddress && ((IPEndPoint)s.RemoteEndPoint).Port.ToString() == port)
                    {
                        sck = s;
                        MessageBox.Show(ipaddress + ":" + ipport);
                    }
                }
                //MessageBox.Show(receiveData.Length.ToString());
                receiveMessage = receiveMessage.Substring(index1 + 6);
                if (receiveMessage == string.Empty)
                {
                    MessageBox.Show("User is offline");
                    cansend = false;
                }
                else
                {
                    if (receiveMessage.Contains("iushcxlchiasdchjaslfcdajhiodcadshjca"))
                    {
                        receiveMessage = Regex.Replace(receiveMessage, "[^0-9.]", "");
                        isimg = true;
                        //MessageBox.Show(receiveMessage);
                    }
                    //MessageBox.Show(receiveData.ToString());
                    if (isimg)
                    {
                        //MessageBox.Show("picturre");
                        flag = false;
                        MemoryStream mStream = new MemoryStream(0);
                        int num;
                        long count = 0;


                        byte[] buffer = new byte[1000000];
                        while ((num = sck.Receive(buffer, 0, buffer.Length, SocketFlags.None)) != 0)
                        {

                            mStream.Write(buffer, 0, num);
                            Thread.Sleep(10);
                            //pictureBox1.Image = Image.FromStream(mStream);
                            count = 0 + mStream.Length;
                            if (mStream.Length == int.Parse(receiveMessage))
                            {
                                if (this.InvokeRequired)
                                {
                                    this.BeginInvoke((MethodInvoker)delegate ()
                                    {
                                        insertpicture(mStream);
                                        //Closesocket();
                                        //Beginsocket();
                                    });
                                }
                                else
                                {
                                    insertpicture(mStream);
                                    //Closesocket();
                                    //Beginsocket();
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                                //MessageBox.Show(receiveMessage.Length.ToString());
                                lb.Text = "Friend: " + receiveMessage;
                                lb.MaximumSize = new Size(843, 0);
                                lb.AutoSize = true;
                                this.flowLayoutPanel1.Controls.Add(lb);
                                //Closesocket();
                                //Beginsocket();
                            });
                        }
                        else
                        {
                            System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                            lb.Text = "Friend: " + receiveMessage;
                            lb.MaximumSize = new Size(843, 0);
                            lb.AutoSize = true;
                            lb.BorderStyle = BorderStyle.FixedSingle;
                            this.flowLayoutPanel1.Controls.Add(lb);
                            //Closesocket();
                            //Beginsocket();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Socket sk in lsSocket)
            {
                if (cansend == false)
                {
                    MessageBox.Show("User has offline");
                }
                else
                {
                    //Convert string to byte[]
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    byte[] sendingMessage = new byte[1000000];
                    if (string.IsNullOrEmpty(textBox5.Text) == false)
                    {
                        string sending = textBox1.Text + ":" + textBox2.Text + "******"+ textBox5.Text;
                        sendingMessage = aSCIIEncoding.GetBytes(sending);
                        //Send
                        sk.Send(sendingMessage);
                        //MessageBox.Show(sending);

                    }
                    else
                    {
                        MessageBox.Show("String not null");
                    }
                }
            }
            //adding to listbox
            int line = textBox5.Lines.Count();
            System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
            lb.Text = "Me: " + textBox5.Text;
            lb.MaximumSize = new Size(843, 0);
            lb.AutoSize = true;
            lb.BorderStyle = BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(lb);
            textBox5.Text = "";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox5.Text))
            {
                Size sz = new Size(textBox5.ClientSize.Width, int.MaxValue);
                TextFormatFlags flags = TextFormatFlags.WordBreak;
                int padding = 3;
                int borders = textBox5.Height - textBox5.ClientSize.Height;
                sz = TextRenderer.MeasureText(textBox5.Text, textBox5.Font, sz, flags);
                int h = sz.Height + borders + padding;
                if (textBox5.Top + h > this.ClientSize.Height - 10)
                {
                    h = this.ClientSize.Height - 10 - textBox5.Top;
                }
                textBox5.Height = h;
            }
            else
            {
                textBox5.Height = 22;
            }
        }


        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        /*
        private void button3_Click(object sender, EventArgs e)
        {
            string sSelectedFile;
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
                sSelectedFile = choofdlog.FileName;
            else
                sSelectedFile = string.Empty;
            bool fact = SendFile(IPAddress.Parse(textBox4.Text), sSelectedFile);

            //MessageBox.Show("DONE!!!!");
        }
        
        public bool SendFile(IPAddress deviceAddr, string filePath)
        {
            int lastStatus = 0;
            //FileStream file = new FileStream(filePath, FileMode.Open); ;
            Image original = Image.FromFile(filePath);
            Image resized = ResizeImage(original, new Size(Width/2, Height/2));
            FileStream fileStream = new FileStream(filePath+"_resized.JPG", FileMode.Create); //I use file stream instead of Memory stream here
            resized.Save(fileStream, ImageFormat.Jpeg);
            fileStream.Close(); //close after use
            FileStream file = new FileStream(filePath + "_resized.JPG", FileMode.Open); ;
            long totalBytes = file.Length, bytesSoFar = 0;
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            byte[] senddata = new byte[1000000];
            senddata = aSCIIEncoding.GetBytes("iushcxlchiasdchjaslfcdajhiodcadshjca" + totalBytes.ToString());
            sck.Send(senddata);
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
            progressBar1.Step = 1;
            progressBar1.Style = ProgressBarStyle.Blocks;
            //IPEndPoint endpoint = new IPEndPoint(deviceAddr, Convert.ToInt32(textBox3.Text));
            //Socket sock = new Socket(deviceAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //sck.SendTimeout = 1000000; //timeout in milliseconds
            try
            {
                //sock.Connect(endpoint);
                byte[] filechunk = new byte[4096];
                int numBytes;
                while ((numBytes = file.Read(filechunk, 0, 4096)) > 0)
                {
                    if (sck.Send(filechunk, numBytes, SocketFlags.None) != numBytes)
                    {
                        throw new Exception("Error in sending the file");
                    }
                    bytesSoFar += numBytes;
                    Byte progress = (byte)(bytesSoFar * 100 / totalBytes);
                    if (progress > lastStatus && progress != 100)
                    {
                        //MessageBox.Show("File sending progress:{0}"+ lastStatus);
                        progressBar1.Value = lastStatus;
                        lastStatus = progress;
                    }
                    Thread.Sleep(30);
                }
                //MessageBox.Show("total " + bytesSoFar);
                //sock.Shutdown(SocketShutdown.Both);
                file.Close(); //close after use
                File.Delete(filePath + "_resized.JPG");
            }
            catch (SocketException e)
            {
                MessageBox.Show("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            System.Windows.Forms.PictureBox picture = new System.Windows.Forms.PictureBox()
            {
                Name = "pictureBox",
                MaximumSize = new Size(1000, 1000),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(100, 100),

            };
            picture.Image = Image.FromFile(filePath);
            this.flowLayoutPanel1.Controls.Add(picture);
            return true;
        }

        private static byte[] ReceiveVarData(Socket s)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];

            recv = s.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, 0);
                if (recv == 0)
                {
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }

        public static bool IsValidImage(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                    Image.FromStream(ms);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        public Image ConvertByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new System.Drawing.Bitmap(image, newWidth, newHeight); // I specify the new image from the original together with the new width and height
            using (Graphics graphicsHandle = Graphics.FromImage(image))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(newImage, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private void Beginsocket()
        {
            
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //binding Socket
                epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text));
                sck.Bind(epLocal);
                //Connecting to remote IP
                epRemote = new IPEndPoint(IPAddress.Parse(textBox4.Text), Convert.ToInt32(textBox3.Text));
                sck.Connect(epRemote);

                //Listeneing the specific port
                buffer = new byte[1000000];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
        }

        private void Closesocket()
        {
            try
            {
                sck.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                sck.Close();
            }
        }

        bool SocketConnected(Socket s)
        {

            try
            {
                bool part1 = s.Poll(1000, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sck != null)
            {
                Closesocket();
            }
        }


        private void Defeat()
        {
            MessageBox.Show("Goodbye");
            Application.Exit();
        }
        */
    }
}
