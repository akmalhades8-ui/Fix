using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace RYLAdminApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnAdminLogin_Click(object sender, RoutedEventArgs e)
        {
            string account = txtAdminAccount.Text.Trim();
            string password = txtAdminPassword.Password.Trim();

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter admin account and password.");
                return;
            }

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("account", account),
                new KeyValuePair<string,string>("password", password)
            });

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync("http://31.58.143.7/APImailadmin/admin_login.php", content);
                    var body = await response.Content.ReadAsStringAsync();

                    if (!body.TrimStart().StartsWith("{"))
                    {
                        lblStatus.Text = "Invalid server response";
                        return;
                    }

                    var json = JObject.Parse(body);
                    if (json["success"]?.ToObject<bool>() == true)
                    {
                        var main = new MainWindow();
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        lblStatus.Text = json["message"]?.ToString() ?? "Login failed";
                    }
                }
                catch (System.Exception ex)
                {
                    lblStatus.Text = "Error: " + ex.Message;
                }
            }
        }
    }
}
