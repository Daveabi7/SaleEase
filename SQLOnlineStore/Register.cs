using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SQLOnlineStore
{
    public partial class Register : Form
    {
        private SqlConnection sqlConnection = null;

        DataBase database = new DataBase();
        public Register()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Register_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);

            sqlConnection.Open();

            textBox_password2.PasswordChar = '*';
            pictureBox3.Visible = false;
            textBox_login2.MaxLength = 50;
            textBox_password2.MaxLength = 50;




        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            
            var login = textBox_login2.Text;
            var password = textBox_password2.Text;

            SqlCommand command = new SqlCommand($"insert into Registers(login_user, password_user, is_admin) values (N'{textBox_login2.Text}', N'{textBox_password2.Text}', 0)", sqlConnection);

            database.openConnection();

            if(command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Акаунт успішно створено!", "Успіх");
                Login frm_login = new Login();
                this.Hide();
                frm_login.ShowDialog();
            }
            else
            {
                MessageBox.Show("Акаунт не створено!");
            }
            database.closeConnection();
        }

        private Boolean checkuser()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            SqlCommand command = new SqlCommand($"select login_user from Registers where login_user = N'{textBox_login2.Text}'", sqlConnection);

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Користувач уже існує!");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox_login2.Text = "";
            textBox_password2.Text = "";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox_password2.UseSystemPasswordChar = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            textBox_password2.UseSystemPasswordChar = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login frm_sing = new Login();
            frm_sing.Show();
            this.Hide();
        }
    }
}
