using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace RYLAdminApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient client;

        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient { BaseAddress = new Uri("http://31.58.143.7/APImailadmin/") };
            _ = LoadMailHistory();
        }

        private async void BtnSendMail_Click(object sender, RoutedEventArgs e)
        {
            string playerId = txtPlayerID.Text.Trim();
            string itemId = txtItemID.Text.Trim();
            string message = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(message))
            {
                MessageBox.Show("PlayerID and Message are required.");
                return;
            }

            var content = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string,string>("PlayerID", playerId),
    new KeyValuePair<string,string>("ItemID", itemId),
    new KeyValuePair<string,string>("Message", message),
    new KeyValuePair<string,string>("account", adminAccount),
    new KeyValuePair<string,string>("password", adminPassword)
            });

            try
            {
                var response = await client.PostAsync("admin_mailbox.php", content);
                var body = await response.Content.ReadAsStringAsync();
                lblResult.Text = "Server: " + body;

                await LoadMailHistory();
            }
            catch (Exception ex)
            {
                lblResult.Text = "Error: " + ex.Message;
            }
        }

        private async Task LoadMailHistory()
        {
            try
            {
                var response = await client.GetStringAsync("admin_mailbox_list.php");
                var json = JObject.Parse(response);

                if (json["success"]?.ToObject<bool>() == true)
                {
                    lstMailHistory.Items.Clear();
                    foreach (var mail in json["mails"])
                    {
                        string item = $"[{mail["Date"]}] PlayerID:{mail["PlayerID"]}, ItemID:{mail["ItemID"]}, Msg:{mail["Message"]}";
                        lstMailHistory.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = "Error loading mail: " + ex.Message;
            }
        }
    }
}
