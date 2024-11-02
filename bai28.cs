using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace phandinhco2122110336
{
    public partial class bai28 : Form
    {

        private bool hasDisplayedLeaderboard = false;

        PictureBox pbBasket = new PictureBox();
        PictureBox pbEgg = new PictureBox();
        PictureBox pbBom = new PictureBox();
        PictureBox pbChicken = new PictureBox();
        Button btnRestart = new Button();

        Timer tmEgg = new Timer();
        Timer tmChicken = new Timer();
        Timer tmBom = new Timer();
        

        SoundPlayer backgroundMusic;

        int xBasket = 300;
        int yBasket = 285;
        int xDeltaBasket = 30;

        int xChicken = 300;
        int yChicken = 10;
        int xDeltaChicken = 3;

        int xEgg = 300;
        int yEgg = 10;
        int yDeltaEgg = 3;

        int xBom = 300;
        int yBom = 10;
        int yDeltaBom = 2;

        bool isEggBroken = false;
        bool isBomBroken = false;
        bool isBomFalling = false;

        int time = 0;
        int diem = 0;

        Button btnNextLevel = new Button();
        int currentLevel = 1;
        private string playerName;
        SoundPlayer scoreSound;

        public bai28()
        {
            InitializeComponent();
            SetupRestartButton();
            SetupNextLevelButton();

            try
            {
                backgroundMusic = new SoundPlayer("../../Images/backgroundmusic.wav");
                backgroundMusic.Load();

                // Tải âm thanh điểm
                scoreSound = new SoundPlayer("../../Images/scoreSound.wav");
                scoreSound.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải nhạc nền hoặc âm thanh điểm: " + ex.Message);
            }
        }
        private bool IsPlayerNameExists(string playerName)
        {
            string filePath = @"D:\phandinhco2122110336\Images\points.txt";
            if (File.Exists(filePath))
            {
                var scores = File.ReadAllLines(filePath)
                                 .Select(line => line.Split(':'))
                                 .Select(parts => parts[0].Trim()); // Chỉ lấy tên người chơi

                return scores.Contains(playerName); // Kiểm tra xem tên có tồn tại không
            }
            return false; // Nếu file không tồn tại, trả về false
        }


public class LeaderboardForm : Form
    {
        private TextBox txtLeaderboard;

        public LeaderboardForm(string leaderboardText)
        {
            Text = "Bảng Xếp Hạng";
            Size = new System.Drawing.Size(300, 400);

            txtLeaderboard = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Text = leaderboardText
            };

            Controls.Add(txtLeaderboard);

            Button closeButton = new Button
            {
                Text = "Close",
                Dock = DockStyle.Bottom
            };
            closeButton.Click += (sender, e) => Close();
            Controls.Add(closeButton);
        }
    }

    public partial class PlayerNameForm : Form
        {
            public string PlayerName { get; private set; }

            public PlayerNameForm()
            {
                
                this.Text = "Nhập Tên Người Chơi";
                this.Size = new Size(300, 150);

                Label lblName = new Label() { Text = "Tên của bạn:", Location = new Point(10, 20) };
                TextBox txtName = new TextBox() { Location = new Point(110, 20), Width = 150 };
                Button btnOk = new Button() { Text = "OK", Location = new Point(100, 60) };

                btnOk.Click += (sender, e) =>
                {
                    PlayerName = txtName.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                this.Controls.Add(lblName);
                this.Controls.Add(txtName);
                this.Controls.Add(btnOk);
            }
        }

        private void SetupNextLevelButton()
        {
            btnNextLevel.Text = "Qua Màn";
            btnNextLevel.Size = new Size(100, 50);
            btnNextLevel.Location = new Point((this.ClientSize.Width - btnNextLevel.Width) / 2, (this.ClientSize.Height - btnNextLevel.Height) / 2 + 60);
            btnNextLevel.Click += BtnNextLevel_Click;
            btnNextLevel.Visible = false;
            this.Controls.Add(btnNextLevel);
        }
        private void BtnNextLevel_Click(object sender, EventArgs e)
        {
            NextLevel();
            btnNextLevel.Visible = false;
            // Đặt lại vị trí các đối tượng khi qua màn mới
            yEgg = 10;
            xEgg = pbChicken.Location.X;
            yBom = 10;
            xBom = pbChicken.Location.X;

            pbEgg.Location = new Point(xEgg, yEgg);
            pbBom.Location = new Point(xBom, yBom);

            StartAllTimers(); // Khởi động lại các Timer để tiếp tục trò chơi bình thường
            this.Focus();
        }
        private void StartAllTimers()
        {
            tmEgg.Start();
            tmBom.Start();
            tmChicken.Start();
            tmStopwatch.Start();
        }

        private void SetupRestartButton()
        {
            btnRestart.Text = "Chơi lại";
            btnRestart.Size = new Size(100, 50);
            btnRestart.Location = new Point((this.ClientSize.Width - btnRestart.Width) / 2, (this.ClientSize.Height - btnRestart.Height) / 2);
            btnRestart.Click += BtnRestart_Click;
            btnRestart.Visible = false;
            this.Controls.Add(btnRestart);
        }

        private void bai28_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile("../../Images/background.jpeg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Thiết lập timer, nhưng không khởi động chúng ở đây
            tmEgg.Interval = 10;
            tmEgg.Tick += TmEgg_Tick;

            tmBom.Interval = 10;
            tmBom.Tick += TmBom_Tick;

            tmChicken.Interval = 10;
            tmChicken.Tick += TmChicken_Tick;

            tmStopwatch.Interval = 1000;
            tmStopwatch.Tick += tmStopwatch_Tick;

            // Hiện form nhập tên
            PlayerNameForm nameForm = new PlayerNameForm();
            while (true) // Vòng lặp cho đến khi người dùng nhập tên hợp lệ
            {
                if (nameForm.ShowDialog() == DialogResult.OK)
                {
                    playerName = nameForm.PlayerName;

                    // Kiểm tra tên người chơi
                    if (!IsPlayerNameExists(playerName))
                    {
                        break; // Nếu tên không trùng lặp, thoát khỏi vòng lặp
                    }
                    else
                    {
                        MessageBox.Show("Tên người chơi đã tồn tại. Vui lòng nhập tên khác.", "Lỗi");
                    }
                }
                else
                {
                    this.Close(); // Đóng trò chơi nếu không nhập tên
                    return;
                }
            }

            StartGame(); // Bắt đầu trò chơi sau khi nhập tên hợp lệ
        }
        private void StartGame()
        {
            InitializeGameObjects();
            tmEgg.Start();
            tmBom.Start();
            tmChicken.Start();
            tmStopwatch.Start();
            backgroundMusic.PlayLooping();
        }
        private void InitializeGameObjects()
        {
            pbBasket.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBasket.Size = new Size(50, 50);
            pbBasket.Location = new Point(xBasket, yBasket);
            pbBasket.BackColor = Color.Transparent;
            this.Controls.Add(pbBasket);
            pbBasket.Image = Image.FromFile("../../Images/basket.png");

            pbEgg.SizeMode = PictureBoxSizeMode.StretchImage;
            pbEgg.Size = new Size(20, 30);
            pbEgg.Location = new Point(xEgg, yEgg);
            pbEgg.BackColor = Color.Transparent;
            this.Controls.Add(pbEgg);
            pbEgg.Image = Image.FromFile("../../Images/ball.png");

            pbBom.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBom.Size = new Size(30, 30);
            pbBom.Location = new Point(xBom, yBom);
            pbBom.BackColor = Color.Transparent;
            this.Controls.Add(pbBom);
            pbBom.Image = Image.FromFile("../../Images/bom.png");

            pbChicken.SizeMode = PictureBoxSizeMode.StretchImage;
            pbChicken.Size = new Size(70, 70);
            pbChicken.Location = new Point(xChicken, yChicken);
            pbChicken.BackColor = Color.Transparent;
            this.Controls.Add(pbChicken);
            pbChicken.Image = Image.FromFile("../../Images/chicken.png");
        }

        private void TmChicken_Tick(object sender, EventArgs e)
        {
            if (isBomBroken || isEggBroken) return;
            xChicken += xDeltaChicken;
            if (xChicken > this.ClientSize.Width - pbChicken.Width || xChicken <= 0)
                xDeltaChicken = -xDeltaChicken;
            pbChicken.Location = new Point(xChicken, yChicken);
        }
        private void EnsureNoOverlap()
        {
            Random random = new Random();

            // Vòng lặp cho đến khi vị trí bom và trứng không trùng nhau
            while (Math.Abs(xEgg - xBom) < pbEgg.Width && Math.Abs(yEgg - yBom) < pbEgg.Height)
            {
                xBom = random.Next(0, this.ClientSize.Width - pbBom.Width); // Tạo vị trí ngẫu nhiên cho bom
            }

            pbBom.Location = new Point(xBom, yBom);
        }

        private void TmBom_Tick(object sender, EventArgs e)
        {
            EnsureNoOverlap();
            if (isEggBroken) return;
            if (!isBomFalling) return;

            yBom += yDeltaBom;

            if (yBom > this.ClientSize.Height)
            {
                
                isBomFalling = false;
                return;
            }

            Rectangle unionreactbom = Rectangle.Intersect(pbBom.Bounds, pbBasket.Bounds);
            if (!unionreactbom.IsEmpty)
            {
                isBomBroken = true;
                tmStopwatch.Stop();
                backgroundMusic.Stop();
                btnRestart.Visible = true;
                SaveScore(); // Lưu điểm số khi thua
                             // Hiển thị bảng xếp hạng chỉ nếu chưa hiển thị trước đó
                if (!hasDisplayedLeaderboard)
                {
                    LoadScores(); // Hiển thị bảng xếp hạng
                    hasDisplayedLeaderboard = true; // Đánh dấu đã hiển thị
                    tmBom.Stop(); // Dừng timer để tránh chạy lại
                    tmEgg.Stop(); // Dừng tất cả các timer khác nếu cần
                }
                return;
            }
            pbBom.Location = new Point(xBom, yBom);
        }
        private void SaveScore()
        {
            string path = @"D:\phandinhco2122110336\Images\points.txt";
            bool isUpdated = false;

            try
            {
                var existingScores = File.Exists(path)
                    ? File.ReadAllLines(path).Select(line => line.Split(':')).ToList()
                    : new List<string[]>();

                // Kiểm tra nếu tên người chơi đã tồn tại, không thực hiện ghi mới
                if (existingScores.Any(score => score[0].Trim() == playerName))
                {
                    return; // Dừng việc ghi điểm nếu tên đã tồn tại
                }

                // Thêm điểm mới cho người chơi chưa có trong tệp
                existingScores.Add(new[] { playerName, diem.ToString() });

                // Ghi lại danh sách vào tệp, mỗi dòng có định dạng "Tên: Điểm"
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (var score in existingScores)
                    {
                        writer.WriteLine($"{score[0].Trim()}: {score[1].Trim()}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu điểm: " + ex.Message);
            }
        }




        private void TmEgg_Tick(object sender, EventArgs e)
        {
            if (isBomBroken) return;
            yEgg += yDeltaEgg;

            if (yEgg > this.ClientSize.Height - pbEgg.Height || yEgg <= 0)
            {
                pbEgg.Image = Image.FromFile("../../Images/ballvo.png");
                isEggBroken = true;
                SaveScore(); // Lưu điểm số khi thua
                             // Hiển thị bảng xếp hạng chỉ nếu chưa hiển thị trước đó
                if (!hasDisplayedLeaderboard)
                {
                    LoadScores(); // Hiển thị bảng xếp hạng
                    hasDisplayedLeaderboard = true; // Đánh dấu đã hiển thị
                    tmBom.Stop(); // Dừng timer để tránh chạy lại
                    tmEgg.Stop(); // Dừng tất cả các timer khác nếu cần
                }
                backgroundMusic.Stop(); 
                btnRestart.Visible = true;
                return;
            }

            Rectangle unionreact = Rectangle.Intersect(pbEgg.Bounds, pbBasket.Bounds);
            if (!unionreact.IsEmpty)
            {
                diem++;
                lbldiem.Text = "" + diem;
                scoreSound.Play();
                if (diem % 1 == 0)
                {
                    btnNextLevel.Visible = true;
                    StopAllTimers();
                }
                yEgg = 30;
                xEgg = pbChicken.Location.X;
            }

            pbEgg.Location = new Point(xEgg, yEgg);
        }
        private void StopAllTimers()
        {
            tmEgg.Stop();
            tmBom.Stop();
            tmChicken.Stop();
            tmStopwatch.Stop();
        }


        private void tmStopwatch_Tick(object sender, EventArgs e)
        {
            time++;
            lblTime.Text = time.ToString();

            if (time % 5 == 0 && !isBomFalling || time == 1)
            {
                yBom = 10;
                xBom = pbChicken.Location.X;
                pbBom.Location = new Point(xBom, yBom);
                isBomFalling = true;
                isBomBroken = false;
            }

            if (isEggBroken)
            {
                tmStopwatch.Stop();
            }
        }

        private void NextLevel()
        {
            currentLevel++;
            xDeltaChicken += 1;
            yDeltaEgg += 1;
            yDeltaBom += 2;
            switch (currentLevel)
            {
                case 1:
                    pbChicken.Image = Image.FromFile("../../Images/chicken.png");
                    pbBasket.Image = Image.FromFile("../../Images/basket.png");
                    this.BackgroundImage = Image.FromFile("../../Images/background.jpeg");
                    break;
                case 2:
                    pbChicken.Image = Image.FromFile("../../Images/chicken2.png");
                    pbBasket.Image = Image.FromFile("../../Images/basket2.png");
                    this.BackgroundImage = Image.FromFile("../../Images/background2.jpeg");
                    break;
                case 3:
                    pbChicken.Image = Image.FromFile("../../Images/chicken3.png"); 
                    pbBasket.Image = Image.FromFile("../../Images/basket.png");
                    this.BackgroundImage = Image.FromFile("../../Images/background3.jpeg");
                    break;
                
                default:
                    
                    break;
            }

        }

        private void bai28_KeyDown(object sender, KeyEventArgs e)
        {
            if (isBomBroken || isEggBroken) return;
            if (e.KeyValue == 39 && (xBasket < this.ClientSize.Width - pbBasket.Width))
                xBasket += xDeltaBasket;
            if (e.KeyValue == 37 && xBasket > 0)
                xBasket -= xDeltaBasket;
            pbBasket.Location = new Point(xBasket, yBasket);
        }
        private LeaderboardForm leaderboardForm; // Biến tham chiếu để theo dõi instance của form

        private void LoadScores()
        {
            if (leaderboardForm != null && !leaderboardForm.IsDisposed)
            {
                // Bảng xếp hạng đã mở; không tạo thêm
                leaderboardForm.BringToFront();
                return;
            }

            string filePath = @"D:\phandinhco2122110336\Images\points.txt"; // Đường dẫn đến file điểm số

            if (File.Exists(filePath))
            {
                var scores = File.ReadAllLines(filePath)
                                 .Select(line => line.Split(':'))
                                 .Select(parts => new { Name = parts[0], Score = int.Parse(parts[1]) })
                                 .OrderByDescending(s => s.Score)
                                 .Take(10)
                                 .ToList();

                StringBuilder leaderboardText = new StringBuilder();
                leaderboardText.AppendLine("Bảng xếp hạng :");

                foreach (var score in scores)
                {
                    leaderboardText.AppendLine($"{score.Name}: {score.Score}");
                }

                // Hiển thị bảng xếp hạng trong form tùy chỉnh
                leaderboardForm = new LeaderboardForm(leaderboardText.ToString());
                leaderboardForm.Show();
            }
            else
            {
                MessageBox.Show("Chưa có điểm nào được lưu.", "Bảng Xếp Hạng");
            }
        }


        private void BtnRestart_Click(object sender, EventArgs e)
        {
            tmBom.Stop();
            tmEgg.Stop();

            isEggBroken = false;
            isBomBroken = false;
            isBomFalling = false;
            time = 0;
            diem = 0;

            lbldiem.Text = "0";
            lblTime.Text = "0";

            xBasket = 300;
            xChicken = 300;
            yEgg = 10;
            yBom = 10;

            pbBasket.Location = new Point(xBasket, yBasket);
            pbChicken.Location = new Point(xChicken, yChicken);
            pbEgg.Location = new Point(xEgg, yEgg);
            pbBom.Location = new Point(xBom, yBom);

            pbEgg.Image = Image.FromFile("../../Images/ball.png");
            tmEgg.Start();
            tmBom.Start();
            tmChicken.Start();
            tmStopwatch.Start();
            LoadScores();
            backgroundMusic.PlayLooping();
            btnRestart.Visible = false;
            hasDisplayedLeaderboard = false;
            leaderboardForm = null;
            this.Focus();
        }
    }
}
