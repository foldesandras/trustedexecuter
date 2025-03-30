using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace texec
{
    public partial class Form1 : Form
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr PathGetArgsW(IntPtr pszPath);

        public Form1()
        {
            InitializeComponent();
        }

        public static (string programName, string arguments) GetProgramAndArguments(string cmdLine)
        {
            // Parancssori sztring
            IntPtr cmdLinePtr = Marshal.StringToHGlobalUni(cmdLine);

            // Kinyerjük az argumentumokat a PathGetArgsW segítségével
            IntPtr lpArg = PathGetArgsW(cmdLinePtr);

            if (lpArg != IntPtr.Zero)
            {
                // Az argumentumok karakterlánccá alakítása
                string args = Marshal.PtrToStringUni(lpArg);

                // A program nevének kinyerése (az argumentumok levonásával)
                string programName = cmdLine.Substring(0, cmdLine.Length - args.Length).Trim();

                // Ha az első és utolsó karakter idézőjel, eltávolítjuk őket
                if (programName.StartsWith("\"") && programName.EndsWith("\""))
                {
                    programName = programName.Substring(1, programName.Length - 2);
                }

                return (programName, args);
            }
            else
            {
                return (cmdLine, string.Empty);  // Ha nincs argumentum
            }
        }

        private void Run(string cmd, ProcessStartInfo customPsi = null)
        {
            (string app, string args) = GetProgramAndArguments(cmd);
            // Alapértelmezett ProcessStartInfo a cmd.exe futtatásához
            ProcessStartInfo defaultPsi = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = app,
                Arguments = args
            };

            // Ha van egy custom Psi, akkor az összes beállítást átmásoljuk
            if (customPsi != null)
            {
                defaultPsi = customPsi;
                // Ha a customPsi nem tartalmaz fájlnevet vagy parancsot, beállítjuk a default-ot
                if (string.IsNullOrEmpty(customPsi.FileName))
                {
                    defaultPsi.FileName = app;
                }
                if (string.IsNullOrEmpty(customPsi.Arguments))
                {
                    defaultPsi.Arguments = args;
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
            (string app, string args) = GetProgramAndArguments(cmd);
            if (string.IsNullOrEmpty(args))
            {
                args = " ";
            }
            string eapp = Convert.ToBase64String(Encoding.UTF8.GetBytes(app));
            string eargs = Convert.ToBase64String(Encoding.UTF8.GetBytes(args));
            string payload = $"import-module ntobjectmanager;sc.exe start trustedinstaller;$p = get-ntprocess -name trustedinstaller.exe;$proc = new-win32process (\"powershell.exe -C `\"Start-Process -FilePath ([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{eapp}'))) -ArgumentList @([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{eargs}'))) `\"\") -creationflags newconsole -parentprocess $p;";
            string epayload = Convert.ToBase64String(Encoding.Unicode.GetBytes(payload));
            RunAdmin("powershell -EncodedCommand " + epayload);
        }

        private void RunSystem(string cmd)
        {
            (string app, string args) = GetProgramAndArguments(cmd);
            if (string.IsNullOrEmpty(args))
            {
                args = " ";
            }
            string eapp = Convert.ToBase64String(Encoding.UTF8.GetBytes(app));
            string eargs = Convert.ToBase64String(Encoding.UTF8.GetBytes(args));
            string payload2 = $"Start-Process -FilePath ([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{eapp}'))) -ArgumentList @([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{eargs}')))";
            string epayload2 = Convert.ToBase64String(Encoding.Unicode.GetBytes(payload2));
            Console.WriteLine(epayload2);
            string payload = $"psexec.exe -s -i powershell.exe -EncodedCommand {epayload2}";
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
                case "Run as 'NT AUTHORITY\\SYSTEM'":
                    if (MessageBox.Show("Running stuff as 'NT AUTHORITY\\SYSTEM' is sketchy and if you do not know what you are doing, you could easily brick your system. Do you want to continue?",
                        "TrustedExecuter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        RunSystem(cmd);
                    }
                    break;
                default:
                    MessageBox.Show($"Cannot run stuff in '{mode}' mode!", "TrustedExecuter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
