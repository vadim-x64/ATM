using System;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form8 : Form
    {
        public DateTime entryTime;
        Timer animationTimer = new Timer();
        int animationSpeed = 3;

        public Form8()
        {
            InitializeComponent();
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5();
            form5.ShowDialog();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            entryTime = DateTime.Now;
            label2.Text = entryTime.ToString("dd/MM/yyyy     HH:mm:ss");
            GenerateRandomNumbers();
            GenerateRandomCheck();
            label4.Text = $"- {Program.WithdrawalAmount} грн";
            label5.Text = $"ЗАЛИШОК: {Program.CurrentBalance} грн";
            label6.Text = $"{Program.TransactionType}";
            animationTimer.Interval = 10;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
            panel3.Top = -panel3.Height;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (panel3.Top < this.ClientSize.Height - panel3.Height)
            {
                panel3.Top += animationSpeed;
            }
            else
            {
                animationTimer.Stop();
            }
        }

        private void GenerateRandomNumbers()
        {
            Random random = new Random();
            string randomNumbers = "ПН ";
            for (int i = 0; i <= 10; i++)
            {
                randomNumbers += random.Next(10);
            }
            label1.Text = randomNumbers;
        }

        private void GenerateRandomCheck()
        {
            Random r = new Random();
            string randomCheck = "#";
            for (int i = 0; i <= 5; i++)
            {
                randomCheck += r.Next(10);
            }
            label8.Text = randomCheck;
        }
    }
}