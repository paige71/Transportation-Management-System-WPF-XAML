//  FILE            :   MainWindow.xaml.cs
//  PROJECT         :   Software Quality Term Project
//  PROGRAMMER      :   Paige Lam
//  FIRST-VERSION   :   November 30, 2021
//  DESCRIPTION     :   This contains the C# code for the WPF log in window.

using System;
using System.IO;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace TMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //database connection info
        public static string name;
        public static string password1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //obtain textbox input info from user
            name = txtUserName.Text.Trim();
            password1 = txtPassWord.Password.Trim();

            if (name.Equals("") || password1.Equals(""))
            {
                MessageBox.Show("username and password are mandatory!");
            }
            else
            {
                try
                {
                    //set up mysql connection
                    string connectionString = @"server = localhost; database = tms; userid = jiangyinruo; password = 123456; ";
                    MySqlConnection connection = new MySqlConnection(connectionString);

                    //check user data
                    string sqlCommand = "select * from users where userName = '" + name + "' and userPassword = '" + password1 + "'";
                    MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);

                    //open database
                    connection.Open();

                    //check if user data exists
                    MySqlDataReader reader = cmd.ExecuteReader();
                    try
                    {
                        bool entryFound = false;

                        // read each line of table until match
                        while (reader.Read())
                        {
                            // match found, check what their userRole is
                            if (reader.HasRows)
                            {
                                // log in as admin
                                if (reader[3].ToString() == "admin")
                                {
                                    MessageBox.Show("Admin log in successful.");
                                    WriteLog("Main: Admin log in successfully");
                                    Admin adminWindow = new Admin();
                                    adminWindow.Show();
                                    entryFound = true;
                                }
                                // log in as buyer
                                else if (reader[3].ToString() == "buyer")
                                {
                                    MessageBox.Show("Buyer log in successful.");
                                    WriteLog("Main: Buyer log in successfully");
                                    Buyer buyerWindow = new Buyer(name, password1);
                                    buyerWindow.Show();
                                    entryFound = true;
                                }
                                //log in as planner
                                else
                                {
                                    MessageBox.Show("Planner log in successful.");
                                    WriteLog("Main: Planner log in successfully");
                                    Planner plannerWindow = new Planner();
                                    plannerWindow.Show();
                                    entryFound = true;
                                }
                            }
                        }
                        // no user found, display error message
                        if (entryFound == false)
                        {
                            MessageBox.Show("Invalid login. Check correct username and password used.");
                            WriteLog("Invalid login.");
                            reader.Close();
                            connection.Close();
                        }
                        // user found, close login page
                        else
                        {
                            reader.Close();
                            connection.Close();
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString() + "Failed to connect to tms database!");
                        WriteLog("Failed to connect to tms database!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString() + "Failed to connect to tms database!");
                    WriteLog("Failed to connect to tms database!");
                }
            }
        }

        /// <summary>
        /// Set up log file path and format
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            string logPath = ConfigurationManager.AppSettings["logPath"];
            string logDirectory = logPath.Remove(logPath.LastIndexOf("\\"));

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            if (!File.Exists(logPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(logPath))
                {
                    sw.WriteLine($"{DateTime.Now} : {message}");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine($"{DateTime.Now} : {message}");
                }
            }
        }
    }
}
