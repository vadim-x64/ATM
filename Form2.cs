using System;
using System.Linq;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;
using System.Data.SqlClient;

namespace ATM
{
    public partial class Form2 : Form
    {
        private bool isTextBox1Active = true;
        private int incorrectAttempts = 0;
        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf;Integrated Security=True;Connect Timeout=30";
        
        public Form2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (isTextBox1Active)
            {
                if (textBox1.Text.All(char.IsDigit))
                {
                    if (textBox1.Text.Length < 4)
                    {
                        textBox1.Text += button.Text;
                    }
                }
                else
                {
                    textBox1.Text = button.Text;
                }
            }
            else
            {
                if (textBox2.Text.Length < 16 && button.Text.All(char.IsDigit))
                {
                    textBox2.Text += button.Text;
                }
            }
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            isTextBox1Active = true;
        }

        private void TextBox2_Enter(object sender, EventArgs e)
        {
            isTextBox1Active = false;
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

        private void Form2_Load(object sender, EventArgs e)
        {
            button5.Click += Button_Click;
            button6.Click += Button_Click;
            button7.Click += Button_Click;
            button8.Click += Button_Click;
            button9.Click += Button_Click;
            button10.Click += Button_Click;
            button11.Click += Button_Click;
            button12.Click += Button_Click;
            button13.Click += Button_Click;
            button14.Click += Button_Click;
            button15.Click += button15_Click;
            button16.MouseUp += button16_MouseUp;
            button17.Click -= button17_Click;
            button17.Click += button17_Click;
            panel4.Visible = false;
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox2.KeyPress += TextBox2_KeyPress;
            textBox1.Enter += TextBox1_Enter;
            textBox2.Enter += TextBox2_Enter;
            textBox1.KeyDown += TextBox1_KeyDown;
            textBox2.KeyDown += TextBox2_KeyDown;
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            textBox1.ContextMenuStrip = contextMenuStrip;
            textBox2.ContextMenuStrip = contextMenuStrip;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && ((TextBox)sender).SelectionLength <= 0)
            {
                e.Handled = false;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && ((TextBox)sender).SelectionLength <= 0)
            {
                e.Handled = false;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox1.Text.Length >= 4 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                button17_Click(sender, e);
            }
            if (e.KeyChar == (char)Keys.V && ModifierKeys.HasFlag(Keys.Control))
            {
                e.Handled = true;
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox2.Text.Length >= 16 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; 
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (isTextBox1Active)
            {
                textBox1.Text = string.Empty;
            }
            else
            {
                textBox2.Text = string.Empty;
            }
        }

        private void button16_MouseUp(object sender, MouseEventArgs e)
        {
            if (isTextBox1Active && !string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            }
            else if (!isTextBox1Active && !string.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.Text = textBox2.Text.Substring(0, textBox2.Text.Length - 1);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string enteredCard = textBox2.Text;
            string enteredPin = textBox1.Text;
            if (string.IsNullOrWhiteSpace(enteredCard) && string.IsNullOrWhiteSpace(enteredPin))
            {
                MessageBox.Show("Будь ласка, введіть номер Вашої карти та пін-код!", "Інформація");
                return;
            }
            else if (string.IsNullOrWhiteSpace(enteredCard))
            {
                MessageBox.Show("Будь ласка, введіть номер Вашої карти!", "Інформація");
                return;
            }
            else if (string.IsNullOrWhiteSpace(enteredPin))
            {
                MessageBox.Show("Будь ласка, введіть пін-код Вашої карти!", "Інформація");
                return;
            }
            if (enteredCard.Length != 16)
            {
                MessageBox.Show("Номер Вашої карти має містити 16 цифр!", "Помилка");
                return;
            }
            if (enteredPin.Length == 4)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string selectQuery = "SELECT Id, pin, IsBlocked FROM [dbo].[Table] WHERE number = @CardNumber";
                        string updateQuery = "UPDATE [dbo].[Table] SET IsBlocked = 1 WHERE Id = @Id";
                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@CardNumber", enteredCard);
                            SqlDataReader reader = selectCommand.ExecuteReader();
                            if (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["Id"]);
                                string storedPin = reader["pin"].ToString();
                                bool isBlocked = Convert.ToBoolean(reader["IsBlocked"]);
                                reader.Close();
                                if (isBlocked)
                                {
                                    MessageBox.Show("Ваша карта заблокована. Будь ласка, зверніться в службу підтримки Вашого банку!", "Інформація");
                                    this.Hide();
                                    Form1 form1 = new Form1();
                                    form1.Show();
                                    return;
                                }
                                if (enteredPin == storedPin)
                                {
                                    Program.CardNumber = enteredCard;
                                    this.Hide();
                                    Form3 form3 = new Form3(userId);
                                    form3.Show();
                                }
                                else
                                {
                                    incorrectAttempts++;
                                    if (incorrectAttempts >= 3)
                                    {
                                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                        {
                                            updateCommand.Parameters.AddWithValue("@Id", userId);
                                            updateCommand.ExecuteNonQuery();
                                        }
                                        MessageBox.Show("Наразі Ваша карта заблокована. Будь ласка, зверніться в службу підтримки Вашого банку!", "Інформація");
                                        this.Hide();
                                        Form1 form1 = new Form1();
                                        form1.Show();
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Невірний пін-код. Після {3 - incorrectAttempts} спроб(и) Вашу карту буде заблоковано!", "Помилка");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Таку карту не знайдено в нашій базі. Будь ласка, введіть вірний номер Вашої карти!", "Помилка");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть пін-код Вашої карти, який повинен складатися з 4 цифр!", "Інформація");
                textBox1.Text = string.Empty;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            panel4.Visible = !panel4.Visible;
        }

        private void textBox2_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.SelectionLength <= 0)
            {
                textBox.SelectedText = string.Empty;
            }
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.SelectionLength <= 0)
            {
                textBox.SelectedText = string.Empty;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}