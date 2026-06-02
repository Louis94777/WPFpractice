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
        //資料庫連接
        private string connString =
            @"Server=.\SQLEXPRESS;
             Database=HospitalDB;
             Trusted_Connection=True;
             TrustServerCertificate=True;";
        //自動清空輸入內容
        private void ClearInput()
        {
            txtPatNo.Text = "";
            txtName.Text = "";
        }
        //自動刷新功能
        private void LoadPatients()
        {
            using (SqlConnection conn =
                new SqlConnection(connString))
            {
                conn.Open();

                string sql = "SELECT * FROM Patients";

                SqlDataAdapter da =
                    new SqlDataAdapter(sql, conn);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dgPatients.ItemsSource =
                    dt.DefaultView;
            }
        }


        public MainWindow()
        {
           InitializeComponent();
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql;
                    SqlDataAdapter da;

                    if (!string.IsNullOrWhiteSpace(txtPatNo.Text))
                    {
                        sql = "SELECT * FROM Patients WHERE PatNo = @PatNo";

                        da = new SqlDataAdapter(sql, conn);

                        da.SelectCommand.Parameters.AddWithValue(
                            "@PatNo",
                            txtPatNo.Text.Trim());
                    }
                    else if (!string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        sql = "SELECT * FROM Patients WHERE Name LIKE @Name";

                        da = new SqlDataAdapter(sql, conn);

                        da.SelectCommand.Parameters.AddWithValue(
                            "@Name",
                            "%" + txtName.Text.Trim() + "%");
                    }
                    else
                    {
                        sql = "SELECT * FROM Patients";

                        da = new SqlDataAdapter(sql, conn);
                    }

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dgPatients.ItemsSource = dt.DefaultView;
                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show("Git Test");
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            

            using (SqlConnection conn =
                   new SqlConnection(connString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Patients
                    ( PatNo, Name )
                    VALUES  
                    ( @PatNo, @Name )";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue(
                    "@PatNo",
                    txtPatNo.Text.Trim());

                cmd.Parameters.AddWithValue(
                    "@Name",
                    txtName.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("新增成功");
                LoadPatients();
                ClearInput();
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn =
                   new SqlConnection(connString))
            {
                conn.Open();

                string sql =
                    @"UPDATE Patients
                    SET Name=@Name
                    WHERE PatNo=@PatNo";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@PatNo",
                    txtPatNo.Text.Trim());

                cmd.Parameters.AddWithValue("@Name",
                    txtName.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("修改成功");
                LoadPatients();
                ClearInput();
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "確定刪除？",
                "確認",
                MessageBoxButton.YesNo)
                != MessageBoxResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(connString))
            {
                conn.Open();

                string sql =
                    "DELETE FROM Patients WHERE PatNo=@PatNo";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@PatNo",
                    txtPatNo.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("刪除成功");
                LoadPatients();
                ClearInput();
            }
        }
        private void dgPatients_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            if (dgPatients.SelectedItem is DataRowView row)
            {
                txtPatNo.Text =
                    row["PatNo"].ToString();

                txtName.Text =
                    row["Name"].ToString();

                LoadVisits(
                    row["PatNo"].ToString());
            }
        }
        private void LoadVisits(string patNo)
        {
            MessageBox.Show("PatNo=" + patNo);

            using (SqlConnection conn =
                new SqlConnection(connString))
            {
                conn.Open();

                string sql =
                    @"SELECT
                VisitID,
                VisitDate,
                Doctor
              FROM Visits
              WHERE PatNo = @PatNo";

                SqlDataAdapter da =
                    new SqlDataAdapter(sql, conn);

                da.SelectCommand.Parameters.AddWithValue(
                    "@PatNo",
                    patNo);

                DataTable dt = new DataTable();

                da.Fill(dt);

                MessageBox.Show("筆數=" + dt.Rows.Count);

                dgVisits.ItemsSource = dt.DefaultView;
            }
        }
        //新增看診記錄
        private void BtnAddVisit_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPatNo.Text))
            {
                MessageBox.Show("請先選擇病患");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDoctor.Text))
            {
                MessageBox.Show("請輸入醫師姓名");
                return;
            }

            if (dpVisitDate.SelectedDate == null)
            {
                MessageBox.Show("請選擇看診日期");
                return;
            }

            using (SqlConnection conn =
                new SqlConnection(connString))
            {
                conn.Open();

                string sql =
                    @"INSERT INTO Visits
              (
                  PatNo,
                  VisitDate,
                  Doctor
              )
              VALUES
              (
                  @PatNo,
                  @VisitDate,
                  @Doctor
              )";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue(
                    "@PatNo",
                    txtPatNo.Text.Trim());

                cmd.Parameters.AddWithValue(
                    "@VisitDate",
                    dpVisitDate.SelectedDate.Value);

                cmd.Parameters.AddWithValue(
                    "@Doctor",
                    txtDoctor.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("新增看診成功");
                txtDoctor.Text = "";
                dpVisitDate.SelectedDate = DateTime.Today;

                LoadVisits(txtPatNo.Text.Trim());
            }
        }
    }
    }


