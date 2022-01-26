//  FILE            :   Admin.xaml.cs
//  PROJECT         :   Software Quality Term Project
//  PROGRAMMER      :   Paige Lam
//  FIRST-VERSION   :   November 30, 2021
//  DESCRIPTION     :   This contains the C# code for the WPF admin window.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace TMS
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            tbIpAddress.Text = ConfigurationManager.AppSettings["ipAddress"];
            tbPort.Text = ConfigurationManager.AppSettings["port"];
            tbLogFilePath.Text = ConfigurationManager.AppSettings["logPath"];
            fill_combo();
        }

        /// <summary>
        /// This method loads carriers and carriertodeport table in a join datagrid view when window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //set connection
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();

                    //select tables
                    string query = "select * from carriers inner join carriertodeport on carriers.carrierID=carriertodeport.carrierID; ";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.ExecuteNonQuery();

                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable("carriers");

                    //display in datagrid view
                    dataAdapter.Fill(dt);
                    DataGrid.ItemsSource = dt.DefaultView;
                    dataAdapter.Update(dt);

                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString() + "Load error.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "Failed to load carrier table from database.");
                WriteLog("Failed to load carrier table from database.");
            }
        }

        /// <summary>
        /// This method sets up the format and path for the service log file
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            string logPath = ConfigurationManager.AppSettings["logPath"];

            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now} : {message}");
            }
        }

        /// <summary>
        /// This method updates the carrier table in the database if change was made by admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Carrier_Update_Cick(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    string query = "update carriers set carriers.carrier_name='" + tbCarrierName.Text +
                                                        "', carriers.carrier_FTL_rate='" + tbFTLRate.Text +
                                                        "', carriers.carrier_LTL_rate='" + tbLTLRate.Text +
                                                        "', carriers.carrier_reef_charge='" + tbrfeeCharge.Text +
                                                        "'  where carriers.carrierID='" + tbCarrierID.Text + "' ";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("carriers table is updated!");
                    WriteLog("Admin: carriers table is updated!");
                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// This method add a new record to the carrier table if insertion was made by admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Carrier_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //set connection
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();

                    //if textboxes are null or empty provide feedback to user
                    if ((string.IsNullOrEmpty(tbCarrierName.Text)) ||
                        (string.IsNullOrEmpty(tbFTLRate.Text)) ||
                        (string.IsNullOrEmpty(tbLTLRate.Text)) ||
                        (string.IsNullOrEmpty(tbrfeeCharge.Text)))
                    {
                        MessageBox.Show("All fields are mandatory!");
                    }
                    else
                    {
                        string query = "insert into carriers(carrier_name, carrier_FTL_rate, carrier_LTL_rate, carrier_reef_charge) values ('" + tbCarrierName.Text + "','" + tbFTLRate.Text + "','" + tbLTLRate.Text + "','" + tbrfeeCharge.Text + "')";

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("carriers table is updated!");
                        WriteLog("Admin: carriers table is updated!");
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// This method allow admin to back up current tms database into a destinated folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Backup_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
            string file = "C:/temp/backup.sql";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup backup = new MySqlBackup(cmd))
                    {
                        cmd.Connection = connection;
                        connection.Open();
                        backup.ExportToFile(file);
                        MessageBox.Show("Backup of tms database is finished.\nFile path C:/temp/backup.sql");
                        WriteLog("Admin: Backup of tms database is finished.");
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Log out the current screen and go back to log in scrren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newMainWindow = new MainWindow();
            newMainWindow.Show();
            this.Close();
            MessageBox.Show("You are logged out of the admin window.");
            WriteLog("Admin: You are logged out of the admin window.");

        }

        /// <summary>
        /// Delete selected record from the carrier table in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Carrier_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    string query = "delete from carriers where carriers.carrierID='" + tbCarrierID.Text + "' ";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("selected carriers record is deleted from tms database!");
                    WriteLog("Admin: selected carriers record is deleted from tms database!");
                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// This method add a new record to the carriertodeport table if insertion was made by admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Route_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();

                    //check if textboxes are null or empty provide feedback to user
                    if ((string.IsNullOrEmpty(tbCarrierDeportCity.Text)) ||
                        (string.IsNullOrEmpty(tbFTLAva.Text)) ||
                        (string.IsNullOrEmpty(tbLTLAva.Text)) ||
                        (string.IsNullOrEmpty(tbRouteCarrierID.Text)))
                    {
                        MessageBox.Show("All fields are mandatory!");
                    }
                    else
                    {
                        string query = "insert into carriertodeport (deport_city, carrier_FTL_availability, carrier_LTL_availability, carrierID) values ('" + tbCarrierDeportCity.Text + "','" + tbFTLAva.Text + "','" + tbLTLAva.Text + "','" + tbRouteCarrierID.Text + "')";

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("carriertodeport table is updated!");
                        WriteLog("Admin: carriertodeport table is updated!");
                        connection.Close();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// This method updates the carriertodeport table in the database if change was made by admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Route_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    string query = "update carriertodeport set carriertodeport.deport_city='" + tbCarrierDeportCity.Text +
                                                               "', carriertodeport.carrier_FTL_availability='" + tbFTLAva.Text +
                                                               "', carriertodeport.carrier_LTL_availability='" + tbLTLAva.Text +
                                                               "'  where carriertodeport.carrierID='" + tbCarrierID.Text +
                                                               "'  and carriertodeport.carrierToDeportID='" + tbCarrierDeportID.Text + "' ";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("carriertodeport table is updated!");
                    WriteLog("Admin: carriertodeport table is updated!");
                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        ///  Delete selected record from the carriertodeport table in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Route_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    string query = "delete from carriertodeport where carriertodeport.carrierToDeportID='" + tbCarrierDeportID.Text + "' ";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("selected carriertodeport record is deleted from tms database!");
                    WriteLog("Admin: selected carriertodeport record is deleted from tms database!");
                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_NewLogFilePath_Click(object sender, RoutedEventArgs e) //WIP
        {
            string newPath = tbLogFilePath.Text;

            //edit existing key's value
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["logPath"].Value = newPath;

            //save changes in app.config file 
            config.Save(ConfigurationSaveMode.Modified);
            MessageBox.Show("Log file path is updated!");
            WriteLog("Admin: Log file path is updated!");

            //reload of a changed section 
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Loads the log file content and display in the textbox area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadLogFileContent_Click(object sender, RoutedEventArgs e)
        {
            tbLoadLogFile.Text = File.ReadAllText(ConfigurationManager.AppSettings["logPath"]);
            MessageBox.Show("Log file content is successfully loaded.");
            WriteLog("Admin: Log file content is successfully loaded.");
        }

        private void DataGridCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            DataRowView rowSelected = dg.SelectedItem as DataRowView;

            if (rowSelected != null)
            {
                //carrier fee/rate
                tbCarrierID.Text = rowSelected["carrierID"].ToString();
                tbCarrierDeportID.Text = rowSelected["carrierToDeportID"].ToString();
                tbCarrierName.Text = rowSelected["carrier_name"].ToString();
                tbCarrierDeportCity.Text = rowSelected["deport_city"].ToString();
                tbFTLAva.Text = rowSelected["carrier_FTL_availability"].ToString();
                tbFTLRate.Text = rowSelected["carrier_FTL_rate"].ToString();
                tbLTLAva.Text = rowSelected["carrier_LTL_availability"].ToString();
                tbLTLRate.Text = rowSelected["carrier_LTL_rate"].ToString();
                tbrfeeCharge.Text = rowSelected["carrier_reef_charge"].ToString();
                tbRouteCarrierID.Text = rowSelected["carrierID"].ToString();
            }

            WriteLog("Admin: datagrid content is loaded into carrier rate/fee section.");
        }

        /// <summary>
        /// This method populates the combo box drop down list by routeID from Route table
        /// </summary>
        void fill_combo()
        {
            //set connection
            string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";

            //select tables
            string query = "select * from route";

            MySqlConnection connection = new MySqlConnection(connectionString);

            MySqlCommand cmd = new MySqlCommand(query, connection);

            try
            {
                cbRoute.Items.Clear();

                connection.Open();


                cmd.ExecuteNonQuery();
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    string routeID = dataReader.GetInt32(0).ToString();
                    string city1 = dataReader.GetString(2);
                    string city2 = dataReader.GetString(3);
                    cbRoute.Items.Add(routeID + " " + city1 + " : " + city2);
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "Load error.");
            }
        }

        /// <summary>
        /// This method populates the route table values into the assigned textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRoute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //set connection
            string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from route where routeID = '" + cbRoute.SelectedItem.ToString() + "'";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    tbRouteID.Text = dr["routeID"].ToString();
                    tbTruckLoadType.Text = dr["TruckLoadType"].ToString();
                    tbCity1.Text = dr["deport_city"].ToString();
                    tbCity2.Text = dr["end_city"].ToString();
                    tbTotalDistance.Text = dr["totalDistance"].ToString();
                    tbTotalTime.Text = dr["totalTime"].ToString();
                    tbRelevantCity1.Text = dr["relevantCity1"].ToString();
                    tbRelevantCity2.Text = dr["relevantCity2"].ToString();
                    tbSurcharge.Text = dr["surcharge"].ToString();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "Load error.");
            }
        }
    }
}
