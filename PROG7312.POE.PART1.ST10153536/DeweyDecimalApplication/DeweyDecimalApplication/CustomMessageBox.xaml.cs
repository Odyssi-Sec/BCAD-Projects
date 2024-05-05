using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace DeweyDecimalApplication
{
    public partial class CustomMessageBox : Window
    {
        public string Message { get; set; }
        public bool IsCorrect { get; set; }

        public CustomMessageBox(string message, bool isCorrect, List<string> generatedCallNumbers)
        {
            InitializeComponent();
            Message = message;
            IsCorrect = isCorrect;

            ReinforceGifImage(isCorrect, generatedCallNumbers);

            DataContext = this;
        }

        private void ReinforceGifImage(bool isCorrect, List<string> generatedCallNumbers)
        {
            string gifImagePath = "Images/well-done.gif";

            if (!isCorrect)
            {
                GifImage.Source = new BitmapImage(new Uri("Images/incorrect.gif", UriKind.Relative));

                if (generatedCallNumbers != null && generatedCallNumbers.Count > 0)
                {
                    Message += Environment.NewLine + "Correct Order: " + string.Join(", ", generatedCallNumbers);
                }
                else
                {
                    Message += Environment.NewLine + "Try again.";
                }
            }
            else
            {
                GifImage.Source = new BitmapImage(new Uri(gifImagePath, UriKind.Relative));
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
