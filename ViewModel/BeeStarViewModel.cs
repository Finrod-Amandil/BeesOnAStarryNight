using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows;
using DispatcherTimer = System.Windows.Threading.DispatcherTimer;

namespace StarryNight.ViewModel
{
    using View;
    using Model;
    using System;

    class BeeStarViewModel
    {
        private readonly ObservableCollection<UIElement> _sprites = new ObservableCollection<UIElement>();
        public INotifyCollectionChanged Sprites { get { return _sprites; } }
        private readonly Dictionary<Star, StarControl> _stars = new Dictionary<Star, StarControl>();
        private readonly List<StarControl> _fadedStars = new List<StarControl>();

        private BeeStarModel _model = new BeeStarModel();
        private readonly Dictionary<Bee, AnimatedImage> _bees = new Dictionary<Bee, AnimatedImage>();
        private DispatcherTimer _timer = new DispatcherTimer();

        public Size PlayAreaSize
        {
            get { return _model.PlayAreaSize; }
            set { _model.PlayAreaSize = value; }
        }

        public BeeStarViewModel()
        {
            _model.BeeMoved += BeeMovedHandler;
            _model.StarChanged += StarChangedHandler;
            _timer.Tick += timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Start();
        }

        private void timer_Tick(object sender, object e)
        {
            foreach (StarControl star in _fadedStars)
            {
                _sprites.Remove(star);
            }
            _model.Update();
        }

        private void BeeMovedHandler(object sender, BeeMovedEventArgs e)
        {
            Bee bee = e.BeeThatMoved;
            if (_bees.ContainsKey(bee))
            {
                BeeStarHelper.MoveElementOnCanvas(_bees[bee], bee.Location.X, bee.Location.Y);
            }
            else
            {
                _bees[bee] = BeeStarHelper.BeeFactory(bee.Size.Width, bee.Size.Height, TimeSpan.FromMilliseconds(20));
                _sprites.Add(_bees[bee]);
                BeeStarHelper.SetCanvasLocation(_bees[bee], bee.Location.X, bee.Location.Y);
            }
        }

        private void StarChangedHandler(object sender, StarChangedEventArgs e)
        {
            Star star = e.StarThatChanged;
            StarControl starControl;

            if (!_stars.ContainsKey(star))
            {
                starControl = new StarControl();
                _stars[star] = starControl;
                starControl.FadeIn();
                _sprites.Add(starControl);
                BeeStarHelper.SendToBack(starControl);
            }
            else
            {
                starControl = _stars[star];
            }
            

            if (e.Removed)
            {
                _stars.Remove(star);
                _fadedStars.Add(starControl);
                starControl.FadeOut();
            }
            else
            {
                BeeStarHelper.SetCanvasLocation(starControl, star.Location.X, star.Location.Y);
            }
        }
    }
}
