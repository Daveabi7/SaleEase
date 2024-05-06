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


namespace SQLOnlineStore
{
    public partial class Add_Form : Form
    {
        private SqlConnection sqlConnection = null;

        DataBase database = new DataBase();
        public Add_Form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

        }

        private void Add_Form_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);

            sqlConnection.Open();
        }

        private void button_save2_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var name = textBox_Product_name2.Text;
            var dataText = textBox_Date_of_sale2.Text;
            var seller = textBox_Seller2.Text;
            var buyer = textBox_Buyer2.Text;
            var unit = textBox_Unit_of_measurement2.Text;

            int quantity, value, total, full;
            var currency = textBox_Currency2.Text;
            var seal = checkBox_Seal_of_the_seller2.Checked;
            var stamp = checkBox_Stamp_of_the_buyer2.Checked;

            if (!DateTime.TryParse(dataText, out DateTime data))
            {
                MessageBox.Show("Неправильний формат дати!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (int.TryParse(textBox_Quantity2.Text, out quantity) &&
                int.TryParse(textBox_Value2.Text, out value) &&
                int.TryParse(textBox_Total_amount2.Text, out total) &&
                int.TryParse(textBox_Full_amount_in_words2.Text, out full))
            {
                var addQuery = @"insert into Products (Date_of_sale, Seller, Buyer, Product_name, Unit_of_measurement, Quantity, Value, Currency, Total_amount, Full_amount_in_words, Seal_of_the_seller, Stamp_of_the_buyer) 
                         values (@Date_of_sale, @Seller, @Buyer, @Product_name, @Unit_of_measurement, @Quantity, @Value, @Currency, @Total_amount, @Full_amount_in_words, @Seal_of_the_seller, @Stamp_of_the_buyer)";

                var command = new SqlCommand(addQuery, database.GetConnection());
                command.Parameters.AddWithValue("@Date_of_sale", data);
                command.Parameters.AddWithValue("@Seller", seller);
                command.Parameters.AddWithValue("@Buyer", buyer);
                command.Parameters.AddWithValue("@Product_name", name);
                command.Parameters.AddWithValue("@Unit_of_measurement", unit);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Value", value);
                command.Parameters.AddWithValue("@Currency", currency);
                command.Parameters.AddWithValue("@Total_amount", total);
                command.Parameters.AddWithValue("@Full_amount_in_words", full);
                command.Parameters.AddWithValue("@Seal_of_the_seller", seal);
                command.Parameters.AddWithValue("@Stamp_of_the_buyer", stamp);

                command.ExecuteNonQuery();

                MessageBox.Show("Запис успішно створено!", "Успіх!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Ціна повинна мати числовий формат!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            database.closeConnection();
        }
    }
}
