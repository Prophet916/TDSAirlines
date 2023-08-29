using System;
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
using System.Data.Sql;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;
using System.Security.AccessControl;

namespace TDSAirlines
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BookingEntities bookingEntities = new BookingEntities();
        public MainWindow()
        {
            InitializeComponent();
            LoadAirports();
            DisplayUserName();
            DisplayBookedFlights();
        }

        // Displays current user logged in, currently hard coded
        private void DisplayUserName()
        {
            int loggedInPassengerID = 305;
            string passengerName = bookingEntities.Passengers
                .Where(p => p.PassengerID == loggedInPassengerID)
                .Select(p => p.FirstName)
                .FirstOrDefault();

            lblUserName.Content = "Welcome, " + passengerName;
            
        }

        // Displays the current users booked flights
        private void DisplayBookedFlights()
        {
            int loggedInPassengerID = 305;

            var bookedFlights = bookingEntities.Bookings
            .Where(b => b.PassengerID == loggedInPassengerID)
            .Select(b => new
            {
                BookingID = b.BookingID,
                FromAirport = bookingEntities.Airports
                    .Where(a => a.AirportID == b.Flight.DepartureAirportID)
                    .Select(a => a.AirportName)
                    .FirstOrDefault(),
                ToAirport = bookingEntities.Airports
                    .Where(a => a.AirportID == b.Flight.ArrivalAirportID)
                    .Select(a => a.AirportName)
                    .FirstOrDefault(),
                DepartureDate = b.Flight.DepartureDate 
            })
            .ToList();

            bookedFlightsDataGrid.ItemsSource = bookedFlights;

        }

        // Populates the comboboxes
        private void LoadAirports()
        {
            
            List<Airport> airports = bookingEntities.Airports.ToList();
            cmbFromAirports.ItemsSource = airports;
            cmbToAirport.ItemsSource = airports;
            

        }


        // Search for flights using the date range picked from the date picker
        private List<Flight> SearchForFlights(DateTime departureDate, DateTime returnDate)
        {
            string fromAirportID = (cmbFromAirports.SelectedItem as Airport)?.AirportID;
            string toAirportID = (cmbToAirport.SelectedItem as Airport)?.AirportID;

            var availableFlights = bookingEntities.Flights
                .Where(f => f.DepartureDate >= departureDate && f.DepartureDate <= returnDate.Date &&
                f.DepartureAirportID == fromAirportID && f.ArrivalAirportID == toAirportID)
                .ToList();

            return availableFlights;
            
        }

        // Updates the UI with the flights found through the search
        private void UpdateFlightResultsUI(List<Flight> flights)
        {
            flightsDataGrid.ItemsSource = flights;
        }

        // Starts the search for flights and calls for the UI to be updated
        private void SearchFlightsBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDepartureDate = departureDateCalendar.SelectedDate ?? DateTime.MinValue;
            DateTime selectedReturnDate = returnDateCalendar.IsEnabled ? (returnDateCalendar.SelectedDate ?? DateTime.MinValue) : DateTime.MinValue;

            List<Flight> availableFlights = SearchForFlights(selectedDepartureDate, selectedReturnDate);

            UpdateFlightResultsUI(availableFlights);
        }

        // Needs more work
        private void roundTripbtn_Checked(object sender, RoutedEventArgs e)
        {
            departureDateCalendar.IsEnabled = true;
            returnDateCalendar.IsEnabled = true;
        }

        // Needs more work
        private void oneWaybtn_Checked(object sender, RoutedEventArgs e)
        {
            returnDateCalendar.IsEnabled = false;
        }

        // Books the selected flight for the current user. Currently the user is hard coded but will change in the future
        private void BookFlightButton_Click(object sender, RoutedEventArgs e)
        {
            int loggedInPassengerID = 305;
            // Get the selected flight from the DataGrid
            Flight selectedFlight = flightsDataGrid.SelectedItem as Flight;
            //selectedFlight = flightsDataGrid.SelectedItem;

            if (selectedFlight != null)
            {
                {
                    // Get the latest BookingID from the existing bookings
                    string latestBookingID = bookingEntities.Bookings.Any() ? bookingEntities.Bookings.Max(b => b.BookingID) : "0";

                    // Generate the next BookingID by incrementing the latestBookingID
                    string nextBookingID = (int.Parse(latestBookingID) + 1).ToString();

                    Booking newBooking = new Booking
                    {
                        PassengerID = loggedInPassengerID, // Get the logged-in passenger's ID
                        FlightID = selectedFlight.FlightID, // Get the selected flight's ID
                        BookingID = nextBookingID
                    };

                    bookingEntities.Bookings.Add(newBooking);
                    bookingEntities.SaveChanges();

                    MessageBox.Show("Flight booked successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please select a flight to book.");
            }
        }
    }
}
