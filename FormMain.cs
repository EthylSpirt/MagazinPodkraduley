using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace МагазинПодкрадулей
{
    public partial class FormMain : Form
    {

        private string connectionString = $"Data Source={Path.Combine(Application.StartupPath, "Магазин_подкрадулей.db")}; Version=3;";

        private string userRole;
        private string userLogin;

        public FormMain(string login, string role)
        {
            InitializeComponent();
            this.userLogin = login;
            this.userRole = role;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (labelUserInfo != null)
            {
                labelUserInfo.Text = $"Пользователь: {userLogin} ({userRole})";
            }

            LoadProducts();

            ApplyPermissions();
        }

        private void LoadProducts(string searchQuery = "")
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT * FROM Товар";

                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        sql += " WHERE Наименование LIKE @search";
                    }

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");
                        }

                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridViewProducts.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProducts(textBoxSearch.Text.Trim());
        }

        private void ApplyPermissions()
        {
            if (userRole != "Администратор" && userRole != "Admin")
            {
                if (buttonAddProduct != null) buttonAddProduct.Visible = false;
                if (buttonEditProduct != null) buttonEditProduct.Visible = false;
                if (buttonDeleteProduct != null) buttonDeleteProduct.Visible = false;
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
