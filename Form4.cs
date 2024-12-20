using System;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using NAudio.Wave;

namespace ATM
{
    public partial class Form4 : Form
    {
        private Timer timer;
        private Timer blinkTimer = new Timer();
        private Color blinkColor;
        private SoundPlayer player;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        public Form4()
        {
            InitializeComponent();
            InitializeTimer();
            HideAllElements();
            blinkTimer.Interval = 500;
            blinkTimer.Tick += BlinkTimer_Tick;
            blinkColor = pictureBox7.BackColor;
            blinkTimer.Start();
            outputDevice = new WaveOutEvent();
            audioFile = new AudioFileReader(@"D:\Архів\КНЕУ\Курсовий проєкт 2024\ATM\bin\Debug\atm.mp3");
            audioFile.Volume = 0.2f;
            outputDevice.Init(audioFile);
            outputDevice.Play();
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            if (pictureBox7.BackColor == blinkColor)
            {
                Color customColor = Color.FromArgb(0, 50, 50);
                pictureBox7.BackColor = customColor;
            }
            else
            {
                pictureBox7.BackColor = blinkColor;
            }
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            label1.Visible = true;
            label2.Visible = true;
            pictureBox5.Visible = true;
            pictureBox7.Visible = true;
            pictureBox6.Visible = true;
            pictureBox2.Visible = true;
            pictureBox4.Visible = true;
            pictureBox3.Visible = true;
            timer.Stop();
            Timer countdownTimer = new Timer();
            countdownTimer.Interval = 13000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            Timer countdownTimer = (Timer)sender;
            countdownTimer.Stop();
            this.Hide();
            Form5 form5 = new Form5();
            form5.Show();
        }

        private void HideAllElements()
        {
            label1.Visible = false;
            label2.Visible = false;
            pictureBox5.Visible = false;
            pictureBox7.Visible = false;
            pictureBox6.Visible = false;
            pictureBox2.Visible = false;
            pictureBox4.Visible = false;
            pictureBox3.Visible = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Application.Exit();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}