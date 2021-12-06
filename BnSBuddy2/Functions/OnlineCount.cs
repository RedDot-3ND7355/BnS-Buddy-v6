using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace BnSBuddy2.Functions
{
    public class OnlineCount
    {
        // Globals 
        BackgroundWorker countworker = new BackgroundWorker();
        // End Globals

        // Start Count Function
        public void StartCount(bool manual = false)
        {
            if (countworker.IsBusy && manual)
            {
                countworker.CancelAsync();
                Form1.CurrentForm.AddLauncherLog("[Log] Refreshed Online User Count");
            }
            countworker = new BackgroundWorker();
            countworker.WorkerSupportsCancellation = true;
            countworker.WorkerReportsProgress = false;
            countworker.DoWork += GetCount;
            countworker.RunWorkerCompleted += Pingworker_RunWorkerCompleted;
            countworker.RunWorkerAsync();
        }

        // Get Online Count
        private void GetCount(object Sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    webClient.Headers.Add("User-Agent", "BnSBuddy");
                    bool Hidden = !Form1.CurrentForm.materialSwitch58.Checked;
                    string txt = webClient.DownloadString("https://bnsbuddy.com/count/usercount.php?hidden=" + Hidden.ToString().ToLower());
                    if (txt.All(char.IsDigit))
                        Form1.CurrentForm.materialButton4.Text = txt;
                    else
                        Form1.CurrentForm.materialButton4.Text = "Offline";
                }
                catch
                {
                    Form1.CurrentForm.materialButton4.Text = "Error";
                }
            }
            Thread.Sleep(18000);
        }

        // Restart Count
        private void Pingworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!countworker.IsBusy)
                countworker.RunWorkerAsync();
        }

    }
}
