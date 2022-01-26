//  FILE            :   Buyer.xaml.cs
//  PROJECT         :   Software Quality Term Project
//  PROGRAMMER      :   Erica Luksts
//  FIRST-VERSION   :   November 30, 2021
//  DESCRIPTION     :   This contains the C# code for the Buyer WPF window.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Reflection;

namespace TMS
{


    //  NAME    :   Buyer
    //  PURPOSE :   This class models the attributes and behaviours for the buyer window of the TMS system. The attributes include
    //              subclasses for contract, invoice, and order that each include their individual TMS database properties. The 
    //              attributes are also included for name, password, and userID to save to login purposes. This class also
    //              includes the event handlers for creating new orders, updating client databases, and generating invoices. To track
    //              some of the items locally within the work session, list data structures have been used to populate
    //              invioce, contract, order, and client information in a local storage space.

    public partial class Buyer : Window
    {

        public string name; // buyer username for login
        public string password; // password for login
        public int userID; // userID number for login



        //  NAME    :   contract
        //  PURPOSE :   This class models the attributes for the contract from the Contract Marketplace. There are properties
        //              included for client_name, job_type (FTL is 0, LTL is 1), quantity, origin, destination, van_type (0 for dry van & 1 for reefer).            
        public class contract : IEquatable<contract>
        {

            public string Client_Name { get; set; }
            public int Job_Type { get; set; }
            public int Quantity { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public int Van_Type { get; set; }

            //  METHOD      :   Equals
            //  PURPOSE     :   Used for comparison of items from the contractList to any new contracts being added to the list.
            //  PARAMETERS  :   contract other - contract instance to compare
            //  RETURNS     :   bool - true if equals and false if not
            public bool Equals(contract other)
            {
                if (this.Client_Name == other.Client_Name && this.Job_Type == other.Job_Type
                    && this.Quantity == other.Quantity && this.Origin == other.Origin &&
                    this.Destination == other.Destination && this.Van_Type == other.Van_Type)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        //  NAME    :   client
        //  PURPOSE :   This class models the attributes for each client item in the TMS database.  There are properties
        //              included for the clientID, clientName, and clientStatus to update the TMS database or local client list.
        public class client
        {
            public int clientID { get; set; }
            public string clientName { get; set; }

            public string clientStatus { get; set; }

        }


        //  NAME    :   order
        //  PURPOSE :   This class models the attributes for each order item in the TMS database.  There are properties
        //              included for orderID, client_name, job_type, quantity, origin, relevant cities, destination,
        //              van type, order status, create time, and carrierID. 
        public class order : IEquatable<order>
        {
            public int orderID { get; set; }
            public string Client_Name { get; set; }
            public int Job_Type { get; set; }
            public int Quantity { get; set; }
            public string Origin { get; set; }

            public string relevant_cities { get; set; } // to attach additional cities for LTL orders
            public string Destination { get; set; }
            public int Van_Type { get; set; }

            public string order_status { get; set; } // set to 'new' for initial order, set to 'done' for completed order

            public string create_time { get; set; }

            public int carrierID { get; set; }

            //  METHOD      :   Equals
            //  PURPOSE     :   Used for comparison of items from the orderList to any new orders
            //  PARAMETERS  :   contract other - order instance to compare
            //  RETURNS     :   bool - true if equals and false if not
            public bool Equals(order other)
            {
                if (this.orderID == other.orderID && this.Client_Name == other.Client_Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //  NAME    :   invoice
        //  PURPOSE :   This class models the attributes for each invoice in the TMS database. There are properties included for
        //              the invoiceID, orderID, invoice_date, amount, distance, surcharge, and trip_time.
        public class invoice
        {
            public int invoiceID { get; set; }

            public int orderID { get; set; }

            public string invoiceDate { get; set; }

            public double invoiceAmount { get; set; }

            public int distance { get; set; }

            public int surcharge { get; set; }

            public double tripTime { get; set; }

        }



        // tracking of invoice list locally
        List<invoice> invoiceList = new List<invoice>();

        // used for new client list table, can be accepted or rejected
        ObservableCollection<client> newClientList = new ObservableCollection<client>();
        ObservableCollection<client> clientList = new ObservableCollection<client>();

        // used for contract table, populates from Contract Marketplace
        ObservableCollection<contract> contractList = new ObservableCollection<contract>();
        ObservableCollection<order> completedOrderList = new ObservableCollection<order>();

        // list of orders from TMS
        List<order> orderList = new List<order>();


        //  METHOD      :   Buyer - constructor
        //  PURPOSE     :   Creates a new buyer window instance. Gets the userID for the session.
        //  PARAMETERS  :   string name - buyer userName
        //                  string password - buyer password
        //  RETURNS     :   Buyer window instance
        public Buyer(string name, string password)
        {
            InitializeComponent();
            this.name = name;
            this.password = password;


            // GET userID for the buyer
            // connect to TMS database
            string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
            string database = "tms";
            string uid = "jiangyinruo";
            string pwd = "123456";
            string port = ConfigurationManager.AppSettings["port"].ToString();
            string connectionString;
            connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, pwd);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                //check user data to find a match, checks for both matching username and password in same line
                string sqlCommand = "select * from tms.users where userName = '" + name + "' and userPassword = '" + password + "'";
                MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);

                //check if user data exists
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {


                    while (reader.Read())
                    {
                        // get userID
                        if (reader.HasRows)
                        {
                            this.userID = Int32.Parse(reader[0].ToString());
                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString() + "\nUnable to validate userID.\nContact database admin for support.", "Database Connection Error");

                }

                finally
                {
                    reader.Close();
                    connection.Close();
                }


            }
        }


        //  EVENT       :   Window_Initialized
        //  PURPOSE     :   Creates a new buyer window instance. Copies any TMS table information for invoices, orders, and clients into local lists
        //                  for the datagrid displays. Copies Contract Marketplace information for contracts and updates contract list. Sets name and password for session.
        //  PARAMETERS  :   object sender
        //                  EventArgs e
        //  RETURNS     :   NONE
        private void Window_Initialized(object sender, EventArgs e)
        {


            // CLIENTS UPDATE
            // update the client window data grid if there are clients in TMS
            string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
            string database = "tms";
            string uid = "jiangyinruo";
            string pwd = "123456";
            string port = ConfigurationManager.AppSettings["port"].ToString();
            string connectionString;
            connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, pwd);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // display only clients that are old
                string sqlString = "SELECT * FROM clients WHERE client_status!=\'new\';";
                MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {

                    // read each line from database
                    while (reader.Read())
                    {

                        client newClient = new client();
                        newClient.clientID = Int32.Parse(reader[0].ToString());
                        newClient.clientName = reader[1].ToString();
                        newClient.clientStatus = reader[2].ToString();


                        this.clientList.Add(newClient);

                    }

                    ClientDataGrid.ItemsSource = this.clientList;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString() + "\nUnable to update current client list.\nContact database admin for support.", "Database Connection Error");

                }

                finally
                {
                    reader.Close();
                    connection.Close();
                }


                //--------------------------------------------
                // ORDERS
                // Get any orders from tables within TMS


                connection.Open();
                string sqlOrderCmd = "SELECT * FROM orders;";
                MySqlCommand cmdOrders = new MySqlCommand(sqlOrderCmd, connection);


                MySqlDataReader orderReader = cmdOrders.ExecuteReader();
                try
                {

                    // read each order from tms.orders
                    while (orderReader.Read())
                    {

                        order newOrder = new order();
                        // include each field
                        newOrder.orderID = Int32.Parse(orderReader[0].ToString());
                        newOrder.Client_Name = orderReader[1].ToString();
                        newOrder.Job_Type = Int32.Parse(orderReader[2].ToString());
                        newOrder.Quantity = Int32.Parse(orderReader[3].ToString());
                        newOrder.Origin = orderReader[4].ToString();
                        newOrder.relevant_cities = orderReader[5].ToString();
                        newOrder.Destination = orderReader[6].ToString();
                        newOrder.Van_Type = Int32.Parse(orderReader[7].ToString());
                        newOrder.order_status = orderReader[8].ToString();
                        newOrder.create_time = orderReader[9].ToString();
                        newOrder.carrierID = Int32.Parse(orderReader[10].ToString());

                        // add to the main list
                        this.orderList.Add(newOrder);


                        // if the order status is done, add it to the completed orderList table
                        if (newOrder.order_status == "done")
                        {
                            this.completedOrderList.Add(newOrder);
                        }

                    }

                    // set data source for the data grid for completed orders
                    OrderDataGrid.ItemsSource = this.completedOrderList;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString() + "\nUnable to update order list.\nContact database admin for support.", "Database Connection Error");

                }
                finally
                {
                    orderReader.Close();

                    connection.Close();
                }


                //--------------------------------------------
                // INVOICES
                // get all invoices from the invoices table within TMS
                connection.Open();
                string sqlInvoicesCmd = "SELECT * FROM invoices;";
                MySqlCommand cmdInvoices = new MySqlCommand(sqlInvoicesCmd, connection);

                MySqlDataReader invoiceReader = cmdInvoices.ExecuteReader();
                try
                {

                    // read each invoice from the invoices table
                    while (invoiceReader.Read())
                    {

                        invoice newInvoice = new invoice();

                        // include each field
                        newInvoice.invoiceID = Int32.Parse(invoiceReader[0].ToString());
                        newInvoice.orderID = Int32.Parse(invoiceReader[1].ToString());
                        newInvoice.invoiceDate = invoiceReader[2].ToString();
                        newInvoice.invoiceAmount = double.Parse(invoiceReader[3].ToString());
                        newInvoice.distance = Int32.Parse(invoiceReader[4].ToString());
                        newInvoice.surcharge = Int32.Parse(invoiceReader[5].ToString());
                        newInvoice.tripTime = double.Parse(invoiceReader[6].ToString());


                        // add to the list
                        this.invoiceList.Add(newInvoice);

                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString() + "\nUnable to update order list.\nContact database admin for support.", "Database Connection Error");

                }
                finally
                {
                    invoiceReader.Close();
                    connection.Close();
                }

                // done window initializing
            }
        }





        //  EVENT       :   RequestNewContracts_Click
        //  PURPOSE     :   Updates the contract data grid with contracts from the Marketplace. 
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void RequestNewContracts_Click(object sender, RoutedEventArgs e)
        {

            // connect to contract marketplace
            string server = "159.89.117.198";
            string database = "cmp";
            string uid = "DevOSHT";
            string password = "Snodgr4ss!";
            string connectionString;
            connectionString = string.Format("Server={0}; Port=3306; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, password);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();


                // select all contracts
                string sqlContractCmd = "SELECT * FROM Contract;";
                MySqlCommand cmd = new MySqlCommand(sqlContractCmd, connection);




                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {

                    // read each contract
                    while (reader.Read())
                    {

                        contract newContract = new contract();

                        newContract.Client_Name = reader[0].ToString();
                        newContract.Job_Type = Int32.Parse(reader[1].ToString());
                        newContract.Quantity = Int32.Parse(reader[2].ToString());
                        newContract.Origin = reader[3].ToString();
                        newContract.Destination = reader[4].ToString();
                        newContract.Van_Type = Int32.Parse(reader[5].ToString());

                        // add to the list


                        if (!contractList.Contains(newContract))
                        {
                            this.contractList.Add(newContract);
                        }


                    }

                    // set data source for the contractlist
                    ContractDataGrid.ItemsSource = this.contractList;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString() + "\nUnable to update order list.\nContact database admin for support.", "Database Connection Error");

                }

                finally
                {
                    reader.Close();

                    connection.Close();
                }





            }
        }


        //  EVENT       :   InvoiceGeneration_Click
        //  PURPOSE     :   Looks up the selected completed order from the order data grid and finds its invoice in the database. Then, it
        //                  generates the invoice text file.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void InvoiceGeneration_Click(object sender, RoutedEventArgs e)
        {


            // no completed orders in the list
            if (orderList.Count == 0)
            {
                MessageBox.Show("Error: No completed orders available to make invoice.\n", "Invoice Error");
            }
            // no completed order selected
            else if (OrderDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Error: Select a completed order before clicking Generate Invoice", "Error");
            }
            // completed order selected
            // create an invoice item to add to the database
            else
            {

                // cast the selected data grid item into an order item
                order completedOrder = (order)OrderDataGrid.SelectedItem;

                // identify what orderID is to be found from the invoice list
                int orderIDToFind = completedOrder.orderID;

                // get the invoice from the invoice list
                string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
                string database = "tms";
                string uid = "jiangyinruo";
                string pwd = "123456";
                string port = ConfigurationManager.AppSettings["port"].ToString();
                string connectionString;
                connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, pwd);
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // find the orderID that matches in the invoice database
                    string sqlString = "SELECT * from invoices WHERE invoices.orderID=" + orderIDToFind.ToString() + ";";


                    MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                    try
                    {

                        invoice newInvoice = new invoice();
                        MySqlDataReader invoiceReader = cmd.ExecuteReader();


                        while (invoiceReader.Read())
                        {
                            if (invoiceReader.HasRows)
                            {
                                newInvoice.invoiceID = Int32.Parse(invoiceReader[0].ToString());
                                newInvoice.orderID = orderIDToFind;
                                newInvoice.invoiceDate = invoiceReader[2].ToString();
                                newInvoice.invoiceAmount = double.Parse(invoiceReader[3].ToString());
                                newInvoice.distance = Int32.Parse(invoiceReader[4].ToString());
                                newInvoice.surcharge = Int32.Parse(invoiceReader[5].ToString());
                                newInvoice.tripTime = double.Parse(invoiceReader[6].ToString());
                            }
                        }


                        invoiceReader.Close();
                        // found invoice in database
                        // now generate the text file

                        string invoiceFolder = "invoices";
                        string path = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
                        path = path.Remove(path.LastIndexOf(@"\")) + @"\" + invoiceFolder;

                        string invoiceName = "Invoice_Ref" + newInvoice.invoiceID.ToString();


                        string invoicePath = path + @"\" + invoiceName + ".txt";

                        // create invoice directory to save invoices to
                        try
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                        }
                        catch (Exception pathError)
                        {
                            MessageBox.Show("Error: " + pathError.ToString() + "\nCannot create Invoice folder.", "Invoice Directory Error");
                        }


                        // create the invoice text file
                        try
                        {
                            // check if it exists
                            // if it does, warn the user they may be overwriting existing data
                            if (File.Exists(invoicePath))
                            {
                                MessageBoxResult result = MessageBox.Show("Warning: This will overwrite an existing invoice.\n Do you want to continue?",
                                    "Existing Invoice Warning", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    string invoiceContent = "Invoice Number: " + newInvoice.invoiceID.ToString() + "\n";
                                    invoiceContent += "Issue Date: " + newInvoice.invoiceDate + "\n";
                                    invoiceContent += "Order ID: " + newInvoice.orderID.ToString() + "\n";
                                    invoiceContent += "Distance: " + newInvoice.distance.ToString() + " km\n";
                                    invoiceContent += "Trip Time: " + newInvoice.tripTime.ToString() + "\n";
                                    invoiceContent += "Surcharge: $ " + newInvoice.surcharge.ToString() + "\n";
                                    invoiceContent += "Total Due: $ " + newInvoice.invoiceAmount.ToString() + "\n";

                                    // add any business rules here
                                    invoiceContent += "\nInvoices are due within 30 days of issue date" + "\n";

                                    File.WriteAllText(invoicePath, invoiceContent);

                                    MessageBox.Show("Invoice ID: " + newInvoice.invoiceID.ToString() + " ready to send to client.", "Invoice Generated");
                                }

                            }
                            else
                            {
                                string invoiceContent = "Invoice Number: " + newInvoice.invoiceID.ToString() + "\n";
                                invoiceContent += "Issue Date: " + newInvoice.invoiceDate + "\n";
                                invoiceContent += "Order ID: " + newInvoice.orderID.ToString() + "\n";
                                invoiceContent += "Distance: " + newInvoice.distance.ToString() + " km\n";
                                invoiceContent += "Trip Time: " + newInvoice.tripTime.ToString() + "\n";
                                invoiceContent += "Surcharge: $ " + newInvoice.surcharge.ToString() + "\n";
                                invoiceContent += "Total Due: $ " + newInvoice.invoiceAmount.ToString() + "\n";


                                // add any business rules here
                                invoiceContent += "\nInvoices are due within 30 days of issue date" + "\n";


                                File.WriteAllText(invoicePath, invoiceContent);

                                MessageBox.Show("Invoice ID: " + newInvoice.invoiceID.ToString() + "ready to send to client.", "Invoice Generated");
                            }
                        }
                        catch (Exception fileError)
                        {
                            MessageBox.Show("File Error: " + fileError.ToString() + "\nUnable to create invoice text file.", "Invoice Error");
                        }

                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Error: Not able to find invoice in database.\n" + er.ToString() + "\nVerify internet connection for SQL database.", "Selection Error");
                    }
                    finally
                    {

                        connection.Close();
                    }



                }



            }




        }







        //  EVENT       :   Logout_Click
        //  PURPOSE     :   Logs user out of buyer window, closes the window and reopens the main page.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You have successfully logged out.");


            // show main login screen again
            MainWindow login = new MainWindow();
            login.Show();

            this.Close();
        }




        //  EVENT       :   CreateNewOrder_Click
        //  PURPOSE     :   Creates a new order entry into the database. First, gets any relevant cities that have been selected. Inserts order
        //                  into the TMS sql database. Selects orders from the tms database to see if there are any duplicates. Adds order into the local list
        //                  and confirms after order has been inserted if user wants to add the client to the database.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void CreateNewOrder_Click(object sender, RoutedEventArgs e)
        {

            // ensure order has cities attached
            //string relevantCities = "";

            //if (this.WindsorCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Windsor&";
            //}
            //if (this.HamiltonCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Hamilton&";
            //}
            //if (this.LondonCheckBox.IsChecked == true)
            //{
            //    relevantCities += "London&";
            //}
            //if (this.TorontoCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Toronto&";
            //}
            //if (this.OshawaCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Oshawa&";
            //}
            //if (this.BellevilleCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Belleville&";
            //}
            //if (this.KingstonCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Kingston&";
            //}
            //if (this.OttawaCheckBox.IsChecked == true)
            //{
            //    relevantCities += "Ottawa&";
            //}


            // no completed orders in the list
            if (contractList.Count == 0)
            {
                MessageBox.Show("Error: No contracts available to make order.\n" +
                    "Select Request New Contracts to check Contract Marketplace.", "Error");
            }
            // no completed order selected
            else if (ContractDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Error: Select a contract before creating an order.", "Error");
            }
            else
            {

                // get contract data
                contract selectedContract = (contract)ContractDataGrid.SelectedItem;

                // create an order instance that will be added to TMS for the selected contract
                order newOrder = new order();
                newOrder.Client_Name = selectedContract.Client_Name;
                newOrder.Job_Type = selectedContract.Job_Type;
                newOrder.Quantity = selectedContract.Quantity;
                newOrder.Origin = selectedContract.Origin;
                newOrder.relevant_cities = relevantCities;
                newOrder.Destination = selectedContract.Destination;
                newOrder.Van_Type = selectedContract.Van_Type;
                newOrder.order_status = "";
                newOrder.create_time = DateTime.Now.ToString("yyyy-MM-dd");
                newOrder.carrierID = 0;


                // insert the new order into the orders table in SQL
                string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
                string database = "tms";
                string uid = "jiangyinruo";
                string password = "123456";
                string port = ConfigurationManager.AppSettings["port"].ToString();
                string connectionString;
                connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, password);
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {

                    connection.Open();


                    // add into the orders table in TMS, will auto generate orderID
                    string orderInsert = "SET FOREIGN_KEY_CHECKS=0;INSERT into orders (client_name, job_type, quantity, origin, relevant_cities" +
                        ", destination, van_type, order_status, create_time, carrierID) VALUES ('";
                    orderInsert += newOrder.Client_Name + "', ";
                    orderInsert += newOrder.Job_Type.ToString() + ", ";
                    orderInsert += newOrder.Quantity.ToString() + ", '";
                    orderInsert += newOrder.Origin + "', '";
                    orderInsert += newOrder.relevant_cities + "', '";
                    orderInsert += newOrder.Destination + "', ";
                    orderInsert += newOrder.Van_Type.ToString() + ", '";
                    orderInsert += "new" + "', '";
                    orderInsert += newOrder.create_time + "', ";
                    orderInsert += newOrder.carrierID.ToString() + ");";


                    MySqlCommand cmd = new MySqlCommand(orderInsert, connection);
                    try
                    {
                        // inserts the order into the orders table in TMS
                        cmd.ExecuteNonQuery();

                        connection.Close();

                        connection.Open();

                        // get the orderID generated from the contract inserted
                        string readOrder = "SELECT orderID from orders WHERE ";
                        readOrder += "client_name='" + newOrder.Client_Name + "' AND ";
                        readOrder += "Job_Type=" + newOrder.Job_Type.ToString() + " AND ";
                        readOrder += "Quantity=" + newOrder.Quantity.ToString() + " AND ";
                        readOrder += "Origin='" + newOrder.Origin + "' AND ";
                        readOrder += "relevant_cities='" + newOrder.relevant_cities + "' AND ";
                        readOrder += "Destination='" + newOrder.Destination + "' AND ";
                        readOrder += "Van_Type=" + newOrder.Van_Type + " AND ";
                        readOrder += "order_status='new' AND ";
                        readOrder += "create_time='" + newOrder.create_time + "' AND ";
                        readOrder += "carrierID=" + newOrder.carrierID.ToString() + ";";

                        // read the generated order item from TMS
                        MySqlCommand readOrderCmd = new MySqlCommand(readOrder, connection);

                        try
                        {
                            MySqlDataReader dataReader = readOrderCmd.ExecuteReader();

                            int count = 0;
                            while (dataReader.Read())
                            {
                                // gets back the order ID
                                if (dataReader.HasRows)
                                {
                                    count++;

                                }

                            }

                            // order already exists in database
                            // delete order from database and inform customer
                            if (count != 1)
                            {
                                dataReader.Close();
                                connection.Close();

                                MessageBox.Show("Duplicate order entry made. Select a different contract to create order.", "Duplicate Warning");

                                // get the orderID generated from the contract inserted
                                string deleteOrder = "DELETE from orders WHERE orderID=" + newOrder.orderID.ToString() + ";";

                                connection.Open();

                                MySqlCommand cmdDelete = new MySqlCommand(deleteOrder, connection);
                                try
                                {
                                    cmdDelete.ExecuteNonQuery();

                                }
                                catch (Exception eA)
                                {
                                    MessageBox.Show("Error: " + eA.ToString() + "\nUnable to delete order from database. Contact database admin.", "Delete Error");
                                }
                                finally
                                {
                                    connection.Close();
                                }


                            }
                            // new order for the table
                            else
                            {
                                newOrder.orderID = Int32.Parse(dataReader[0].ToString());
                                dataReader.Close();
                                connection.Close();

                                // add order to orderList
                                orderList.Add(newOrder);

                                MessageBoxResult result = MessageBox.Show("Order successfully entered into database. " +
                                        "Do you want to enter client into database?", "Success", MessageBoxButton.YesNo);

                                // attempt to add client also to client database
                                if (result == MessageBoxResult.Yes)
                                {


                                    // add client to new client list
                                    // verify if client is already in the client data base
                                    string client_nameToFind = newOrder.Client_Name;

                                    connection.Open();

                                    // search for matching clientID from the clientName
                                    string searchQuery = "SELECT clientID from clients WHERE client_name='" + client_nameToFind + "';";
                                    string clientIDStr = "newClient";
                                    MySqlCommand cmdSearch = new MySqlCommand(searchQuery, connection);
                                    MySqlDataReader reader = cmdSearch.ExecuteReader();
                                    try
                                    {

                                        while (reader.Read())
                                        {

                                            // client already in database, get its client ID
                                            if (reader.HasRows)
                                            {
                                                clientIDStr = reader[0].ToString();
                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error:" + ex.ToString() + "\nUnable to search for client in database.", "Database Connection Error");

                                    }

                                    finally
                                    {
                                        reader.Close();
                                        connection.Close();
                                    }


                                    // client does not exist in database, add client to the database
                                    // get the clientID from the database
                                    // add the client to the local newClientList


                                    if (clientIDStr == "newClient")
                                    {

                                        client newClient = new client();
                                        newClient.clientName = newOrder.Client_Name;
                                        newClient.clientStatus = "new";

                                        connection.Open();

                                        string insertClient = "INSERT into clients (client_name, client_status) VALUES('";
                                        insertClient += newClient.clientName + "', '";
                                        insertClient += newClient.clientStatus + "');";

                                        MySqlCommand cmdInsert = new MySqlCommand(insertClient, connection);
                                        try
                                        {
                                            cmdInsert.ExecuteNonQuery();

                                            MessageBox.Show("Client successfully entered into database. Select \"Request New Clients\" to update" +
                                                " Clients table and review their info.", "Success");

                                        }
                                        catch (Exception exc)
                                        {
                                            MessageBox.Show("Error: " + exc.ToString() + "\nUnable to add new client to TMS database.", "Database Error");
                                        }
                                        finally
                                        {
                                            connection.Close();
                                        }


                                    }
                                    // end if (clientIDStr == "newClient")

                                    // client already exists in client table

                                    else
                                    {
                                        MessageBox.Show("Client already exists in database. Select \"Request New Clients\" to update" +
                                                " Clients table and review their info.", "Error");


                                    }

                                }
                                // end if (MessageBoxButton result == "Yes"

                            }


                        }
                        catch (Exception er)
                        {
                            MessageBox.Show("Error: " + er.ToString() + "\nOrderID not generated for order inserted.", "Database Error");
                        }
                        finally
                        {

                            connection.Close();

                        }

                    }

                    // end try -catch - insert order into database
                    catch (Exception exc)
                    {
                        MessageBox.Show("Error: " + exc.ToString() + "\nUnable to add order to TMS database.", "Database Error");
                    }
                    finally
                    {
                        connection.Close();
                    }

                }


                // reset the checkboxes once order is submitted


            } // end else
        }





        //  EVENT       :   RequestNewClients_Click
        //  PURPOSE     :   Reads tms database client list and includes them in the new client list accept/review table. Updates local client list.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void RequestNewClientsButton_Click(object sender, RoutedEventArgs e)
        {

            string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
            string database = "tms";
            string uid = "jiangyinruo";
            string pwd = "123456";
            string port = ConfigurationManager.AppSettings["port"].ToString();
            string connectionString;
            connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, pwd);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // display only clients that are old
                string sqlString = "SELECT * FROM clients WHERE client_status=\'new\';";
                MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    // read each line from database
                    while (reader.Read())
                    {
                        client newClient = new client();
                        newClient.clientID = Int32.Parse(reader[0].ToString());
                        newClient.clientName = reader[1].ToString();
                        newClient.clientStatus = reader[2].ToString();


                        if (!clientList.Contains(newClient))
                        {
                            this.newClientList.Add(newClient);
                        }
                    }

                    NewClientDataGrid.ItemsSource = this.newClientList;

                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error: " + exc.ToString() + "\nUnable to update local client list.", "Database error");
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }


            }
        }



        //  EVENT       :   AcceptCustomer_Click
        //  PURPOSE     :   Updates the client status from new to active and moves them to the current customers table. Updates tms system with new client status.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void AcceptCustomer_Click(object sender, RoutedEventArgs e)
        {

            // no clients in the list
            if (newClientList.Count == 0)
            {
                MessageBox.Show("Error: No new clients available to accept.\n" +
                    "Select Request New clients to see any new clients available.", "Error");
            }
            // no client has been selected
            else if (NewClientDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Error: Select a client before clicking accept", "Error");
            }

            // proceed with acceptance
            // modifies the client status to current and moves the client from the bottom window to the top window
            // removes client from the new list and adds it to the client list
            else
            {

                client itemFound = (client)NewClientDataGrid.SelectedItem;

                int idToFind = itemFound.clientID;


                // find the matching ID in the database and update
                string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
                string database = "tms";
                string uid = "jiangyinruo";
                string pwd = "123456";
                string port = ConfigurationManager.AppSettings["port"].ToString();
                string connectionString;
                connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, pwd);
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();




                    string sqlString = "UPDATE clients SET client_status='active' WHERE clientID=" + idToFind.ToString() + ";";
                    MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                    try
                    {
                        cmd.ExecuteNonQuery();

                        // update the newClientList and clientList

                        newClientList.Remove(itemFound);

                        itemFound.clientStatus = "active";
                        clientList.Add(itemFound);
                    }
                    catch (Exception eD)
                    {
                        MessageBox.Show("Error: " + eD.ToString() + "\nUnable to update client status to active", "Database error");
                    }
                    finally
                    {
                        connection.Close();
                    }




                }






            }



        }



        //  EVENT       :   RejectCustomer_Click
        //  PURPOSE     :   Removes client from client list and tms client table.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void RejectCustomer_Click(object sender, RoutedEventArgs e)
        {
            // no clients in the list
            if (newClientList.Count == 0)
            {
                MessageBox.Show("Error: No new clients in list.\n" +
                    "Select Request New clients to see any new clients available.", "Error");
            }
            // no client has been selected
            else if (NewClientDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Error: Select a client to reject.", "Error");
            }

            // proceed with acceptance
            // modifies the client status to current and moves the client from the bottom window to the top window
            // removes client from the new list and adds it to the client list
            else
            {

                client itemFound = (client)NewClientDataGrid.SelectedItem;

                int idToFind = itemFound.clientID;


                // find the matching ID in the database and update
                string server = "127.0.0.1";
                string database = "tms";
                string uid = "jiangyinruo";
                string password = "123456";
                string connectionString;
                connectionString = string.Format("Server={0}; Port=3306; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, password);
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();




                    string sqlString = "DELETE FROM clients WHERE clientID=" + idToFind.ToString() + ";";
                    MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();


                }

                // update the newClientList and clientList

                newClientList.Remove(itemFound);


            }
        }


        //  EVENT       :   CompletedCustomer_Click
        //  PURPOSE     :   Updates order data grid with completed orders for invoice generation. Selects any items from tms orders table that are done.
        //  PARAMETERS  :   object sender
        //                  RoutedEventArgs e
        //  RETURNS     :   NONE
        private void CompletedOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            string server = ConfigurationManager.AppSettings["ipAddress"].ToString();
            string database = "tms";
            string uid = "jiangyinruo";
            string pwd = "123456";
            string port = ConfigurationManager.AppSettings["port"].ToString();
            string connectionString;
            connectionString = string.Format("Server={0}; Port=" + port + "; Database={1}; Uid ={2}; Pwd ={3}; ", server, database, uid, pwd);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sqlOrderCmd = "SELECT * FROM orders WHERE order_status='complete';";
                MySqlCommand cmdOrders = new MySqlCommand(sqlOrderCmd, connection);


                MySqlDataReader orderReader = cmdOrders.ExecuteReader();
                try
                {
                    int count = 0;
                    // read each order from tms.orders
                    while (orderReader.Read())
                    {

                        order newOrder = new order();
                        // include each field
                        newOrder.orderID = Int32.Parse(orderReader[0].ToString());
                        newOrder.Client_Name = orderReader[1].ToString();
                        newOrder.Job_Type = Int32.Parse(orderReader[2].ToString());
                        newOrder.Quantity = Int32.Parse(orderReader[3].ToString());
                        newOrder.Origin = orderReader[4].ToString();
                        newOrder.relevant_cities = orderReader[5].ToString();
                        newOrder.Destination = orderReader[6].ToString();
                        newOrder.Van_Type = Int32.Parse(orderReader[7].ToString());
                        newOrder.order_status = orderReader[8].ToString();
                        newOrder.create_time = orderReader[9].ToString();
                        newOrder.carrierID = Int32.Parse(orderReader[10].ToString());

                        if (!orderList.Contains(newOrder))
                        {
                            this.orderList.Add(newOrder);
                        }
                        if (!completedOrderList.Contains(newOrder))
                        {
                            this.completedOrderList.Add(newOrder);
                            count++;
                        }

                    }

                    // inform user of successful order search, even if no items found
                    MessageBox.Show("New completed orders found: " + count.ToString());


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString() + "\nUnable to update order list.\nContact database admin for support.", "Database Connection Error");

                }
                finally
                {
                    orderReader.Close();

                    connection.Close();
                }
            }



        }

        public string relevantCities = "";
        private void WindsorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.WindsorCheckBox.IsChecked == true)
            {
                relevantCities += "Windsor&";
            }
        }

        private void LondonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.LondonCheckBox.IsChecked == true)
            {
                relevantCities += "London&";
            }
        }

        private void HamiltonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.HamiltonCheckBox.IsChecked == true)
            {
                relevantCities += "Hamilton&";
            }
        }

        private void TorontoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.TorontoCheckBox.IsChecked == true)
            {
                relevantCities += "Toronto&";
            }
        }

        private void OshawaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.OshawaCheckBox.IsChecked == true)
            {
                relevantCities += "Oshawa&";
            }
        }

        private void BellevilleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.BellevilleCheckBox.IsChecked == true)
            {
                relevantCities += "Belleville&";
            }
        }

        private void KingstonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.KingstonCheckBox.IsChecked == true)
            {
                relevantCities += "Kingston&";
            }
        }

        private void OttawaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.OttawaCheckBox.IsChecked == true)
            {
                relevantCities += "Ottawa&";
            }
        }
    }



}