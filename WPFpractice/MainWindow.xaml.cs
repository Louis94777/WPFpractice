using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WPFpractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           InitializeComponent();
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connString =
                    @"Server=.\SQLEXPRESS;
                     Database=HospitalDB;
                     Trusted_Connection=True;
                     TrustServerCertificate=True;";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql =
                        "SELECT * FROM Patients WHERE PatNo = @PatNo";

                    SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn);

                    da.SelectCommand.Parameters.AddWithValue(
                        "@PatNo",
                        txtPatNo.Text.Trim());

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dgPatients.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}