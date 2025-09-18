using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeSearchApp
{
    public partial class Form1 : Form
    {
        static readonly string apiKey = "AIzaSyBduNY1Xlm7F9DMiLbUbD88QXQOuFlDAZM";
        static readonly string dbFile = "youtube_db.json";

        private Panel historyPanel;
        private ListBox listBoxHistory;
        private Button btnBack, btnClearHistory;
        private MenuStrip menuStrip;

        public Form1()
        {
            InitializeComponent();

          
            menuStrip = new MenuStrip();
            var menuSearch = new ToolStripMenuItem("Пошук");
            var menuHistory = new ToolStripMenuItem("Історія пошуків");
            var menuExit = new ToolStripMenuItem("Вихід");

            menuSearch.Click += (s, e) => ShowSearch();
            menuHistory.Click += (s, e) => ShowHistory();
            menuExit.Click += (s, e) => Application.Exit();

            menuStrip.Items.Add(menuSearch);
            menuStrip.Items.Add(menuHistory);
            menuStrip.Items.Add(menuExit);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
            menuStrip.BringToFront();

            
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };
            this.Controls.Add(topPanel);
            topPanel.BringToFront();

            txtQuery.Left = 10;
            txtQuery.Top = 15;
            txtQuery.Width = 400;
            topPanel.Controls.Add(txtQuery);

            btnSearch.Left = 420;
            btnSearch.Top = 12;
            btnSearch.Width = 100;
            topPanel.Controls.Add(btnSearch);
            btnSearch.Click += btnSearch_Click;

           
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.WrapContents = false;
            flowPanel.FlowDirection = FlowDirection.TopDown;
            flowPanel.AutoScroll = true;
            this.Controls.Add(flowPanel);
            flowPanel.BringToFront();

            this.Resize += Form1_Resize;

            
            historyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            this.Controls.Add(historyPanel);
            historyPanel.BringToFront();

            listBoxHistory = new ListBox
            {
                Dock = DockStyle.Top,
                Height = 200
            };
            historyPanel.Controls.Add(listBoxHistory);

            btnBack = new Button
            {
                Text = "Повернутись",
                Top = 210,
                Left = 10,
                Width = 100
            };
            btnBack.Click += (s, e) => ShowSearch();
            historyPanel.Controls.Add(btnBack);

            btnClearHistory = new Button
            {
                Text = "Очистити історію",
                Top = 210,
                Left = 120,
                Width = 150
            };
            btnClearHistory.Click += (s, e) =>
            {
                listBoxHistory.Items.Clear();
                SaveDatabase(new List<YoutubeRecord>());
            };
            historyPanel.Controls.Add(btnClearHistory);
        }

        
        private void Form1_Resize(object sender, EventArgs e)
        {
            foreach (Panel p in flowPanel.Controls)
            {
                p.Width = flowPanel.ClientSize.Width - 25; 
            }
        }

        private void ShowSearch()
        {
            historyPanel.Visible = false;
            flowPanel.Visible = true;
            lstResults.Visible = false;
        }

        private void ShowHistory()
        {
            flowPanel.Visible = false;
            lstResults.Visible = true;
            historyPanel.Visible = true;

            listBoxHistory.Items.Clear();
            var db = LoadDatabase();
            for (int i = 0; i < db.Count; i++)
            {
                var r = db[i];
                listBoxHistory.Items.Add($"{i + 1}. {r.Date}: \"{r.Query}\" ({r.Videos.Count} відео)");
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtQuery.Text.Trim();
            if (string.IsNullOrWhiteSpace(query)) return;

            flowPanel.Visible = true;
            lstResults.Visible = false;
            historyPanel.Visible = false;

            List<VideoInfo> videos = await SearchYouTube(query);

            flowPanel.Controls.Clear();

            foreach (var v in videos)
            {
                Panel panel = new Panel
                {
                    Width = flowPanel.Width - 25,
                    Height = 120,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5)
                };

                PictureBox pb = new PictureBox
                {
                    Width = 160,
                    Height = 90,
                    Left = 5,
                    Top = 15,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                try { pb.Load(v.ThumbnailUrl); } catch { }

                Label lblTitle = new Label
                {
                    Text = $"🎬 {v.Title}\n📺 {v.ChannelTitle}\n📅 {v.PublishedAt}",
                    Left = 170,
                    Top = 5,
                    Width = panel.Width - 180,
                    Height = 60
                };

                LinkLabel link = new LinkLabel
                {
                    Text = "Перейти на YouTube",
                    Left = 170,
                    Top = 70,
                    AutoSize = true
                };
                link.Click += (s, ev) => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = $"https://www.youtube.com/watch?v={v.VideoId}",
                    UseShellExecute = true
                });

                panel.Controls.Add(pb);
                panel.Controls.Add(lblTitle);
                panel.Controls.Add(link);

                flowPanel.Controls.Add(panel);
            }

         
            var db = LoadDatabase();
            db.Add(new YoutubeRecord
            {
                Query = query,
                Date = DateTime.Now,
                Videos = videos
            });
            SaveDatabase(db);
        }


      
        private void btnHistory_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void btnClearDb_Click(object sender, EventArgs e)
        {
            SaveDatabase(new List<YoutubeRecord>());
            MessageBox.Show("База очищена ✅", "Інфо");
        }

        
        private async Task<List<VideoInfo>> SearchYouTube(string query)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://www.googleapis.com/youtube/v3/search" +
                             $"?part=snippet&type=video&maxResults=10&q={Uri.EscapeDataString(query)}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Error {response.StatusCode}:\n{responseBody}", "YouTube API Error");
                    return new List<VideoInfo>();
                }

                try
                {
                    var videos = new List<VideoInfo>();
                    var json = JsonDocument.Parse(responseBody);
                    var items = json.RootElement.GetProperty("items");

                    foreach (var item in items.EnumerateArray())
                    {
                        string title = item.GetProperty("snippet").GetProperty("title").GetString();
                        string channel = item.GetProperty("snippet").GetProperty("channelTitle").GetString();
                        string published = item.GetProperty("snippet").GetProperty("publishedAt").GetString();
                        string videoId = item.GetProperty("id").GetProperty("videoId").GetString();
                        string thumbnail = item.GetProperty("snippet").GetProperty("thumbnails")
                                               .GetProperty("default").GetProperty("url").GetString();

                        videos.Add(new VideoInfo
                        {
                            Title = title,
                            ChannelTitle = channel,
                            PublishedAt = published,
                            VideoId = videoId,
                            ThumbnailUrl = thumbnail
                        });
                    }

                    return videos;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("JSON parse error: " + ex.Message);
                    return new List<VideoInfo>();
                }
            }
        }

       
        static List<YoutubeRecord> LoadDatabase()
        {
            if (!File.Exists(dbFile)) return new List<YoutubeRecord>();
            string json = File.ReadAllText(dbFile);
            return JsonSerializer.Deserialize<List<YoutubeRecord>>(json) ?? new List<YoutubeRecord>();
        }

        static void SaveDatabase(List<YoutubeRecord> db)
        {
            string json = JsonSerializer.Serialize(db, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dbFile, json);
        }
    }

    public class YoutubeRecord
    {
        public string Query { get; set; }
        public DateTime Date { get; set; }
        public List<VideoInfo> Videos { get; set; }
    }

    public class VideoInfo
    {
        public string Title { get; set; }
        public string ChannelTitle { get; set; }
        public string VideoId { get; set; }
        public string PublishedAt { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
