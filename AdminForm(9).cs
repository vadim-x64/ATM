using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ATM
{
    public partial class AdminForm_9_ : Form
    {
        private const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Архів\\КНЕУ\\Курсовий проєкт 2024\\ATM\\Database1.mdf\";Integrated Security=True";
        private bool changesSaved = false;

        public AdminForm_9_()
        {
            InitializeComponent();
            LoadDataIntoGridView();
            this.TopMost = false;
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

        private void SaveChanges()
        {
            dataGridView1.EndEdit();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && !cell.Value.Equals(cell.Tag))
                    {
                        string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                        int userId = Convert.ToInt32(dataGridView1.Rows[cell.RowIndex].Cells["Id"].Value);
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string updateQuery = $"UPDATE [dbo].[Table] SET {columnName} = @NewValue WHERE Id = @UserId";
                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@NewValue", cell.Value);
                                command.Parameters.AddWithValue("@UserId", userId);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            changesSaved = true;
            MessageBox.Show("Зміни успішно збережено!");
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string newValue = cell.Value != null ? cell.Value.ToString() : string.Empty;
                int userId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = $"UPDATE [dbo].[Table] SET {columnName} = @NewValue WHERE Id = @UserId";
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@NewValue", newValue);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void LoadDataIntoGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM [dbo].[Table]";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!changesSaved)
            {
                DialogResult result = MessageBox.Show("Бажаєте зберегти зміни перед закриттям?", "Інформація", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    SaveChanges();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            this.Hide();
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
}