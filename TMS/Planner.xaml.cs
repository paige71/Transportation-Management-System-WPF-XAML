//  FILE            :   Planner.xaml.cs
//  PROJECT         :   Software Quality Term Project
//  PROGRAMMER      :   Yinruo J
//  FIRST-VERSION   :   December 6, 2021
//  DESCRIPTION     :   This contains the C# code for the Planner WPF window.

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Threading;
using System.Windows.Threading;
using System.Configuration;
using System.IO;

namespace TMS
{
    /// <summary>
    /// Interaction logic for Planner.xaml
    /// </summary>
    public partial class Planner : Window
    {
        //  NAME    :   Planner
        //  PURPOSE :   This class models the attributes and behaviours for the planner window of the TMS system. The attributes include
        //              subclasses for route and carrier that each include their individual TMS database properties. This class also
        //              includes the event handlers for receiving orders from database, updating order status, adding trip, simulating the whole 
        //              trip, updating invoice information in the database, and displaying the invoice information in the screen

        public Planner()
        {
            InitializeComponent();
            orderProcessingDate.IsTodayHighlighted = false;
        }


        //  EVENT       :   btnloaddata_Click
        //  PURPOSE     :   Load the active orders from database
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void btnloaddata_Click(object sender, RoutedEventArgs e)
        {
            string server = "localhost";
            string database = "TMS";
            string uid = "jiangyinruo";
            string password = "123456";
            string connectionString;
            connectionString = string.Format("Server={0}; Port=3306; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, password);
            string sql = "SELECT orderID,Client_Name, Job_Type,Quantity, Origin, Destination, Van_Type, order_status,create_time,carrierID FROM orders;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "LoadDataBinding");
                dataGridCustomers.DataContext = ds;
                conn.Close();
            }
        }


        //  EVENT       :   UpdateOrder_Click
        //  PURPOSE     :   Update order status in the database
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void UpdateOrder_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, string> Carrier = new Dictionary<int, string>();

            Carrier.Add(1, "Planet Express");
            Carrier.Add(2, "Schooner's");
            Carrier.Add(3, "Tillman Transport");
            Carrier.Add(4, "We Haul");

            int order_ID = Int32.Parse(orderID.Text);
            ComboBoxItem carrierItem = (ComboBoxItem)CarrierName.SelectedItem;
            string carrier = carrierItem.Content.ToString();
            var myCarrier = Carrier.FirstOrDefault(x => x.Value == carrier).Key;
            ComboBoxItem orderStatusItem = (ComboBoxItem)OrderStatusCombo.SelectedItem;
            string orderStatus = orderStatusItem.Content.ToString();
            var myDate = orderProcessingDate.SelectedDate;

            var connstr = "Server = localhost; Uid = jiangyinruo; Pwd = 123456; database = TMS";
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "UPDATE orders SET order_status=@order_status,create_time=@create_time,carrierID=@carrierID WHERE orderID = @orderID";
                comm.Parameters.AddWithValue("@order_status", orderStatus);
                comm.Parameters.AddWithValue("@create_time", myDate);
                comm.Parameters.AddWithValue("@carrierID", myCarrier);
                comm.Parameters.AddWithValue("@orderID", order_ID);
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }


        //  EVENT       :   AddTrip_Click
        //  PURPOSE     :   Add trip in the database, and simulate the whole trip
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private async void AddTrip_Click(object sender, RoutedEventArgs e)
        {
            int order_ID = Int32.Parse(orderID.Text);
            ComboBoxItem carrierItem = (ComboBoxItem)CarrierName.SelectedItem;
            string carrier = carrierItem.Content.ToString();
            int quantity = 0;
            string origin;
            string relevantCities;
            string destination;
            int distance = 0;
            double totalTime=0.0;
            double amount = 0.0;
            int surchange = 0;


            string server = "localhost";
            string database = "TMS";
            string uid = "jiangyinruo";
            string password = "123456";
            string connectionString;
            connectionString = string.Format("Server={0}; Port=3306; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, password);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT Quantity, Origin, relevant_cities, Destination FROM orders where orderID = @orderID";
                comm.Parameters.AddWithValue("@orderID", order_ID);
                using (MySqlDataReader reader = comm.ExecuteReader())
                {
                    reader.Read();
                    quantity = reader.GetInt32(0);
                    origin = reader.GetString(1);
                    relevantCities = reader.GetString(2);
                    destination = reader.GetString(3);
                }
                conn.Close();
            }
            List<string> currentRoute = new List<string>();
            currentRoute.Add(origin);
            relevantCities = relevantCities.Remove(relevantCities.Length - 1, 1);
            string[]  cities = relevantCities.Split('&');
            foreach (var city in cities)
            {
                currentRoute.Add(city);
            }
            currentRoute.Add(destination);

            orderStatusDisplay.Inlines.Add(new Run("Cargo Loading in " + origin));
            await PutTaskDelay(2000);
            totalTime += 2;
            orderStatusDisplay.Inlines.Add(new LineBreak());
            orderStatusDisplay.Inlines.Add(new Run("Cargo Loaded"));
            
            for (int i = 0; i < currentRoute.Count - 1; i++)
            {
                orderStatusDisplay.Inlines.Add(new LineBreak());
                orderStatusDisplay.Inlines.Add(new Run("Depart from " + currentRoute[i]));
                Tuple<string, string> cityTocity;
                cityTocity = new Tuple<string, string>(currentRoute[i], currentRoute[i + 1]);
                IEnumerable<route> results = route.timetable().Where(x => x.oriToDes.Item1==cityTocity.Item1 && x.oriToDes.Item2==cityTocity.Item2);
                orderStatusDisplay.Inlines.Add(new LineBreak());
                await PutTaskDelay((int)results.ToArray()[0].tripTime*1000);
                orderStatusDisplay.Inlines.Add(new Run("Arrive in " + currentRoute[i+1]));
                distance += results.ToArray()[0].distance;
                totalTime += results.ToArray()[0].tripTime;
            }
            orderStatusDisplay.Inlines.Add(new LineBreak());
            orderStatusDisplay.Inlines.Add(new Run("Cargo Unloading..."));
            await PutTaskDelay(2000);
            totalTime += 2;
            orderStatusDisplay.Inlines.Add(new LineBreak());
            orderStatusDisplay.Inlines.Add(new Run("Cargo Unloaded"));
            MessageBox.Show("Cargo Arrived and Please Confirm this order", "TMS", MessageBoxButton.OK);
            IEnumerable<carrierData> carrierResult = carrierData.CarrierTable().Where(x => x.CarrierName.Trim().Equals(carrier.Trim()));
            amount = distance * carrierResult.ToArray()[0].frate;
            if (totalTime >= 12)
            {
                surchange = 150;
            }
            else if (totalTime >= 24)
            {
                surchange = 300;
            }
            else
            {
                surchange = 0;
            }

            var connstr = "Server = localhost; Uid = jiangyinruo; Pwd = 123456; database = TMS";
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Insert INTO invoices(orderID,invoice_date,amount,distance,surcharge,trip_time) VALUES(@orderID,@invoice_date,@amount,@distance,@surcharge,@trip_time)";
                comm.Parameters.AddWithValue("@orderID", order_ID);
                comm.Parameters.AddWithValue("@invoice_date", DateTime.Today);
                comm.Parameters.AddWithValue("@amount", Math.Round(amount, 2));
                comm.Parameters.AddWithValue("@distance", distance);
                comm.Parameters.AddWithValue("@surcharge", surchange);
                comm.Parameters.AddWithValue("@trip_time", Math.Round(totalTime, 2));
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        
        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }


        //  EVENT       :   InvoiceGeneration_Click
        //  PURPOSE     :   display invoice information in the database
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void btnloadinvoice_Click(object sender, RoutedEventArgs e)
        {
            string server = "localhost";
            string database = "TMS";
            string uid = "jiangyinruo";
            string password = "123456";
            string connectionString;
            connectionString = string.Format("Server={0}; Port=3306; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, password);
            string sql = "SELECT invoiceID,orderID,invoice_date, amount,distance, surcharge, trip_time FROM invoices;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "LoadDataBinding");
                dataGridInvoices.DataContext = ds;
                conn.Close();
            }
        }

        private void Btn_LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newMainWindow = new MainWindow();
            newMainWindow.Show();
            this.Close();
            MessageBox.Show("You are logged out of the planner window.");
            WriteLog("Admin: You are logged out of the admin window.");
        }

        public static void WriteLog(string message)
        {
            string logPath = ConfigurationManager.AppSettings["logPath"];

            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now} : {message}");
            }
        }
    }
}
