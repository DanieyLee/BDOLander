using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hssm
{
    public partial class StartGame : Form
    {
        IniFiles ini = new IniFiles(Application.StartupPath + @"\config.ini");
        string fileContent = "@echo off\r\ncd bin64\r\nstart BlackDesert64.exe ";
        string filePath = Application.StartupPath + @"\bin64\run.bat";
        public StartGame()
        {
            InitializeComponent();
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private void StartGame_MouseDown(object sender, MouseEventArgs e)//按下拖动
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF010 + 0x0002, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("请输入账号密码！","提醒信息");
                return;
            }
            if (checkBox1.Checked)
            {
                ini.IniWriteValue("登入信息", "login", textBox1.Text.Trim());
                ini.IniWriteValue("登入信息", "password", textBox2.Text.Trim());
            }else
            {
                ini.IniWriteValue("登入信息", "login", "");
                ini.IniWriteValue("登入信息", "password", "");
            }
            this.Visible = false;
            ExcuteDosCommand(textBox1.Text.Trim()+", "+textBox2.Text.Trim());
        }
        public void ExcuteDosCommand(string cmd)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine(fileContent+cmd);//开始写入值
            sr.Close();
            fs.Close();
            Process p = new Process();//设定调用的程序名，不是系统目录的需要完整路径 
            p.StartInfo.FileName = filePath;//传入执行参数 
            p.StartInfo.Arguments = "";
            p.StartInfo.UseShellExecute = false;//是否重定向标准输入 
            p.StartInfo.RedirectStandardInput = false;//是否重定向标准转出 
            p.StartInfo.RedirectStandardOutput = false;//是否重定向错误 
            p.StartInfo.RedirectStandardError = false;//执行时是不是显示窗口 
            p.StartInfo.CreateNoWindow = true;//启动 
            p.Start();
            p.WaitForExit();
            p.Close();
            this.Dispose();
            this.Close();
        }

        private void StartGame_Load(object sender, EventArgs e)
        {
            if (!ini.ExistINIFile())
            {
                ini = new IniFiles(Application.StartupPath + @"\config.INI");
                ini.IniWriteValue("登入信息","login","");
                ini.IniWriteValue("登入信息", "password", "");
            }else
            {
                textBox1.Text = ini.IniReadValue("登入信息","login");
                textBox2.Text = ini.IniReadValue("登入信息", "password");
            }
            
            if (!File.Exists(filePath))
            {
                FileStream fs1 = new FileStream(filePath, FileMode.Create, FileAccess.Write);//创建写入文件
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(fileContent);//开始写入值
                sw.Close();
                fs1.Close();
            }
        }
    }
}
