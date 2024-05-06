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

namespace SQLOnlineStore
{
    public partial class Panel_Admin : Form
    {
        private SqlConnection sqlConnection = null;

        DataBase database = new DataBase();

        public Panel_Admin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "ID");
            dataGridView1.Columns.Add("login_user", "Логін");
            dataGridView1.Columns.Add("password_user", "Пароль");
            var checkColum = new DataGridViewCheckBoxColumn();
            checkColum.HeaderText = "IsAdmin";
            dataGridView1.Columns.Add(checkColum);
        }

        private void ReadSingleRow(IDataRecord record)
        {
            dataGridView1.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetBoolean(3));
        }

        private void RefreshDataGrid()
        {
            dataGridView1.Rows.Clear();

            string queryString = $"select * from Registers";

            SqlCommand command = new SqlCommand(queryString, database.GetConnection());

            database.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(reader);
            }

            reader.Close();
        }

        private void Panel_Admin_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                var isadmin = dataGridView1.Rows[index].Cells[3].Value.ToString();

                var changeQuery = $"UPDATE Registers SET is_admin = N'{isadmin}' WHERE id = N'{id}'";

                var command = new SqlCommand(changeQuery, database.GetConnection());
                command.ExecuteNonQuery();
            }

            database.closeConnection();

            RefreshDataGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells[0].Value);
            var deleteQuery = $"DELETE FROM Registers WHERE id = N'{id}'";

            var command = new SqlCommand(deleteQuery, database.GetConnection());
            command.ExecuteNonQuery();

            database.closeConnection();

            RefreshDataGrid();
        }
    }
}
