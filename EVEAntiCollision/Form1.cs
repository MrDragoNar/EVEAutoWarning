using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVEAntiCollision
{
    public partial class Form1 : Form
    {
        SynchronizationContext _syncContext = null;
        public Form1()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
        }

        #region 定义程序变量
        // 定义变量

        // 用来记录鼠标按下的坐标，用来确定绘图起点
        private Point DownPoint;

        // 用来表示是否截图完成
        private bool CatchFinished = false;

        //预警进程是否开启
        private bool WarningProcess = true;
        private bool WarningProcessCol = false;
        private bool HasEnemy = false;

        // 用来表示截图开始
        private bool CatchStart = false;

        // 用来保存原始图像
        private Bitmap originBmp;

        // 用来保存截图的矩形
        private Rectangle CatchRectangle;

        #endregion



        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        /// <summary>
        /// 生成0-z的随机字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns>随机字符串</returns>
        public static string GenerateRandomString(int length)
        {
            string checkCode = String.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                checkCode += constant[rd.Next(10)].ToString();
            }
            return checkCode;
        }

        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds-28800).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (WarningProcess)
            {
                //界面操作
                button4.Text = "预警已开启";
                button4.ForeColor = Color.Green;
                WarningProcess = false;
                String info = DateTime.Now.ToString("hh:mm:ss") + ":" + "预警开启\n Fly Safe！\n";
                this.richTextBox1.AppendText(info);
                //开启监控进程
                WarningProcessCol = true;
                Thread reviewthread = new Thread(picThread);
                reviewthread.IsBackground = true;
                reviewthread.Start();
            }
            else {
                //界面操作
                button4.Text = "预警已暂停";
                button4.ForeColor = Color.Red;
                WarningProcess = true;
                WarningProcessCol = false;
                String info = DateTime.Now.ToString("hh:mm:ss") + ":" + "预警暂停\n ------ \n";
                this.richTextBox1.AppendText(info);
            }

        }

        private void picThread()
        {
            while(WarningProcessCol)
            {
                    //启动截图
                    // 新建一个和屏幕大小相同的图片
                    Bitmap CatchBmp = null;
                    CatchBmp = new Bitmap(100, 800);
                    // 创建一个画板，让我们可以在画板上画图
                    // 这个画板也就是和屏幕大小一样大的图片
                    // 我们可以通过Graphics这个类在这个空白图片上画图
                    Graphics g = null;
                    g = Graphics.FromImage(CatchBmp);
                

                // 把屏幕图片拷贝到我们创建的空白图片 CatchBmp中
                    g.CopyFromScreen(200,300,0,0, new Size(100,700));
                //pictureBox1.BackgroundImage =CatchBmp;
                try
                {
                    if (this.pictureBox1.Image != null)
                    {
                        this.pictureBox1.Image.Dispose();
                        this.pictureBox1.Image = null;
                    }
                    this.pictureBox1.Image = (Image)CatchBmp;
                    Thread.Sleep(600);
                Bitmap btimap1 = CatchBmp;
                    if (GetRGB(btimap1, 149, 18, 23) == true)
                    {
                        Audio a = new Audio();
                        a.Play("sound.wav");
                        //子线程中通过UI线程上下文更新UI   
                        _syncContext.Post(SetLabelText, "");
                        //HasEnemy = true;
                    }
                    else {
                       
                    }

                g.Dispose();
                }
                catch (Exception e) { }
                Thread.Sleep(600);
                }

        }

        private void SetLabelText(object text)
        {
            String info = DateTime.Now.ToString("hh:mm:ss") + ":" + "来红！\n";
            this.richTextBox1.AppendText(info);
            richTextBox1.ScrollToCaret();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 改变鼠标样式
            this.Cursor = Cursors.Cross;

        }
        public bool GetRGB(Bitmap src, int tr, int tg, int tb)
        {
            bool has = false;

            int w = src.Width;
            int h = src.Height;
            System.Drawing.Imaging.BitmapData srcData = src.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* pIn = (byte*)srcData.Scan0.ToPointer();
                byte* p;
                int stride = srcData.Stride;
                int r, g, b;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        p = pIn;
                        b = pIn[0];
                        g = pIn[1];
                        r = pIn[2];
                        String Log = b + "-" + g + "-" + r;
                        //Console.WriteLine(Log);
                        //if (r == tr && g == tg && b == tb) { has = true; break; }
                        if (r > 125 && g < 20 && b < 30 ) { has = true; break; }
                        pIn += 4;
                    }
                    pIn += srcData.Stride - w * 4;
                }
            }
            src.UnlockBits(srcData);
            return has;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();

            f2.Show();
        }
    }
}
