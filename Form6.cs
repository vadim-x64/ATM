using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form6 : Form
    {
        private bool isPanelVisible = false;
        private TextBox activeTextBox;
        private const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf;Integrated Security=True;Connect Timeout=30";

        public Form6()
        {
            InitializeComponent();
            textBox1.KeyPress += textBox1_KeyPress;
            textBox2.KeyPress += textBox2_KeyPress;
            activeTextBox = textBox1;
            textBox1.Click += TextBox_Click;
            textBox2.Click += TextBox_Click;
            button6.Click += NumberButton_Click;
            button7.Click += NumberButton_Click;
            button8.Click += NumberButton_Click;
            button9.Click += NumberButton_Click;
            button10.Click += NumberButton_Click;
            button11.Click += NumberButton_Click;
            button12.Click += NumberButton_Click;
            button13.Click += NumberButton_Click;
            button14.Click += NumberButton_Click;
            button15.Click += NumberButton_Click;
            button20.Click += NumberButton_Click;
            button18.Click += button18_Click;
            textBox2.KeyDown += textBox2_KeyDown;
            textBox1.ContextMenu = new ContextMenu();
            textBox2.ContextMenu = new ContextMenu();
        }

        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (activeTextBox.Text == "" || activeTextBox.Text != "0")
            {
                activeTextBox.Text += button.Text;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            e.Handled = true;
            activeTextBox.Focus();
            int selectionStart = textBox.SelectionStart;
            if (e.KeyChar == (char)Keys.Back)
            {
                if (selectionStart > 0)
                {
                    textBox.Text = textBox.Text.Remove(selectionStart - 1, 1);
                    textBox.SelectionStart = selectionStart - 1;
                }
            }
            else if (char.IsDigit(e.KeyChar) || (textBox == textBox1 && e.KeyChar == '+'))
            {
                if (textBox.Text.Length < 13) 
                {
                    textBox.Text = textBox.Text.Insert(selectionStart, e.KeyChar.ToString());
                    textBox.SelectionStart = selectionStart + 1;
                }
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '+')
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            e.Handled = true;
            activeTextBox.Focus();
            if (e.KeyChar == (char)Keys.Back)
            {
                if (textBox.SelectionLength == textBox.TextLength)
                {
                    activeTextBox.Text = "";
                }
                else
                {
                    activeTextBox.Text = activeTextBox.Text.Substring(0, activeTextBox.Text.Length - 1);
                }
            }
            else if (char.IsDigit(e.KeyChar))
            {
                if (activeTextBox.Text.Length < 6)
                {
                    activeTextBox.Text += e.KeyChar;
                }
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox2.Text.Length >= 6 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            activeTextBox = (TextBox)sender;
        }

        private async void PerformTransfer()
        {
            string phoneNumber = textBox1.Text;
            string transferAmountText = textBox2.Text;
            if (string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrWhiteSpace(transferAmountText))
            {
                MessageBox.Show("Заповніть номер телефону та суму переказу!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Будь ласка, введіть номер телефону!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(transferAmountText))
            {
                MessageBox.Show("Будь ласка, введіть суму для переказу!");
                return;
            }
            if (!IsValidPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Невірний номер телефону. Будь ласка, введіть його у форматі +380XXXXXXXXX!");
                return;
            }
            if (!int.TryParse(transferAmountText, out int transferAmount) || transferAmount < 50)
            {
                MessageBox.Show("Помилка поповнення рахунку. Мінімальна сума для зарахування - 50 грн!");
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    bool isPhoneNumberValid = CheckIfPhoneNumberExists(phoneNumber);
                    if (!isPhoneNumberValid)
                    {
                        MessageBox.Show("Вказаний номер телефону не знайдено!");
                        return;
                    }
                    string updateQuery = "UPDATE [dbo].[Table] SET [bill] = ISNULL([bill], 0) + @TransferAmount, [balance] = [balance] - @TransferAmount WHERE [phone] = @PhoneNumber AND [balance] >= @TransferAmount";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@TransferAmount", transferAmount);
                        updateCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"На телефон успішно зараховано {transferAmount} грн!");
                            await Task.Delay(2000);
                            Form5 form5 = new Form5();
                            form5.Show();
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Недостатньо коштів на рахунку. Будь ласка, перевірте суму переказу або спробуйте меншу суму!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка: {ex.Message}");
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+380\d{9}$");
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                PerformTransfer();
            }
        }

        private bool CheckIfPhoneNumberExists(string phoneNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM [dbo].[Table] WHERE [phone] = @PhoneNumber";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        int count = (int)checkCommand.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Під час перевірки номера телефону сталася помилка: {ex.Message}");
                return false;
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

        private void button17_Click(object sender, EventArgs e)
        {
            PerformTransfer();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (isPanelVisible)
            {
                panel8.Hide();
                isPanelVisible = false;
            }
            else
            {
                panel8.Show();
                isPanelVisible = true;
            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            panel8.Visible = false;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            activeTextBox.Text = string.Empty;
        }

        private void button16_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeTextBox.Text.Length > 0)
            {
                activeTextBox.Text = activeTextBox.Text.Substring(0, activeTextBox.Text.Length - 1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5();
            form5.ShowDialog();
        }
    }
}