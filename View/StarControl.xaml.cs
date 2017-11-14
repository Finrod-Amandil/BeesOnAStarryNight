using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace StarryNight.View
{
    /// <summary>
    /// Interaction logic for StarControl.xaml
    /// </summary>
    public partial class StarControl : UserControl
    {
        public StarControl()
        {
            InitializeComponent();
        }

        public void FadeOut()
        {
            Storyboard storyboard = FindResource("fadeOutStoryboard") as Storyboard;
            storyboard.Begin();
        }

        public void FadeIn()
        {
            Storyboard storyboard = FindResource("fadeInStoryboard") as Storyboard;
            storyboard.Begin();
        }
    }
}
