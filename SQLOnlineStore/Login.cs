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
    public partial class Login : Form
    {
        private SqlConnection sqlConnection = null;

        DataBase database = new DataBase();

        
        public Login()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            var loginUser = textBox_login.Text;
            var passUser = textBox_password.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            SqlCommand command = new SqlCommand($"select login_user, password_user, is_admin from Registers where login_user = N'{loginUser}' and password_user = N'{passUser}'", sqlConnection);


            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                var user = new CheckUser(table.Rows[0]["login_user"].ToString(), Convert.ToBoolean(table.Rows[0]["is_admin"]));

                MessageBox.Show("Ви успішно увішли!", "Успішно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1 frm1 = new Form1(user);
                this.Hide();
                frm1.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Такого акаунта не існує!", "Акаунта не існує!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);

            sqlConnection.Open();

            textBox_password.PasswordChar = '*';
            pictureBox3.Visible = false;
            textBox_login.MaxLength = 50;
            textBox_password.MaxLength = 50;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register frm_sing = new Register();
            frm_sing.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox_login.Text = "";
            textBox_password.Text = "";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox_password.UseSystemPasswordChar = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            textBox_password.UseSystemPasswordChar = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = false;
        }
    }
}
