using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;

namespace ATM
{
    public partial class Form7 : Form
    {
        public string CardNumber { get; set; }
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf;Integrated Security=True;Connect Timeout=30";
        private TextBox activeTextBox;

        public Form7(string cardNumber)
        {
            InitializeComponent();
            CardNumber = cardNumber;
            label1.Text = CardNumber;
            if (Program.CardNumber != null)
            {
                label1.Text = Program.CardNumber;
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

        private bool IsCardExists(string cardNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM [dbo].[Table] WHERE number = @CardNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private int GetCardBalance(string cardNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT balance FROM [dbo].[Table] WHERE number = @CardNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private void UpdateCardBalance(string cardNumber, int newBalance)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE [dbo].[Table] SET balance = @NewBalance WHERE number = @CardNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewBalance", newBalance);
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string sourceCardNumber = label1.Text;
            string destinationCardNumber = textBox2.Text;
            string transferAmountText = textBox3.Text;
            int transferAmount;
            if (string.IsNullOrWhiteSpace(destinationCardNumber) && string.IsNullOrWhiteSpace(transferAmountText))
            {
                MessageBox.Show("Будь ласка, введіть номер картки-одержувача та суму для переказу!", "Помилка");
                return;
            }
            else if (string.IsNullOrWhiteSpace(destinationCardNumber))
            {
                MessageBox.Show("Будь ласка, введіть номер картки-одержувача!", "Помилка");
                return;
            }
            else if (destinationCardNumber.Length != 16) {
                MessageBox.Show("Номер карти повинен складатися з 16 цифр!", "Помилка");
                return;
            }
            else if (string.IsNullOrWhiteSpace(transferAmountText))
            {
                MessageBox.Show("Будь ласка, введіть суму для переказу!", "Помилка");
                return;
            }
            if (!int.TryParse(transferAmountText, out transferAmount) || transferAmount <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректну суму для переказу!", "Помилка");
                return;
            }
            if (!IsCardExists(sourceCardNumber))
            {
                MessageBox.Show("Карта, з якої ви хочете здійснити переказ, не існує!", "Помилка");
                return;
            }
            if (!IsCardExists(destinationCardNumber))
            {
                MessageBox.Show("Такої карти не існує. Спробуйте ще раз!", "Помилка");
                return;
            }
            if (sourceCardNumber == destinationCardNumber)
            {
                MessageBox.Show("Ви не можете здійснювати переказ на ту ж саму карту!", "Помилка");
                return;
            }
            int sourceBalance = GetCardBalance(sourceCardNumber);
            if (sourceBalance < transferAmount)
            {
                MessageBox.Show("Недостатньо коштів на рахунку для здійснення переказу!", "Помилка");
                return;
            }
            UpdateCardBalance(sourceCardNumber, sourceBalance - transferAmount);
            int destinationBalance = GetCardBalance(destinationCardNumber);
            UpdateCardBalance(destinationCardNumber, destinationBalance + transferAmount);
            MessageBox.Show($"Переказ на {destinationCardNumber} здійснено успішно!", "Інформація");
            Thread.Sleep(2000);
            Form5 form5 = new Form5();
            form5.Show();
            this.Hide();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            button6.Click += Button_Click;
            button7.Click += Button_Click;
            button8.Click += Button_Click;
            button9.Click += Button_Click;
            button10.Click += Button_Click;
            button11.Click += Button_Click;
            button12.Click += Button_Click;
            button13.Click += Button_Click;
            button14.Click += Button_Click;
            button15.Click += Button_Click;
            textBox2.Click += TextBox_Click;
            textBox3.Click += TextBox_Click;
            textBox2.KeyPress += textBox2_KeyPress;
            textBox3.KeyPress += textBox3_KeyPress;
            textBox2.TextChanged += textBox2_TextChanged;
            textBox3.TextChanged += textBox3_TextChanged;
            panel8.Visible = false;
            textBox2.ContextMenu = new ContextMenu();
            textBox3.ContextMenu = new ContextMenu();
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            activeTextBox = sender as TextBox;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (activeTextBox != null && button != null)
            {
                activeTextBox.Text += button.Text;
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (activeTextBox != null)
            {
                activeTextBox.Text = string.Empty;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if ((e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete) && textBox.SelectionLength > 0)
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                textBox.Text = textBox.Text.Remove(start, length);
                textBox.SelectionStart = start;
                e.Handled = true;
            }
            else if ((e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete) && textBox.SelectionLength == 0 && textBox.Text.Length > 0)
            {
                int cursorPosition = textBox.SelectionStart;
                textBox.Text = textBox.Text.Remove(cursorPosition - 1, 1);
                textBox.SelectionStart = cursorPosition - 1;
                e.Handled = true;
            }
            else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) || textBox.Text.Length >= 16)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                button17_Click(sender, e);
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if ((e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete) && textBox.SelectionLength > 0)
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                textBox.Text = textBox.Text.Remove(start, length);
                textBox.SelectionStart = start;
                e.Handled = true;
            }
            else if ((e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete) && textBox.SelectionLength == 0 && textBox.Text.Length > 0)
            {
                int cursorPosition = textBox.SelectionStart;
                textBox.Text = textBox.Text.Remove(cursorPosition - 1, 1);
                textBox.SelectionStart = cursorPosition - 1;
                e.Handled = true;
            }
            else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) || textBox.Text.Length >= 6)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                button17_Click(sender, e);
            }
        }

        private void button16_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeTextBox != null && activeTextBox.SelectionLength > 0)
            {
                int selectionStart = activeTextBox.SelectionStart;
                int selectionLength = activeTextBox.SelectionLength;
                string text = activeTextBox.Text;
                string newText = text.Substring(0, selectionStart) + text.Substring(selectionStart + selectionLength);
                activeTextBox.Text = newText;
                activeTextBox.SelectionStart = selectionStart;
                activeTextBox.SelectionLength = 0; 
            }
            else if (activeTextBox != null && activeTextBox.SelectionStart > 0)
            {
                int selectionStart = activeTextBox.SelectionStart;
                string text = activeTextBox.Text;
                activeTextBox.Text = text.Substring(0, selectionStart - 1) + text.Substring(selectionStart);
                activeTextBox.SelectionStart = selectionStart - 1;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string input = textBox2.Text;
            string result = string.Empty;
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    result += c;
                }
            }
            textBox2.Text = result;
            textBox2.SelectionStart = textBox2.Text.Length;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 1 && textBox.Text[0] == '0')
            {
                textBox.Text = string.Empty;
                return;
            }
            string input = textBox.Text;
            string result = string.Empty;
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    result += c;
                }
            }
            textBox.Text = result;
            textBox.SelectionStart = textBox.Text.Length;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            panel8.Visible = !panel8.Visible;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5();
            form5.ShowDialog();
        }
    }
}