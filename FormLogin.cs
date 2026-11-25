using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace МагазинПодкрадулей
{
    public partial class FormLogin : Form
    {

        private string connectionString = $"Data Source={Path.Combine(Application.StartupPath, "Магазин_подкрадулей.db")}; Version=3;";
        public FormLogin()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните оба поля", "!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "SELECT Роль, ФИО FROM Пользователи WHERE Логин = @Login AND Пароль = @Password";

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        // Очищаем на всякий случай и добавляем параметры заново
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Login", login);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                string role = dt.Rows[0]["Роль"].ToString();
                                string fio = dt.Rows[0]["ФИО"].ToString();

                                MessageBox.Show($"Добро пожаловать, {fio}!", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                FormMain formMain = new FormMain(role, fio);
                                formMain.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка БД: {ex.Message}", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}


