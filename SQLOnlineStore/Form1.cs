using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace SQLOnlineStore
{
    public partial class Form1 : Form
    {
        private readonly CheckUser _user;

        private SqlConnection sqlConnection = null;

        DataBase database = new DataBase();

        private DataTable dataTable;

        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

        int selectedRow;
        public Form1(CheckUser user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            dataTable = new DataTable();
        }

        private void IsAdmin()
        {
            ToolStripMenuItem.Enabled = _user.IsAdmin;
            btn_new.Enabled = _user.IsAdmin;
            btn_save.Enabled = _user.IsAdmin;
            btn_modify.Enabled = _user.IsAdmin;
            btn_delete.Enabled = _user.IsAdmin;
        }


        private void ClearFields()
        {
            textBox_Id.Text = "";
            textBox_Date_of_sale.Text = "";
            textBox_Seller.Text = "";
            textBox_Buyer.Text = "";
            textBox_Product_name.Text = "";
            textBox_Unit_of_measurement.Text = "";
            textBox_Quantity.Text = "";
            textBox_Value.Text = "";
            textBox_Currency.Text = "";
            textBox_Total_amount.Text = "";
            textBox_Full_amount_in_words.Text = "";
        }


        private void RefreshDataGrid(DataGridView dgw)
        {
            dataTable.Clear(); 
            dataTable.Rows.Clear(); 

            string queryString = $"select * from Products";

            SqlCommand command = new SqlCommand(queryString, database.GetConnection());

            database.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            dataTable.Load(reader);

            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataTable = new DataTable();

            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();


            tlsUserStatus.Text = $"{_user.Login}: {_user.Status}";
            IsAdmin();
            RefreshDataGrid(dataGridView1);

            string queryString = "SELECT * FROM Products";
            SqlCommand command = new SqlCommand(queryString, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;

            dataGridView1.Columns["id"].HeaderText = "ID";
            dataGridView1.Columns["Date_of_sale"].HeaderText = "Дата продажу";
            dataGridView1.Columns["Seller"].HeaderText = "Продавець";
            dataGridView1.Columns["Buyer"].HeaderText = "Покупець";
            dataGridView1.Columns["Product_name"].HeaderText = "Назва товару";
            dataGridView1.Columns["Unit_of_measurement"].HeaderText = "Одиниця виміру";
            dataGridView1.Columns["Quantity"].HeaderText = "Кількість";
            dataGridView1.Columns["Value"].HeaderText = "Вартість";
            dataGridView1.Columns["Currency"].HeaderText = "Валюта";
            dataGridView1.Columns["Total_amount"].HeaderText = "Повна сума";
            dataGridView1.Columns["Full_amount_in_words"].HeaderText = "Сума прописом";
            dataGridView1.Columns["Seal_of_the_seller"].HeaderText = "Печатка продавця";
            dataGridView1.Columns["Stamp_of_the_buyer"].HeaderText = "Печатка покупця";
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[13].Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox_Id.Text = row.Cells[0].Value.ToString();
                textBox_Date_of_sale.Text = row.Cells[1].Value.ToString();
                textBox_Seller.Text = row.Cells[2].Value.ToString();
                textBox_Buyer.Text = row.Cells[3].Value.ToString();
                textBox_Product_name.Text = row.Cells[4].Value.ToString();
                textBox_Unit_of_measurement.Text = row.Cells[5].Value.ToString();
                textBox_Quantity.Text = row.Cells[6].Value.ToString();
                textBox_Value.Text = row.Cells[7].Value.ToString();
                textBox_Currency.Text = row.Cells[8].Value.ToString();
                textBox_Total_amount.Text = row.Cells[9].Value.ToString();
                textBox_Full_amount_in_words.Text = row.Cells[10].Value.ToString();
                bool sealOfSeller = Convert.ToBoolean(row.Cells[11].Value);
                bool stampOfBuyer = Convert.ToBoolean(row.Cells[12].Value);

                checkBox_Seal_of_the_seller.Checked = sealOfSeller;
                checkBox_Stamp_of_the_buyer.Checked = stampOfBuyer;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
            ClearFields();
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            Add_Form addfrm = new Add_Form();
            addfrm.Show();
        }

        private void Search(DataGridView dgw)
        {
            dataTable.Clear();

            string searchString = "SELECT * FROM Products WHERE id LIKE @search OR Date_of_sale LIKE @search OR Seller LIKE @search OR Buyer LIKE @search OR Product_name LIKE @search OR Unit_of_measurement LIKE @search OR Quantity LIKE @search OR Value LIKE @search OR Currency LIKE @search OR Total_amount LIKE @search OR Full_amount_in_words LIKE @search OR Seal_of_the_seller LIKE @search OR Stamp_of_the_buyer LIKE @search";

            SqlCommand com = new SqlCommand(searchString, database.GetConnection());
            com.Parameters.AddWithValue("@search", "%" + textBox_Search.Text + "%");

            database.openConnection();

            SqlDataReader read = com.ExecuteReader();

            dataTable.Load(read);

            read.Close();
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows.RemoveAt(index);
                return;
            }

            dataGridView1.Rows[index].Cells[13].Value = RowState.Deleted;
        }

        private void Updata()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                if (index >= 0 && index < dataGridView1.Rows.Count)
                {
                    if (dataGridView1.Rows[index].Cells.Count >= 14)
                    {
                        var cell = dataGridView1.Rows[index].Cells[13];
                        if (cell.Value != null)
                        {
                            var rowState = (RowState)cell.Value;

                            if (rowState == RowState.Existed)
                                continue;

                            if (rowState == RowState.Deleted)
                            {
                                var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                                var deleteQuery = $"delete from Products where id = N'{id}'";

                                var command = new SqlCommand(deleteQuery, database.GetConnection());
                                command.ExecuteNonQuery();
                            }

                            if (rowState == RowState.Modified)
                            {
                                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                                var data = dataGridView1.Rows[index].Cells[1].Value.ToString();
                                var seller = dataGridView1.Rows[index].Cells[2].Value.ToString();
                                var buyer = dataGridView1.Rows[index].Cells[3].Value.ToString();
                                var name = dataGridView1.Rows[index].Cells[4].Value.ToString();
                                var unit = dataGridView1.Rows[index].Cells[5].Value.ToString();
                                var quantity = dataGridView1.Rows[index].Cells[6].Value.ToString();
                                var value = dataGridView1.Rows[index].Cells[7].Value.ToString();
                                var currency = dataGridView1.Rows[index].Cells[8].Value.ToString();
                                var total = dataGridView1.Rows[index].Cells[9].Value.ToString();
                                var full = dataGridView1.Rows[index].Cells[10].Value.ToString();
                                var seal = dataGridView1.Rows[index].Cells[11].Value.ToString();
                                var stamp = dataGridView1.Rows[index].Cells[12].Value.ToString();

                                var changeQuery = $"update Products set Date_of_sale = N'{data}', Seller = N'{seller}', Buyer = N'{buyer}', Product_name = N'{name}', Unit_of_measurement = N'{unit}', Quantity = N'{quantity}', Value = N'{value}', Currency = N'{currency}', Total_amount = N'{total}', Full_amount_in_words = N'{full}', Seal_of_the_seller = N'{seal}', Stamp_of_the_buyer = N'{stamp}' where id = N'{id}'";

                                var command = new SqlCommand(changeQuery, database.GetConnection());
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            database.closeConnection();
        }

        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = textBox_Id.Text;
            var name = textBox_Product_name.Text;
            var data = textBox_Date_of_sale.Text;
            var seller = textBox_Seller.Text;
            var buyer = textBox_Buyer.Text;
            var unit = textBox_Unit_of_measurement.Text;

            int quantity, value, total, full;
            var currency = textBox_Currency.Text;
            var seal = checkBox_Seal_of_the_seller.Checked;
            var stamp = checkBox_Stamp_of_the_buyer.Checked;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBox_Quantity.Text, out quantity) &&
                int.TryParse(textBox_Value.Text, out value) &&
                int.TryParse(textBox_Total_amount.Text, out total) &&
                int.TryParse(textBox_Full_amount_in_words.Text, out full))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id, data, seller, buyer, name, unit, quantity, value, currency, total, full, seal, stamp);
                    dataGridView1.Rows[selectedRowIndex].Cells[13].Value = RowState.Modified;
                }
                else
                {
                    MessageBox.Show("Ціна повинна мати числовий формат!");
                }
            }
        }

        private void btn_modify_Click_1(object sender, EventArgs e)
        {
            Change();
            ClearFields();
        }

        private void btn_save_Click_1(object sender, EventArgs e)
        {
            Updata();
            ClearFields();
        }

        private void comboBox_Quantity_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Quantity.SelectedIndex)
            {
                case 0:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Quantity <=10";

                    break;
                case 1:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Quantity >=10 AND Quantity <=100";

                    break;
                case 2:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Quantity >=100 AND Quantity <=500";

                    break;
                case 3:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Quantity >=500 AND Quantity <=1000";

                    break;
                case 4:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";

                    break;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Panel_Admin panela = new Panel_Admin();
            panela.Show();
        }

        private void comboBox_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Value.SelectedIndex)
            {
                case 0:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Value <=10";

                    break;
                case 1:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Value >=10 AND Value <=100";

                    break;
                case 2:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Value >=100 AND Value <=500";

                    break;
                case 3:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Value >=500 AND Value <=1000";

                    break;
                case 4:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";

                    break;
            }
        }

        private void comboBox_Total_amount_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Total_amount.SelectedIndex)
            {
                case 0:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Total_amount <=10";

                    break;
                case 1:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Total_amount >=10 AND Total_amount <=100";

                    break;
                case 2:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Total_amount >=100 AND Total_amount <=500";

                    break;
                case 3:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Total_amount >=500 AND Total_amount <=1000";

                    break;
                case 4:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";

                    break;
            }
        }

        private void comboBox_Full_amount_in_words_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Full_amount_in_words.SelectedIndex)
            {
                case 0:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Full_amount_in_words <=10";

                    break;
                case 1:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Full_amount_in_words >=10 AND Full_amount_in_words <=100";

                    break;
                case 2:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Full_amount_in_words >=100 AND Full_amount_in_words <=500";

                    break;
                case 3:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Full_amount_in_words >=500 AND Full_amount_in_words <=1000";

                    break;
                case 4:
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";

                    break;
            }
        }

        private void comboBox_Currency_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currencyFilter = string.Empty;

            switch (comboBox_Currency.SelectedIndex)
            {
                case 0: 
                    currencyFilter = "Currency = 'USD'";
                    break;
                case 1: 
                    currencyFilter = "Currency = 'EUR'";
                    break;
                case 2: 
                    currencyFilter = "Currency = 'UAH'";
                    break;
                case 3: 
                    currencyFilter = string.Empty;
                    break;
            }

            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = currencyFilter;
        }

        private void comboBox_Unit_of_measurement_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currencyFilter = string.Empty;

            switch (comboBox_Unit_of_measurement.SelectedIndex)
            {
                case 0: 
                    currencyFilter = "Unit_of_measurement = 'Кількість'";
                    break;
                case 1: 
                    currencyFilter = "Unit_of_measurement = 'Маса'";
                    break;
                case 2:
                    currencyFilter = "Unit_of_measurement = 'Літри'";
                    break;
                case 3:
                    currencyFilter = string.Empty;
                    break;
            }

            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = currencyFilter;
        }
    }
}
