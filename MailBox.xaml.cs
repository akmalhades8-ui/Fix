using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace RYLWebshopApp
{
    public partial class MailBoxWindow : Window
    {
        private readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        private readonly int uid;
        private readonly string charCid;
        private readonly string mailboxListUrl = "http://31.58.143.7/APImailadmin/mailbox_list.php";
        private readonly string mailboxClaimUrl = "http://31.58.143.7/APImailadmin/mailbox_claim.php";

        public MailBoxWindow(int uid, string charCid)
        {
            InitializeComponent();
            this.uid = uid;
            this.charCid = charCid;
            txtPlayerInfo.Text = $"PlayerID: {uid}  •  Character: {charCid}";
            _ = LoadMailsAsync();
        }

        public class MailModel
        {
            public string Date { get; set; }
            public string ItemID { get; set; }
            public string Message { get; set; }
            public bool ClaimedBool { get; set; } = false;
            public string Claimed { get { return ClaimedBool ? "Yes" : "No"; } }
            public bool CanClaim { get { return !ClaimedBool; } }
            public string Identifier { get; set; }
        }

        private async Task LoadMailsAsync()
        {
            lblStatus.Text = "Loading mails...";
            try
            {
                var url = $"{mailboxListUrl}?UID={uid}";
                var response = await client.GetStringAsync(url);
                if (!response.TrimStart().StartsWith("{")) { lblStatus.Text = "Invalid server response"; return; }

                var json = JObject.Parse(response);
                if (json["status"]?.ToString() == "success")
                {
                    var mails = json["mails"];
                    var list = new List<MailModel>();
                    foreach (var m in mails)
                    {
                        string date = m["Date"]?.ToString() ?? "";
                        string itemId = m["ItemID"]?.ToString() ?? "";
                        string message = m["Message"]?.ToString() ?? "";
                        bool claimed = m["Claimed"]?.ToObject<bool>() ?? false;
                        string id = m["ID"]?.ToString() ?? $"{date}::{itemId}";
                        list.Add(new MailModel { Date = date, ItemID = itemId, Message = message, ClaimedBool = claimed, Identifier = id });
                    }
                    dgMail.ItemsSource = list;
                    lblStatus.Text = $"Loaded {list.Count} mails.";
                }
                else
                {
                    lblStatus.Text = json["msg"]?.ToString() ?? "Failed to load mails.";
                }
            }
            catch (Exception ex) { lblStatus.Text = "Error: " + ex.Message; }
        }

        private async void ClaimButton_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Text = "";
            var btn = sender as System.Windows.Controls.Button;
            if (btn == null) return;
            var mail = btn.Tag as MailModel;
            if (mail == null) return;

            if (mail.ClaimedBool) { lblStatus.Text = "Already claimed"; return; }
            var res = MessageBox.Show($"Claim item {mail.ItemID} to character {charCid}?\n\nMessage: {mail.Message}", "Confirm Claim", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            try
            {
                var values = new Dictionary<string, string> {
                    {"UID", uid.ToString() },
                    {"CID", charCid },
                    {"ItemID", mail.ItemID },
                    {"MailIdentifier", mail.Identifier }
                };
                var content = new FormUrlEncodedContent(values);
                var httpResp = await client.PostAsync(mailboxClaimUrl, content);
                var respText = await httpResp.Content.ReadAsStringAsync();
                if (!respText.TrimStart().StartsWith("{")) { lblStatus.Text = respText; return; }

                var j = JObject.Parse(respText);
                if (j["status"]?.ToString() == "success")
                {
                    mail.ClaimedBool = true;
                    dgMail.Items.Refresh();
                    lblStatus.Text = j["msg"]?.ToString() ?? "Claim success";
                }
                else
                {
                    lblStatus.Text = j["msg"]?.ToString() ?? "Claim failed";
                }
            }
            catch (Exception ex) { lblStatus.Text = "Error: " + ex.Message; }
        }
    }
}
