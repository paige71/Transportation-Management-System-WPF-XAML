//  FILE            :   Program.cs (DAL)
//  PROJECT         :   Software Quality - Term Project
//  PROGRAMMERS     :   Erica Luksts & Yinruo Jiang
//  FIRST-VERSION   :   November 23, 2021
//  DESCRIPTION     :   This file contains the main classes for the data access layer portion of the TMS term project. The classes in this file
//                      are used to perform communications between the UI, the database, and the communication layer.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// 
    /// 
    /// \class Program
    /// 
    /// \brief <b>Brief Description</b> - The Program class includes the main method for running the data access layer program
    /// 
    /// The Program class listens for any messages sent by the User interface, to send on any commands to the database or Communication layer.
    /// \author <i>Erica Luksts</i>
    class Program
    {
        
        
        /// \brief <b>Brief Description</b> - Static Main method - this method is the main executing portion of the program
        /// \details <b>Details</b>
        /// This is the Main execution portion of the program, that calls any constructor methods when detecting any user interface commands
        /// and sends any additional commands on to the database if needed.
        /// 
        /// \params string[] args - any command line arguments
        /// \returns NONE
        static void Main(string[] args)
        {
        }
    }

    
    
    
    /// 
    /// 
    /// \class Customer
    /// 
    /// \brief <b>Brief Description</b> - The Customer class includes all of the attributes and methods involved in tracking, inserting, and updating customer information.
    /// 
    /// The Customer class includes methods for inserting a new customer, updating an existing customer, read data from an existing customer, and deleting an existing customer. 
    /// This class is called from the main UI and interfaces with the SQL database. The attributes in this class include id, name, and an array for storing any contractIDs that
    /// are currently or previously associated with the customer.
    /// 
    /// \author <i>Erica Luksts</i>
    class Customer
    {

        // data members
        private Int64 customerID; /// used to track different customers based on their information

        private string customerName; /// used as a descriptive identifier for the customer

        public Int64[] contractID; /// tracks any associated contracts attached to the customer


        // ------------------- METHODS -------------------------------

        /// \brief <b>Brief Description</b> - CustomerID Property - used to access or mutate the customerID attribute
        public Int64 CustomerID
        {
            /// \brief <b>Brief Description</b> - get - customerID
            /// \details <b>Details</b>
            /// 
            /// This method returns the customerID attribute value
            /// \param NONE
            /// \returns customerID (Int64)
            get { return customerID; }


            /// \brief <b>Brief Description</b> - set - customerID
            /// \details <b>Details</b>
            /// 
            /// This method mutates the customerID attribute value
            /// \param value (Int64)
            /// \returns NONE
            set
            {
                customerID = value;
            }
        }


        /// \brief <b>Brief Description</b> - CustomerName Property - used to access or mutate the customerName attribute
        public string CustomerName
        {
            /// \brief <b>Brief Description</b> - get - customerName
            /// \details <b>Details</b>
            /// 
            /// This method returns the customerName attribute value
            /// \param NONE
            /// \returns customerName (string)
            get { return customerName; }


            /// \brief <b>Brief Description</b> - set - customerName
            /// \details <b>Details</b>
            /// 
            /// This method mutates the customerName attribute value
            /// \param value (string)
            /// \returns NONE
            set
            {
                customerName = value;
            }
        }


        /// \brief <b>Brief Description</b> - Constructor - creates a new Customer Object (default)
        /// 
        /// \details <b>Details</b>
        /// 
        /// This method initializes the customerID, customerName, and contractID data members and creates
        /// a new Customer object.
        /// \param NONE
        /// \return Customer object
        /// 
        /// 
        public Customer()
        {
            this.CustomerID = 0;
            this.CustomerName = "Unknown";
            this.contractID[0] = 0;
        }


        // method for reading data

        // method for updating data

        
    }



    /// 
    /// \class Carrier
    /// 
    /// \brief <b>Brief Description</b> - The Carrier class includes all of the attributes and methods involved in tracking, inserting, and updating carrier information.
    /// 
    /// The Carrier class includes attributes for City, FTLAvailable (number of full loads available), LTLAvailable (number of pallets light load available), 
    /// FTLRate (full load rate per km), LTLRate (single pallet load rate per km), and ReefCharge (refrigerated trailer)
    /// \author <i>Erica Luksts</i>
    class Carrier
    {
        private string city; /// city that the carrier depot is located in

        private Int32 FTLAvailable; /// number of full truck loads available at depot

        private Int32 LTLAvailable; /// number of pallets of LTL shipping available at depot

        private double FTLRate; /// full load trailer rate per km

        private double LTLRate; /// less than full load rate ( rate for each pallet) per km

        private double reefCharge; /// additional fee for refrigerated trailer
        
    }

    /// 
    /// \class Invoice
    /// 
    /// \brief <b>Brief Description</b> - The Invoice class includes all of the attributes and methods involved in creating, reading, and sending invoices.
    /// 
    /// The Invoice class includes attributes for specifying the orderID, customerID, employeeID, any rates applied and the final charges. There is a member for
    /// applying any taxes. The methods in this class are included for generating an invoice or reading any past invoice data.
    /// \author <i>Erica Luksts</i>
    class Invoice
    {
        private Int64 invoiceID; /// used to read the invoice at a later time
        private Int64 orderID; /// specifies the original contract that was made
        private Int64 customerID; /// specifies who the invoice is for
        private Int64 employeeID; /// specifies who created the invoice
        private Int64 tripID; /// the trip information for distance, time etc.
        private Int64 carrierID; /// any associated carrier fees


        /// \brief <b>Brief Description</b> - GenerateInvoice - creates a new invoice for the customer given the invoice object's ID and other attributes
        /// \details <b>Details</b> - This method is used to generate a new invoice once a trip is completed. The customerID, invoiceID, and 
        /// trip & carrier attributes are used to calculate the final fee for the customer. This method uses file I/O to generate a text file invoice for the customer.
        /// \params NONE
        /// \returns NONE
        public void GenerateInvoice()
        {

        }

        /// \brief <b>Brief Description</b> - ReadInvoice - read one of the invoices from the main database
        /// \details <b>Details</b> - This method is used to read an existing invoice from the main database. This method uses File I/O to regenerate an invoice.
        /// \params Int64 invoiceID - specifies the invoice number to read from the database
        /// \returns NONE
        public void ReadInvoice(Int64 invoiceID)
        {
            // read the invoice data from the sql database
        }
    }

    /// 
    /// \class Trip
    /// 
    /// \brief <b>Brief Description</b> - The Trip class includes the overall trip data for distance and time, as well as final & starting destinations.
    /// 
    /// The Trip class includes the tripID member for identifying specific trip, totalKilometres to calculate the distance travelled by the truck,
    /// startCity for the start destination, & endCity for the end destination. There are data members for specifying if the van is refrigerated, if the trip
    /// is using an LTL or FTL, and if there are any intermediate cities or more than one day travel for additional charges. 
    /// The methods in the Trip class are used to calculate the total kilometres, total time, and others.
    /// \author <i>Erica Luksts</i>
    class Trip
    {
        private Int64 tripID; /// trip number stored in the database
        private string tripType; /// specifies if trip is FTL or LTL
        private bool reeferVan; /// specifies if the truck is refrigerated, true if yes
        private double totalKilometres; /// distance travelled by the truck for fee calculation
        private string startCity; /// starting city of the trip
        private string[] intermediateCities; /// cities that the LTL truck could stop in
        private Int32[] intermediateCityKMs; /// kilometres between each intermediate city
        private int numOfIntermediateCities; /// adds 2 hours to trip time for each for LTL loads only
        private string endCity; /// ending city of the trip
        private Int32 totalHours; /// total hours of the trip
        private Int32 numberOfDays; /// if the trip is beyond the one day travel time, tells other program to add the $150 surcharge


        /// \brief <b>Brief Description</b> - CalculateTotalKilometres
        /// \details <b>Details</b> - This method is used to add up all of the kilometres that the truck travelled.
        /// \params Int64 tripID - specifies what trip identifier data to calculate the total kilometres
        /// \returns NONE
        public double CalculateTotalKilometres(Int64 tripID)
        {
            string startCity = this.startCity;
            double totalKilometres;

            if (this.tripType == "FTL")
            {
                string endCity = this.endCity;

                foreach (string city in this.intermediateCities)
                {
                    //totalKilometres += this.intermediateCityKMs[index];
                    
                }
                totalKilometres = 0;
            }
            else
            {
                foreach (string city in this.intermediateCities)
                {
                    //totalKilometres += this.intermediateCityKMs[index];
                    
                }
                totalKilometres = 0;
            }

            return totalKilometres;
        }
    }

    /// 
    /// \class SummaryReport
    /// 
    /// \brief <b>Brief Description</b> - The SummaryReport class allows the Planner to review all active orders and invoice data for all time or past 2 weeks.
    /// 
    /// The SummaryReport class includes methods getting all of the active orderIDs from the main database, the invoice data from the database
    /// and the invoice data from the past two weeks. There is also a method for generating a graphical map layout to show where the active Orders are currently located
    /// in proximity to one another.
    /// \author <i>Erica Luksts</i>
    class SummaryReport
    {

        /// \brief <b>Brief Description</b> - ReviewCurrentOrders
        /// \details <b>Details</b> - This method gets all of the active orders from the main database and displays the map where they are located.
        /// \params bool mapView - if true, will display where the current active orders are in a graphical map layout instead of listing
        /// \returns NONE
        public void ReviewCurrentOrders(bool mapView)
        {
            // read active order data from sql
            // display the map with all points included
        }


        /// \brief <b>Brief Description</b> - GenerateInvoiceReport
        /// \details <b>Details</b> - This method gets all of the invoice data and displays it in a report format.
        /// \params bool allTime - if true, will display all time data. if false, will display all data from past two weeks
        /// \returns NONE
        public void GenerateInvoiceReport(bool allTime)
        {

            if (allTime)
            {
                // read invoiceIds from sql
                // display invoice list for all time data
            }
            else
            {
                // read invoiceIds from sql for past two week
            }
        }

    }


}
