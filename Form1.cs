using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NulityCrash {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            TopMost = true;
            var processes = Process.GetProcessesByName("unity");
            foreach(var p in processes) {
                KillAllProcessesSpawnedBy((UInt32)p.Id);
            }
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e) {
            if(Process.GetProcessesByName("unity").Length == 0) {
                label1.Text = "No Nulity Process Open";
                return;
            } else {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C taskkill /IM unity.exe /F & unity.exe";
                process.StartInfo = startInfo;
                startInfo.CreateNoWindow = true;
                process.Start();
                label1.Text = "No Nulity Process Open";
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void KillAllProcessesSpawnedBy(UInt32 parentProcessId) {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + parentProcessId);
            ManagementObjectCollection collection = searcher.Get();
            if(collection.Count > 0) {
                foreach(var item in collection) {
                    UInt32 childProcessId = (UInt32)item["ProcessId"];
                    if((UInt32)childProcessId != Process.GetCurrentProcess().Id) {
                        KillAllProcessesSpawnedBy(childProcessId);
                        Process childProcess = Process.GetProcessById((int)childProcessId);
                        label2.Text = childProcessId.ToString();
                        childProcess.Kill();
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e) {

        }

        private void timer1_Tick(object sender, EventArgs e) {
            var processes = Process.GetProcessesByName("unity");
            foreach(var p in processes) {
                try {
                    label3.Text = $"Physical memory usage     : {p.WorkingSet64 / (1024 * 1024)} Mb";
                    label4.Text = $"Base priority             : {p.BasePriority}";
                    label5.Text = $"Priority class            : {p.PriorityClass}";
                    label6.Text = $"User processor time       : {p.UserProcessorTime.Hours} : {p.UserProcessorTime.Minutes} : {p.UserProcessorTime.Seconds}";
                    label7.Text = $"Privileged processor time : {p.PrivilegedProcessorTime.Hours} : {p.PrivilegedProcessorTime.Minutes} : {p.PrivilegedProcessorTime.Seconds}";
                    label8.Text = $"Total processor time      : {p.TotalProcessorTime.Hours} : {p.TotalProcessorTime.Minutes} : {p.TotalProcessorTime.Seconds}";
                    label9.Text = $"Paged system memory size  : {p.PagedSystemMemorySize64 / (1024 * 1024)} Mb";
                    label10.Text = $"Paged memory size         : {p.PagedMemorySize64 / (1024 * 1024)} Mb";
                    if(p.Responding) {
                        button1.BackColor = Color.Black;
                    } else {
                        button1.BackColor = Color.Red;
                    }
                }
                catch(Exception f) { }
            }
            if(processes.Length != 0) {
                button1.Enabled = true;
                label1.Text = "Unity Found!";
            } else {
                button1.Enabled = false;
            }
        }

        public bool IsProcessResponding(Process process) {
            if(process.MainWindowHandle == IntPtr.Zero) {
                return (true);
            } else {
                if(!process.Responding) {
                    Console.WriteLine("Process " + process.ProcessName +
                      " is not responding.");
                    return (false);
                } else {
                    Console.WriteLine("Process " + process.ProcessName +
                      " is responding.");
                    return (true);
                }
            }
        }
    }
}
