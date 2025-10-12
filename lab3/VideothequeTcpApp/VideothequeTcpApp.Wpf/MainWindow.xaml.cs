using System.Collections.ObjectModel;
using System.Windows;
using VideothequeTcpApp.Networking;
using VideothequeTcpApp.Wpf.Models;

namespace VideothequeTcpApp.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly RestOverTcpClient _client;
        private readonly ObservableCollection<Video> _videos = new();
        private readonly ObservableCollection<Customer> _customers = new();
        private readonly ObservableCollection<Rental> _rentals = new();
        private readonly ObservableCollection<Report> _reports = new();

        public MainWindow()
        {
            InitializeComponent();
            _client = new RestOverTcpClient("127.0.0.1", 8888);
            VideosDataGrid.ItemsSource = _videos;
            CustomersDataGrid.ItemsSource = _customers;
            RentalsDataGrid.ItemsSource = _rentals;
            ReportTextBox.DataContext = _reports;
            CustomerComboBox.ItemsSource = _customers;
            VideoComboBox.ItemsSource = _videos;
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VideosDataGrid.IsVisible)
                {
                    var all = await _client.GetAsync<Video[]>("/api/videos");
                    _videos.Clear();
                    foreach (var v in all ?? Array.Empty<Video>())
                        _videos.Add(v);
                    VideoComboBox.ItemsSource = _videos;
                }
                else if (CustomersDataGrid.IsVisible)
                {
                    var all = await _client.GetAsync<Customer[]>("/api/customers");
                    _customers.Clear();
                    foreach (var c in all ?? Array.Empty<Customer>())
                        _customers.Add(c);
                    CustomerComboBox.ItemsSource = _customers; 
                }
                else if (RentalsDataGrid.IsVisible)
                {
                    var all = await _client.GetAsync<Rental[]>("/api/rentals");
                    _rentals.Clear();
                    foreach (var r in all ?? Array.Empty<Rental>())
                        _rentals.Add(r);
                    CustomerComboBox.ItemsSource = _customers; 
                    VideoComboBox.ItemsSource = _videos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VideosDataGrid.IsVisible)
                {
                    var video = new Video
                    {
                        Title = "New Video",
                        Genre = "Genre",
                        Year = 2025,
                        Director = "Director",
                        Price = 1,
                        AvailableCopies = 10
                    };
                    var created = await _client.PostAsync<Video>("/api/videos", video);
                    if (created != null) _videos.Add(created);
                    VideoComboBox.ItemsSource = _videos;
                }
                else if (CustomersDataGrid.IsVisible)
                {
                    var customer = new Customer
                    {
                        Name = "New Customer",
                        Email = "new@customer.com",
                        PhoneNumber = "123-456-7890",
                        RegisteredAt = DateTime.Now
                    };
                    var created = await _client.PostAsync<Customer>("/api/customers", customer);
                    if (created != null) _customers.Add(created);
                    CustomerComboBox.ItemsSource = _customers; 
                }
                else if (RentalsDataGrid.IsVisible)
                {
                    if (CustomerComboBox.SelectedValue == null || VideoComboBox.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a Customer and a Video.");
                        return;
                    }

                    var rental = new Rental
                    {
                        CustomerId = (Guid)CustomerComboBox.SelectedValue,
                        VideoId = (Guid)VideoComboBox.SelectedValue,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(7),
                        IsReturned = false,
                        TotalPrice = 5.99m
                    };
                    var created = await _client.PostAsync<Rental>("/api/rentals", rental);
                    if (created != null) _rentals.Add(created);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VideosDataGrid.SelectedItem is Video selectedVideo)
                {
                    var success = await _client.PutAsync<Video>("/api/videos/" + selectedVideo.Id, selectedVideo);
                    if (success != null)
                    {
                        var index = _videos.IndexOf(selectedVideo);
                        _videos[index] = success;
                    }
                }
                else if (CustomersDataGrid.SelectedItem is Customer selectedCustomer)
                {
                    var success = await _client.PutAsync<Customer>("/api/customers/" + selectedCustomer.Id, selectedCustomer);
                    if (success != null)
                    {
                        var index = _customers.IndexOf(selectedCustomer);
                        _customers[index] = success;
                    }
                }
                else if (RentalsDataGrid.SelectedItem is Rental selectedRental)
                {
                    var success = await _client.PutAsync<Rental>("/api/rentals/" + selectedRental.Id, selectedRental);
                    if (success != null)
                    {
                        var index = _rentals.IndexOf(selectedRental);
                        _rentals[index] = success;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VideosDataGrid.SelectedItem is Video selectedVideo)
                {
                    await _client.DeleteAsync("/api/videos/" + selectedVideo.Id);
                    _videos.Remove(selectedVideo);
                }
                else if (CustomersDataGrid.SelectedItem is Customer selectedCustomer)
                {
                    await _client.DeleteAsync("/api/customers/" + selectedCustomer.Id);
                    _customers.Remove(selectedCustomer);
                }
                else if (RentalsDataGrid.SelectedItem is Rental selectedRental)
                {
                    await _client.DeleteAsync("/api/rentals/" + selectedRental.Id);
                    _rentals.Remove(selectedRental);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var from = ReportFromDate.SelectedDate ?? DateTime.Now.AddYears(-1);
                var to = ReportToDate.SelectedDate ?? DateTime.Now;
                var query = $"/api/reports?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"; 
                var report = await _client.GetAsync<Report>(query);

                if (report != null)
                {
                    _reports.Clear();
                    _reports.Add(report);
                    ReportTextBox.Text = $"Total Revenue: {report.TotalRevenue}\n" +
                                        $"Most Popular Video ID: {report.MostPopularVideoId}, Rentals: {report.MostPopularVideoRentals}\n" +
                                        $"Most Profitable Video ID: {report.MostProfitableVideoId}, Revenue: {report.MostProfitableVideoRevenue}\n" +
                                        $"Average Rental Duration: {report.AverageRentalDuration:F2} days\n" +
                                        $"Top Customer ID: {report.TopCustomerId}, Spent: {report.TopCustomerSpent}";
                }
                else
                {
                    MessageBox.Show("Failed to generate report.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}