using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form3 : Form
    {
        public int SelectedAmount { get; private set; }
        private int userId;
        private bool isBalanceVisible = false;
        private bool button1Clicked = false;
        private bool button2Clicked = false;
        private bool button3Clicked = false;

        public Form3(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            AttachNumericButtonClickEventHandlers();
            textBox1.MouseDown += TextBox1_MouseDown;
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int result))
            {
                textBox1.Text = result.ToString();
                textBox1.Select(textBox1.Text.Length, 0);
                Program.TransactionType = "ОПЕРАЦІЯ З ВИДАЧІ ГОТІВКИ";
            }
            else
            {
                textBox1.Text = "0";
            }
        }

        private void TextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                textBox1.ContextMenu = new ContextMenu();
            }
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

        private void Form3_Load(object sender, EventArgs e)
        {
            panel8.Visible = false;
            textBox1.Text = "0";
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox1.KeyDown += TextBox1_KeyDown;
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT name FROM [dbo].[Table] WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        label7.Text = $"{name}";
                    }
                }
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                if (textBox1.Text == "0")
                {
                    textBox1.Text = e.KeyChar.ToString();
                }
                else if (textBox1.Text.Length < 6)
                {
                    textBox1.Text += e.KeyChar;
                }
                e.Handled = true; 
                textBox1.Select(textBox1.Text.Length, 0);
            }
            else
            {
                e.Handled = true; 
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                e.Handled = true;
                if (textBox1.Text.Length > 0)
                {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1); 
                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        textBox1.Text = "0";
                    }
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                CheckAmount();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            panel8.Visible = !panel8.Visible;
        }

        private void NumericButtonClick(object sender, EventArgs e)
        {
            Button numericButton = (Button)sender;
            if (textBox1.Text.Length < 6)
            {
                if (textBox1.Text == "0" || string.IsNullOrEmpty(textBox1.Text))
                {
                    textBox1.Text = numericButton.Text;
                }
                else
                {
                    textBox1.Text += numericButton.Text;
                }
            }
        }

        private void BackspaceButtonClick(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            }
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "0";
            }
        }

        private async void CheckAmount()
        {
            if (textBox1.Text == "0")
            {
                MessageBox.Show("Будь ласка, введіть суму для зняття!", "Інформація");
            }
            else
            {
                int amount;
                if (int.TryParse(textBox1.Text, out amount))
                {
                    Program.WithdrawalAmount = amount;
                    if (amount < 200)
                    {
                        MessageBox.Show("Мінімальна сума для зняття - 200 грн!", "Помилка");
                        textBox1.Text = "0";
                    }
                    else if (amount % 200 != 0)
                    {
                        MessageBox.Show("Введена невірна сума. Сума повинна бути кратною 200!", "Помилка");
                        textBox1.Text = "0";
                    }
                    else if (amount > 5000)
                    {
                        MessageBox.Show("Ви не можете зняти більше 5000 грн за один раз!", "Інформація");
                        textBox1.Text = "0";
                    }
                    else
                    {
                        int currentBalance = GetAccountBalance();
                        if (currentBalance < amount)
                        {
                            MessageBox.Show("Недостатньо коштів на рахунку!", "Помилка");
                        }
                        else
                        {
                            UpdateAccountBalance(currentBalance - amount);
                            await Task.Delay(2000);
                            this.Hide();
                            Form4 form4 = new Form4();
                            form4.ShowDialog();
                        }
                    }
                }
            }
        }

        private int GetAccountBalance()
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT balance FROM [dbo].[Table] WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private void UpdateAccountBalance(int newBalance)
        {
            Program.CurrentBalance = newBalance;
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE [dbo].[Table] SET balance = @Balance WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Balance", newBalance);
                    command.Parameters.AddWithValue("@Id", userId); 
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ClearAmount()
        {
            textBox1.Text = "0";
        }

        private void AttachNumericButtonClickEventHandlers()
        {
            button5.Click += NumericButtonClick;
            button6.Click += NumericButtonClick;
            button7.Click += NumericButtonClick;
            button8.Click += NumericButtonClick;
            button9.Click += NumericButtonClick;
            button10.Click += NumericButtonClick;
            button11.Click += NumericButtonClick;
            button12.Click += NumericButtonClick;
            button13.Click += NumericButtonClick;
            button14.Click += NumericButtonClick;
            button16.Click += BackspaceButtonClick;
            button17.Click += (sender, e) => CheckAmount();
            button15.Click += (sender, e) => ClearAmount();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1Clicked)
                return;
            SelectedAmount = 200;
            DeductAmountFromAccount(SelectedAmount);
            button1Clicked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2Clicked)
                return;
            SelectedAmount = 1000;
            DeductAmountFromAccount(SelectedAmount);
            button2Clicked = true;
        }

        private async void DeductAmountFromAccount(int amountToDeduct)
        {
            Program.WithdrawalAmount = amountToDeduct;
            int currentBalance = GetAccountBalance();
            if (currentBalance < amountToDeduct)
            {
                MessageBox.Show("Недостатньо коштів на рахунку!", "Помилка");
            }
            else
            {
                int newBalance = currentBalance - amountToDeduct;
                Program.CurrentBalance = newBalance;
                Program.TransactionType = "ОПЕРАЦІЯ З ВИДАЧІ ГОТІВКИ";
                UpdateAccountBalance(newBalance);
                await Task.Delay(2000);
                this.Hide();
                Form4 form4 = new Form4();
                form4.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3Clicked)
                return;
            isBalanceVisible = !isBalanceVisible;
            int currentBalance = GetAccountBalance();
            label3.Text = isBalanceVisible ? $"Баланс: {currentBalance} грн" : "";
            label3.Visible = isBalanceVisible;
            button20.Text = isBalanceVisible ? "Приховати баланс" : "Перевірити баланс";
            label3.Font = new Font("Cambria", 24, FontStyle.Regular);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5();
            form5.ShowDialog();
        }
    }
}