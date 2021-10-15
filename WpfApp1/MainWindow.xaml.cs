using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string token = string.Empty;
        RestClient restClient;
        HubConnection hubConnection;
        public MainWindow()
        {
            InitializeComponent();
            restClient = new RestClient("http://localhost:8429");
            hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:8429/chat", options => 
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            }).Build();

            hubConnection.On<string, string>("Receive", (user, message) =>
            {
                SendLocalMessage(user, message);
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest("http://localhost:8429/token", Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("Login", login.Text);
            request.AddParameter("Password", password.Text);

            var response = restClient.ExecuteAsync(request);
            var con = response.Result.Content;
            var data = JsonSerializer.Deserialize<TokenModel>(response.Result.Content);
            
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show(data.name_identifier);
                var str = password.Text;
                token = data.access_token;

                hubConnection.StartAsync();
            }

            
        }

        private void SendLocalMessage(string user, string message)
        {
            MessageBox.Show(user, message);
        }

        private IRestResponse Response(string login, string password)
        {
            RestRequest request = new RestRequest("http://localhost:8429/token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Login", login);
            request.AddParameter("Password", password);
            

            var response = restClient.Execute(request);
            return response;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(hubConnection.State.ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            hubConnection.InvokeAsync("Send", login.Text, "Всем привет");
        }
    }
}
