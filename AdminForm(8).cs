using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ATM
{
    public partial class AdminForm_8_ : Form
    {
        LinearGradientBrush gradientBrush;
        private const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database2.mdf\";Integrated Security=True";
        private bool isView1Shown = true;

        public AdminForm_8_()
        {
            InitializeComponent();
            pictureBox1.Image = Image.FromFile("D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\bin\\Debug\\view2.png");
            this.Paint += new PaintEventHandler(AdminForm_8__Paint);
            this.Shown += AdminForm_8__Shown;
            textBox2.KeyDown += TextBox2_KeyDown;
            textBox2.KeyPress += TextBox2_KeyPress;
            textBox2.MaxLength = 16;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
                e.Handled = true;
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
                e.Handled = true;
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
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

        private void AdminForm_8__Shown(object sender, EventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            gradientBrush = new LinearGradientBrush(rect, Color.FromArgb(209, 241, 255), Color.FromArgb(255, 255, 214), LinearGradientMode.Vertical);
            string query = "SELECT [user] FROM [dbo].[Table] WHERE [Id] = 1";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        string username = (string)command.ExecuteScalar();

                        if (!string.IsNullOrEmpty(username))
                        {
                           label1.Text = username;
                        }
                        else
                        {
                            MessageBox.Show("Користувач з Id = 1 не знайдений.", "Помилка");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка під час отримання імені користувача: " + ex.Message, "Помилка");
                    }
                }
            }
        }

        private void AdminForm_8__Paint(object sender, PaintEventArgs e)
        {
            if (gradientBrush != null)
            {
                e.Graphics.FillRectangle(gradientBrush, this.ClientRectangle);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string password = textBox2.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Не введено пароль!", "Помилка");
                return;
            }
            string query = "SELECT COUNT(*) FROM [dbo].[Table] WHERE [password] = @Password";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", password);
                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        if (count > 0)
                        {
                            AdminForm_9_ adminForm = new AdminForm_9_();
                            adminForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Неправильний пароль. Будь ласка, спробуйте ще раз!", "Помилка");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка під час перевірки входу: " + ex.Message, "Помилка");
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (isView1Shown)
            {
                pictureBox1.Image = Image.FromFile("D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\bin\\Debug\\view1.png");
                textBox2.PasswordChar = '\0';
            }
            else
            {
                pictureBox1.Image = Image.FromFile("D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\bin\\Debug\\view2.png");
                textBox2.PasswordChar = '•';
            }
            isView1Shown = !isView1Shown;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = panel2.ClientRectangle;
            Color darkColor = Color.FromArgb(200, 255, 255);
            Color lightColor = Color.FromArgb(255, 255, 235);
            LinearGradientBrush gradientBrush = new LinearGradientBrush(rect, darkColor, lightColor, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(gradientBrush, rect);
        }
    }
}