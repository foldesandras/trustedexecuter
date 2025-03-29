using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace texec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Run(string cmd, ProcessStartInfo customPsi = null)
        {
            // Alapértelmezett ProcessStartInfo a cmd.exe futtatásához
            ProcessStartInfo defaultPsi = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = "cmd.exe",
                Arguments = $"/C start {cmd}"
            };

            // Ha van egy custom Psi, akkor az összes beállítást átmásoljuk
            if (customPsi != null)
            {
                defaultPsi = customPsi;
                // Ha a customPsi nem tartalmaz fájlnevet vagy parancsot, beállítjuk a default-ot
                if (string.IsNullOrEmpty(customPsi.FileName))
                {
                    defaultPsi.FileName = "cmd.exe";
                }
                if (string.IsNullOrEmpty(customPsi.Arguments))
                {
                    defaultPsi.Arguments = $"/C start {cmd}";
                }
                defaultPsi.UseShellExecute = true;
            }

            // Elindítjuk a folyamatot a végleges ProcessStartInfo-val
            Process.Start(defaultPsi);
        }


        private void RunNormally(string cmd)
        {
            Run(cmd);
        }

        private void RunAdmin(string cmd)
        {
            Run(cmd, new ProcessStartInfo() { Verb = "RunAs" });
        }

        private void RunTrustedInstaller(string cmd)
        {
            string ecmd = Convert.ToBase64String(Encoding.UTF8.GetBytes(cmd));
            string payload = $"import-module ntobjectmanager;sc.exe start trustedinstaller;$p = get-ntprocess -name trustedinstaller.exe;$proc = new-win32process (\"cmd.exe /C start \" + [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(\"{ecmd}\"))) -creationflags newconsole -parentprocess $p;";
            string epayload = Convert.ToBase64String(Encoding.Unicode.GetBytes(payload));
            RunAdmin("powershell -EncodedCommand " + epayload);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cmd = cmdTxtBox.Text;
            if (string.IsNullOrEmpty(cmd))
            {
                MessageBox.Show("Cannot run air!", "TrustedExecuter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string mode = modeComboBox.Text;
            switch (mode)
            {
                case "Run normally":
                    RunNormally(cmd);
                    break;
                case "Run as Administrator":
                    RunAdmin(cmd);
                    break;
                case "Run as TrustedInstaller":
                    if (MessageBox.Show("Running stuff as TrustedInstaller is sketchy and if you do not know what you are doing, you could easily brick your system. Do you want to continue?", 
                        "TrustedExecuter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        RunTrustedInstaller(cmd);
                    }
                    break;
                default:
                    MessageBox.Show($"Cannot run stuff in '{mode}' mode!", "TrustedExecuter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
