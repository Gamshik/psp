using System.Collections.ObjectModel;
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
using VideothequeTcpApp.Models.Domain;
using VideothequeTcpApp.Networking;

namespace VideothequeTcpApp.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RestOverTcpClient _client;
        private readonly ObservableCollection<Video> _videos = new();

        public MainWindow()
        {
            InitializeComponent();
            _client = new RestOverTcpClient("127.0.0.1", 8888); // IP и порт сервера
            VideosDataGrid.ItemsSource = _videos;
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var all = await _client.GetAsync<Video[]>("/videos");
                _videos.Clear();
                foreach (var v in all ?? Array.Empty<Video>())
                    _videos.Add(v);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var video = new Video
            {
                Title = "New Video",
                Genre = "Genre",
                Year = 2025,
                Director = "Director",
                Price = 1.99m
            };

            try
            {
                var created = await _client.PostAsync("/videos", video);
                if (created != null) _videos.Add(created);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideosDataGrid.SelectedItem is not Video selected) return;

            try
            {
                var success = await _client.PutAsync("/videos/" + selected.Id, selected);
                if (success != null)
                {
                    var index = _videos.IndexOf(selected);
                    _videos[index] = success;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideosDataGrid.SelectedItem is not Video selected) return;

            try
            {
                await _client.DeleteAsync("/videos/" + selected.Id);
                _videos.Remove(selected);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}