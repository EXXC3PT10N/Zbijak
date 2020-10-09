using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;



namespace ZbijakGame
{
    class cGame
    {

        private List<Button> holes = new List<Button>();
        private List<Button> moles = new List<Button>();
        private TextBox tbxPoints;
        private TextBlock tblGameOver;
        private Grid gridBoard;
        private int points = 0;
        private DispatcherTimer moleTimer;
        private DispatcherTimer newMoleTimer = new DispatcherTimer();
        private int lives = 3;
        private List<Button> BtnLives = new List<Button>();
        public event EventHandler RaiseCustomEvent;
        private int initalSpeed;
        private double speedBooster;
        private string difficulty;

        MediaPlayer player;
        MediaPlayer mole_hide;
        MediaPlayer mole_hitted;

        Stopwatch timer = new Stopwatch();

        private int iteracje = 0;
        private int iteracje2 = 0;




        public cGame(Grid gridBoard, TextBox tbxPoints, TextBlock tblGameOver, int difficulty)
        {
            timer.Start();
            
            InitalizeAudio();
            SetDificulty(difficulty);
            this.gridBoard = gridBoard;
            this.tbxPoints = tbxPoints;
            this.tblGameOver = tblGameOver;
            MakeBoard();
            CreateMole();
        }

        private void SetDificulty(int difficulty)
        {
           
            switch (difficulty)
            {
                case 0:
                    this.difficulty = "Łatwy";
                    initalSpeed = 4000;
                    speedBooster = 0.02;
                    break;
                case 1:
                    this.difficulty = "Średni";
                    initalSpeed = 3000;
                    speedBooster = 0.03;
                    break;
                case 2:
                    this.difficulty = "Trudny";
                    initalSpeed = 2800;
                    speedBooster = 0.04;
                    break;
                default:
                    this.difficulty = "Średni";
                    initalSpeed = 3000;
                    speedBooster = 0.03;
                    break;
            }
        }

        private async void InitalizeAudio()
        {
            mole_hide = new MediaPlayer();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("mole_hide.mp3");

            mole_hide.Source = MediaSource.CreateFromStorageFile(file);

            mole_hitted = new MediaPlayer();
            folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            file = await folder.GetFileAsync("mole_hitted.mp3");

            mole_hitted.Source = MediaSource.CreateFromStorageFile(file);
        }

        private void MakeBoard()
        {
            

            for (int i = 0; i < lives; i++)
            {
                Button button = new Button();
                button.Height = 50;
                button.Width = 50;
                button.Margin = new Thickness((i*50)+10, 10, 0, 0);


                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/heart.png"));


                button.Background = imageBrush;



                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.VerticalAlignment = VerticalAlignment.Top;
                BtnLives.Add(button);
                gridBoard.Children.Add(button);
                
               

            }




            for (int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    Button button = new Button();
                    button.Name = "btnHole" + i;
                    button.HorizontalAlignment = HorizontalAlignment.Right;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Height = 100;
                    button.Width = 100;
                    
                    
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/hole.png"));

                    button.Background = imageBrush;
                    

                    double topMargin = 0;
                    double leftMargin = 0;

                    double rightMargin = 10;

                    switch (i)
                    {
                        case 0:
                            topMargin = button.Height * i;
                            break;
                        case 1:
                            topMargin = button.Height * i;
                            break;
                        case 2:
                            topMargin = button.Height * i;
                            break;
                        case 3:
                            topMargin = button.Height* i;
                            break;
                        case 4:
                            topMargin = button.Height * i;
                            break;
                    }

                    switch (j)
                    {
                        case 0:
                            leftMargin = -400;
                            rightMargin += 400;
                            break;
                        case 1:
                            leftMargin = -200;
                            rightMargin += 300;
                            break;
                        case 2:
                            leftMargin = 0;
                            rightMargin += 200;
                            break;
                        case 3:
                            leftMargin = 200;
                            rightMargin += 100;
                            break;
                        case 4:
                            leftMargin += 400;
                            
                            break;
                    }

                    button.Margin = new Windows.UI.Xaml.Thickness(0, topMargin, rightMargin, 0);

                    holes.Add(button);
                    gridBoard.Children.Add(holes.Last());
                    
                }
                
            }
           
        }

        private void CopyToMole(Button holeButton, bool isGolden)
        {
            
            ImageBrush imageBrush = new ImageBrush();
            if(isGolden)
                imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/goldenMole.png"));
            else imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/mole.png"));

            holeButton.Background = imageBrush;
            moles.Add(holeButton);
           
        }

        private void CopyToHole(Button moleButton)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/hole.png"));

            moleButton.Background = imageBrush;
            holes.Add(moleButton);
        }

        private void CreateMole()
        {
            
            if (moles.Count <= 0)
            {
                
                Random rng = new Random();
                bool isGolden = rng.Next(0, 3) == 2 ? true : false;
                int random = rng.Next(0, holes.Count - 1);
                CopyToMole(holes[random], isGolden);
                holes[random].Click += RemoveMole;
                holes.RemoveAt(random);

                //Timer section
                moleTimer = new DispatcherTimer();
                moleTimer.Tick += HideMole;
                int timeToHide = initalSpeed - Convert.ToInt32(timer.ElapsedMilliseconds * speedBooster);
                if (isGolden)
                    timeToHide /= 2;

                if (timeToHide < 10)
                    timeToHide = 10;
                moleTimer.Interval = new TimeSpan(0, 0, 0, 0, timeToHide);
                moleTimer.Start();
            }

        }

        private void CreateMole(object sender, object e)
        {
            iteracje2++;
            if (iteracje2 <= 1)
            {
                newMoleTimer.Stop();

                CreateMole();
                iteracje2--;
            }
            
        }

        private void HideMole(object sender, object e)
        {
            try
            {
                RemoveMole(moles.First());
                lives--;
                gridBoard.Children.Remove(BtnLives.Last());
                BtnLives.Remove(BtnLives.Last());
                mole_hide.Play();
                if (lives == 0)
                    GameOver();
            }
            catch
            {
                GameOver();
            }
            
            
        }

        private void RemoveMole(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mole_hitted.Play();

            iteracje++;
            
            Button button = sender as Button;
            ImageBrush bgImg = (ImageBrush) button.Background;
            ImageSource imgSrc = bgImg.ImageSource;
            if (imgSrc.GetValue(BitmapImage.UriSourceProperty).ToString() == "ms-appx:///Assets/goldenMole.png")
                AddPoints(true);
            else AddPoints(false);

            button.Click -= RemoveMole;
            CopyToHole(button);
            moles.Remove(button);
            
            RemoveMole();
        }

        

        private void RemoveMole(Button moleButton)
        {
            CopyToHole(moleButton);
            moles.Remove(moleButton);
            

            RemoveMole();
        }

        private void RemoveMole()
        {
            moleTimer.Stop();
            
            Random rng = new Random();
            newMoleTimer.Interval = new TimeSpan(0, 0, rng.Next(1, 4));

            newMoleTimer.Tick += CreateMole;
            
            
            if (!newMoleTimer.IsEnabled)
            {

               newMoleTimer.Start();
            }
                
        }

        

        private void AddPoints(bool isGolden)
        {
            // TODO zrobić punktacje
            // koncept: punkty += czas gry * 1.5
            // koncept2: punkty = (punkty + 5) * 1.1
            if (isGolden)
                points += 20;
            else points += 10;
            tbxPoints.Text = points.ToString();
            
        }

        public void GameOver()
        {
            
            newMoleTimer.Stop();
            moleTimer.Stop();
            tblGameOver.Visibility = Visibility.Visible;

            EventHandler handler = RaiseCustomEvent;
            handler(this, new EventArgs());

        }

        public void Exit()
        {
            player.Pause();
            GameOver();
        }


        public async void LoadMusic()
        {
            
            player = new MediaPlayer();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("bg_music.mp3");

            player.Source = MediaSource.CreateFromStorageFile(file);
            player.IsLoopingEnabled = true;
            player.Play();
            
        }

        public string GetDifficulty()
        {
            return this.difficulty;
        }


    }
}
