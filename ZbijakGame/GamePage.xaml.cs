using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace ZbijakGame
{
    
    public sealed partial class GamePage : Page
    {
        cGame game;
        public GamePage()
        {
            this.InitializeComponent();
           
        }

        private void ABtnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                game.Exit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            
            Frame.Navigate(typeof(MainPage));
        }

        private void BtnEasy_Click(object sender, RoutedEventArgs e)
        {
            game = new cGame(gridBoard, TbxPoints, TblGameOver, 0);
            game.LoadMusic();
            gridBoard.Visibility = Visibility.Visible;
            gridDifficulty.Visibility = Visibility.Collapsed;
            game.RaiseCustomEvent += Game_GameOvered;
        }

        private void Game_GameOvered(object sender, EventArgs e)
        {
            gridBoard.Visibility = Visibility.Collapsed;
            gridAfterGameOver.Visibility = Visibility.Visible;
            tblRank.Text = "Ranking\n" + ReadFromFile();
            
        }

        private void BtnMedium_Click(object sender, RoutedEventArgs e)
        {
            game = new cGame(gridBoard, TbxPoints, TblGameOver, 1);
            game.LoadMusic();
            gridBoard.Visibility = Visibility.Visible;
            gridDifficulty.Visibility = Visibility.Collapsed;
            game.RaiseCustomEvent += Game_GameOvered;
        }

        private void BtnHard_Click(object sender, RoutedEventArgs e)
        {
            game = new cGame(gridBoard, TbxPoints, TblGameOver, 2);
            game.LoadMusic();
            gridBoard.Visibility = Visibility.Visible;
            gridDifficulty.Visibility = Visibility.Collapsed;
            game.RaiseCustomEvent += Game_GameOvered;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile(tbxName.Text, game.GetDifficulty());
            tblRank.Text = "Ranking\n" + ReadFromFile();
        }

        private void SaveToFile(string name, string difficulty)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {


                //if (!isoStore.FileExists("SavedGame.txt"))
                //{
                //    isoStore.CreateFile("SavedGame.txt");
                //    System.Diagnostics.Debug.WriteLine("Created a new file in the Saves.");
                //}

                IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream("SavedGame.txt", FileMode.Append, isoStore);
                StreamWriter writer = new StreamWriter(isoStream);
                writer.WriteLine(TbxPoints.Text + ":" + name + ":" + difficulty);
                writer.Dispose();
            }
        }

        private string ReadFromFile()
        {
            string data = "";

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {


                if (!isoStore.FileExists("SavedGame.txt"))
                {
                    return data;
                }
                else
                {
                    IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream("SavedGame.txt", FileMode.Open, isoStore);
                    StreamReader reader = new StreamReader(isoStream);
                    List<string> list = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        
                        list.Add(reader.ReadLine());
                    }

                    var ordered = list.Select(s => new { Str = s, Split = s.Split(':') })
                                    .OrderByDescending(x => int.Parse(x.Split[0]))
                                    .ThenBy(x => x.Split[1])
                                    .Select(x => x.Str)
                                    .ToList();

                    for (int i = 0; i < ordered.Count; i++)
                    {
                        string[] splitted = ordered[i].Split(':');
                        data += (i + 1) + ". " + splitted[1] + ": " + splitted[0] + " (" + splitted[2] + ")" + "\n";

                    }

                   
                    reader.Dispose();
                    return data;
                }
                
            }
        }

        private string ReadFromFile(string difficulty)
        {
            string data = "";

            if (difficulty == "Wszystko")
                return ReadFromFile();

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {


                if (!isoStore.FileExists("SavedGame.txt"))
                {
                    return data;
                }
                else
                {
                    IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream("SavedGame.txt", FileMode.Open, isoStore);
                    StreamReader reader = new StreamReader(isoStream);
                    List<string> list = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var nxtLine = reader.ReadLine();
                        var split = nxtLine.Split(':');
                        if(split[2] == difficulty)
                            list.Add(nxtLine);
                    }

                    var ordered = list.Select(s => new { Str = s, Split = s.Split(':') })
                                    .OrderByDescending(x => int.Parse(x.Split[0]))
                                    .ThenBy(x => x.Split[1])
                                    .Select(x => x.Str)
                                    .ToList();

                    for (int i = 0; i < ordered.Count; i++)
                    {
                        string[] splitted = ordered[i].Split(':');
                        
                            data += (i + 1) + ". " + splitted[1] + ": " + splitted[0] + " (" + splitted[2] + ")" + "\n";
                        
                        

                    }


                    reader.Dispose();
                    return data;
                }

            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            ComboBoxItem comboItem = combo.SelectedItem as ComboBoxItem;
            //System.Diagnostics.Debug.WriteLine(comboItem.Content.ToString());
            tblRank.Text = "Ranking\n" + ReadFromFile(comboItem.Content.ToString());
        }
    }
}
