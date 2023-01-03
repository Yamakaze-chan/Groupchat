using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TCPSever;
using System.IO;
using System.Text.RegularExpressions;

namespace TCPSever
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpServer sever;
        string temp_string;
        string pp_send;
        int receivedataimg;

        private void btnStart_Click(object sender, EventArgs e)
        {
            sever.Start();
            txtInfo.Text += $"Starting...{Environment.NewLine}";
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }

        MemoryStream memoryStream = new MemoryStream(0);

        private void Form1_Load(object sender, EventArgs e)
        {
            txtIP.Text.Trim();
            btnStart.Enabled = true;
            btnSend.Enabled = false;
            sever = new SimpleTcpServer(txtIP.Text);
            sever.Events.ClientConnected += Events_ClientConnected; ;
            sever.Events.ClientDisconnected += Events_ClientDisconnected;
            sever.Events.DataReceived += Events_DataReceived;
            
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //var str = System.Text.Encoding.Default.GetString(e.Data);
                //string receive = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                //bool isimg = false;
                //if (receive.Contains("iushcxlchiasdchjaslfcdajhiodcadshjca"))
                //{
                //    isimg = true;
                //    receive = Regex.Replace(receive, "[^0-9.]", "");
                //}

                //if(!isimg)
                //{
                //    string receive = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                //    txtInfo.Text += $"{e.IpPort} : {receive}{Environment.NewLine}";
                //    pp_send = e.IpPort;
                //    temp_string = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                //    btnSend_Click(sender, e);
                //}
                //else
                //{
                pp_send = e.IpPort;
                memoryStream.Capacity = memoryStream.Capacity+e.Data.Count;
                //while (e.Data.Count > 0)
                //{
                    memoryStream.Write(e.Data.Array, 0, e.Data.Count);



                    //MessageBox.Show(e.Data.Count.ToString());

                    //if (memoryStream.Length == int.Parse(receive))
                    //{
                    //    pictureBox1.Image = Image.FromStream(fs);
                    //}
                    //}
                    try
                    {
                    Image.FromStream(memoryStream);
                    //var myUniqueFileName = $@"{DateTime.Now.Ticks}.png";
                    //Thread.Sleep(10);
                    //MessageBox.Show("Done");
                    //string path = "C:\\Users\\ACER\\Desktop\\picture\\outfile.png";
                    string path = "outfile.png";
                    FileStream fs = new FileStream(path, FileMode.Create);
                    fs.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    //memoryStream.SetLength(0);
                    fs.Close();
                    //using (FileStream fs = new FileStream(@"C:\Users\ACER\Desktop\picture\outfile.png", FileMode.Create))
                    //{
                    //    fs.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    //    //fs.Flush();
                    //}
                    //MessageBox.Show((memoryStream.Capacity-receivedataimg).ToString());
                    if(receivedataimg == memoryStream.Capacity)
                    {
                        //Thread.Sleep(1000);
                        sendFile(path);
                        memoryStream.Close();
                        memoryStream.Dispose();
                        memoryStream = new MemoryStream(0);
                    }
                    }
                    catch (Exception ex)
                    {
                    //Thread.Sleep(10);
                    
                    string receive = Encoding.UTF8.GetString(memoryStream.ToArray());
                    //MessageBox.Show(memoryStream.Capacity.ToString());
                    if (receive.Contains("iushcxlchiasdchjaslfcdajhiodcadshjca"))
                    {
                        //MessageBox.Show(receive);
                        receivedataimg = int.Parse(Regex.Replace(receive, "[^0-9.]", ""))+ memoryStream.Capacity;
                        //Thread.Sleep(30);
                        MessageBox.Show(this,"File has been sent");
                        txtInfo.Text += $"{e.IpPort} : send picture with {receivedataimg} bytes {Environment.NewLine}";
                        memoryStream.Close();
                        memoryStream.Dispose();
                        memoryStream = new MemoryStream(receivedataimg);
                    }
                    else
                    {
                        txtInfo.Text += $"{e.IpPort} : {receive}{Environment.NewLine}";
                        //pp_send = e.IpPort;
                        temp_string = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                        btnSend_Click(sender, e);
                        memoryStream.SetLength(0);
                    }
                }


                //memoryStream.Capacity = memoryStream.Capacity + e.Data.Length;

                //FileStream fs = new FileStream(@"C:\Users\ACER\Desktop\picture\outfile.png", FileMode.Create);
                //fs.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                ////pictureBox1.Image = Image.FromStream(fs);
                //fs.Close();
                //    MessageBox.Show("Send success");
                //}

            });
            //memoryStream.SetLength(0);
            //MessageBox.Show(e.Data.Count.ToString());
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} connected.{Environment.NewLine}";
                listClientIP.Items.Add(e.IpPort);
            });
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //memoryStream.SetLength(0);
                txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                listClientIP.Items.Remove(e.IpPort);
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(sever.IsListening)
            {
                if(listClientIP.Items.Count >0)
                {
                    foreach (var item in listClientIP.Items)
                    {
                        string ip = listClientIP.GetItemText(item);
                        if(ip == pp_send)
                        {
                            continue;
                        }
                        string textmsg = pp_send + "//////" + temp_string;
                        sever.Send(ip, textmsg);
                        txtInfo.Text += $"{pp_send} {ip} {txtMessage.Text}{Environment.NewLine}";
                        txtMessage.Text = "";
                    }
                }
            }
        }

        private void sendFile(string filePath)
        {
            if(sever.IsListening)
            {
                if(listClientIP.Items.Count > 0)
                {
                    
                    foreach (var item in listClientIP.Items)
                    {
                        string ip = listClientIP.GetItemText(item);
                        if (ip == pp_send)
                        {
                            continue;
                        }
                        FileStream fs = new FileStream(filePath, FileMode.Open); ;
                        long totalBytes = fs.Length;
                        string senddata = "iushcxlchiasdchjaslfcdajhiodcadshjca" + totalBytes.ToString();
                        byte[] byteArray = Encoding.UTF8.GetBytes(senddata);
                        MemoryStream stream = new MemoryStream(byteArray);
                        //MessageBox.Show(stream.Length.ToString());
                        sever.Send(ip,stream.Length, stream);
                        Thread.Sleep(50);
                        sever.Send(ip, fs.Length, fs);
                        Thread.Sleep(50);
                        Thread.Sleep(200);
                        fs.Close();
                    }
                    
                    
                }
            }
        }

        public static bool IsValidImage(MemoryStream ms)
        {
            try
            {
                    Image.FromStream(ms);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
    }
}
