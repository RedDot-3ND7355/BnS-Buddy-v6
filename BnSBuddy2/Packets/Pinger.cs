using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BnSBuddy2.Packets
{
    public class Pinger
    {
        // Globals
        string IP = "184.73.104.101";
        private Stopwatch stopwatch = new Stopwatch();
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        BackgroundWorker pingworker = new BackgroundWorker();
        // End Globals
        public void PingRegionChange(string region)
        {
            Form1.CurrentForm.materialLabel8.Text = "Pinging...";
            Form1.CurrentForm.materialLabel91.Text = "Pinging...";
            if (region == "Europe")
                IP = "18.194.180.254";
            if (region == "North America")
                IP = "184.73.104.101";
            if (region == "Taiwan")
                IP = "203.67.68.227";
            if (region == "Japanese")
                IP = "106.186.46.101";
            if (region == "Korean")
                IP = "222.122.231.3";
            if (region == "Russia")
                IP = "109.105.133.64";
            if (region == "Chinese")
                IP = "0";
            if (region == "Rebellion")
                IP = "51.195.57.237";
            if (region == "Garena")
                IP = "0";
        }
        public void PingStatus(string ping)
        {
            if (ping == "Pinging...")
            {
                Form1.CurrentForm.materialLabel12.Text = "Offline";
                return;
            }
            if (ping == "Offline")
            {
                Form1.CurrentForm.materialLabel12.Text = "Offline";
                return;
            }
            int _ping = Convert.ToInt32(ping.Replace("ms", ""));
            if (_ping >= 120)
                Form1.CurrentForm.materialLabel12.Text = "Bad";
            else if (_ping >= 64 && _ping < 120)
                Form1.CurrentForm.materialLabel12.Text = "Playable";
            else if (_ping >= 1 && _ping < 64)
                Form1.CurrentForm.materialLabel12.Text = "Good";
            else
                Form1.CurrentForm.materialLabel12.Text = "Offline";
        }

        public void StartPinger()
        {
            pingworker = new BackgroundWorker();
            pingworker.WorkerSupportsCancellation = true;
            pingworker.WorkerReportsProgress = false;
            pingworker.DoWork += bw1_DoWork;
            pingworker.RunWorkerCompleted += Pingworker_RunWorkerCompleted;
            if (Form1.CurrentForm.materialSwitch41.Checked)
                pingworker.RunWorkerAsync();
        }

        private void Pingworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Form1.CurrentForm.materialSwitch41.Checked)
                pingworker.RunWorkerAsync();
        }

        private void bw1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Blocking = true;
                stopwatch.Restart();
                stopwatch.Start();
                socket.Connect(IP, 10100);
                stopwatch.Stop();
                Form1.CurrentForm.materialLabel8.Text = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds) + "ms";
                socket.Close();
                // Set Ping for GCD
                Form1.CurrentForm.materialLabel91.Text = Convert.ToInt32(Math.Round(Convert.ToInt32(Math.Round(Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds) * 2.833333333333333)) * 1.666666666666667)) + "ms";
                // Sleep timer
                Thread.Sleep(Convert.ToInt32(Form1.CurrentForm.materialScrollBar1.Value));
            } 
            catch
            {
                Form1.CurrentForm.materialLabel8.Text = "Offline";
                Form1.CurrentForm.materialLabel91.Text = "Offline";
                Thread.Sleep(Convert.ToInt32(Form1.CurrentForm.materialScrollBar1.Value));
            }
        }

        public void StopPinger()
        {
            pingworker.CancelAsync();
            Form1.CurrentForm.materialLabel8.Text = "Offline";
            Form1.CurrentForm.materialLabel91.Text = "Offline";
        }
    }
}
