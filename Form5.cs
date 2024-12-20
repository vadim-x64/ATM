using System;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
      
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 form6 = new Form6();
            form6.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 form7 = new Form7(Program.CardNumber);
            form7.ShowDialog();
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
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (Program.WithdrawalAmount <= 0)
            {
                MessageBox.Show("Жодних операцій не було здійснено!", "Помилка");
            }
            else
            {
                this.Hide();
                Form8 form8 = new Form8();
                form8.ShowDialog();
            }
        }
    }
}