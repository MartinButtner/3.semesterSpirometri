using System;
using System.Windows;

namespace Spirometer.UI
{
    public partial class VideoInstruction : Window
    {
        private string _cprNumber;
        public VideoInstruction(string cprNumber)
        {
            InitializeComponent();
            _cprNumber = cprNumber;
            InstructionVideo.Source = new Uri("Video/Lungefunktionsundersøgelse_guide.mp4", UriKind.Relative);
            NextInstructionButton.Visibility = Visibility.Collapsed;
            PlayAgainButton.Visibility = Visibility.Collapsed;
        }

        private void PlayVideo_Click(object sender, RoutedEventArgs e)
        {
            InstructionVideo.Play();
            PlayVideoButton.Visibility = Visibility.Collapsed;
        }
        private void InstructionVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            NextInstructionButton.Visibility = Visibility.Visible;
            PlayAgainButton.Visibility = Visibility.Visible;
        }

        private void NextInstruction_Click(object sender, RoutedEventArgs e)
        {
            InstructionVideo.Stop();
            ShowNextInstruction();
        }
        private void ShowNextInstruction()
        {
            InstructionText nextWindow = new InstructionText(_cprNumber);
            nextWindow.Show();
            this.Close();
        }
        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            InstructionVideo.Stop();
            InstructionVideo.Position = TimeSpan.Zero;
            InstructionVideo.Play();
            PlayAgainButton.Visibility = Visibility.Collapsed;
            NextInstructionButton.Visibility = Visibility.Collapsed;
        }
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            InstructionText skip = new InstructionText(_cprNumber);
            skip.Show();
            this.Close();
        }
    }
}