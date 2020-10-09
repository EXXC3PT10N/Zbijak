using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace ZbijakGame
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HelpPage));
        }

        private async void BtnExit_ClickAsync(object sender, RoutedEventArgs e)
        {
            ContentDialog exitDialog = new ContentDialog
            {
                Title = "Wyjście",
                Content = "Czy na pewno chcesz wyjść z aplikacji?",
                PrimaryButtonText = "Tak",
                CloseButtonText = "Nie"
            };

            ContentDialogResult result = await exitDialog.ShowAsync();

            
            if (result == ContentDialogResult.Primary)
                Windows.ApplicationModel.Core.CoreApplication.Exit();
        }
    }
}
