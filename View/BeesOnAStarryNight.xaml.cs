using System.Windows;

namespace StarryNight.View
{
    /// <summary>
    /// Interaction logic for BeesOnAStarryNight.xaml
    /// </summary>
    public partial class BeesOnAStarryNight : Window
    {
        ViewModel.BeeStarViewModel viewModel;

        public BeesOnAStarryNight()
        {
            InitializeComponent();

            viewModel = FindResource("viewModel") as ViewModel.BeeStarViewModel;
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            viewModel.PlayAreaSize = new Size(e.NewSize.Width, e.NewSize.Height);
        }
    }
}
