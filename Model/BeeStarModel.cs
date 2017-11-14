using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StarryNight.Model
{
    class BeeStarModel
    {
        public static readonly Size StarSize = new Size(100, 75);

        private readonly Dictionary<Bee, Point> _bees = new Dictionary<Bee, Point>();
        private readonly Dictionary<Star, Point> _stars = new Dictionary<Star, Point>();
        private Random _random = new Random();
        private Size _playAreaSize;

        public BeeStarModel()
        {
            _playAreaSize = Size.Empty;
        }

        public Size PlayAreaSize
        {
            get { return _playAreaSize; }
            set
            {
                _playAreaSize = value;
                CreateBees();
                CreateStars();
            }
        }

        public void Update()
        {
            MoveOneBee();
            AddOrRemoveAStar();
        }

        private static bool RectsOverlap(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            if (r1.Width > 0 || r1.Height > 0)
            {
                return true;
            }
            return false;
        }

        private void CreateBees()
        {
            if (_playAreaSize == Size.Empty) return;
            if (_bees.Count > 0)
            {
                List<Bee> bees = _bees.Keys.ToList();
                foreach (Bee bee in bees)
                {
                    MoveOneBee(bee);
                }
            }
            else
            {
                int beeCount = _random.Next(5, 16);
                for (int i = 0; i < beeCount; i++)
                {
                    int size = _random.Next(40, 151);
                    if (TryFindNonOverlappingPoint(new Size(size, size), out Point location))
                    {
                        Bee bee = new Bee(location, new Size(size, size));
                        _bees[bee] = location;
                        OnBeeMoved(bee, location.X, location.Y);
                    }
                }
            }
        }

        private void CreateStars()
        {
            if (_playAreaSize == Size.Empty) return;
            if (_stars.Count > 0)
            {
                foreach (Star star in _stars.Keys)
                {
                    if (TryFindNonOverlappingPoint(StarSize, out Point location))
                    {
                        star.Location = location;
                        OnStarChanged(star, false);
                    }
                }
            }
            else
            {
                int starCount = _random.Next(5, 11);
                for (int i = 0; i < starCount; i++)
                {
                    CreateAStar();
                }
            }
        }

        private void CreateAStar()
        {
            if (TryFindNonOverlappingPoint(StarSize, out Point location))
            {
                Star star = new Star(location);
                _stars[star] = location;
                OnStarChanged(star, false);
            }
        }

        private bool TryFindNonOverlappingPoint(Size size, out Point point)
        {
            List<Rect> sprites = new List<Rect>();
            foreach (Bee bee in _bees.Keys) sprites.Add(new Rect(bee.Location, bee.Size));
            foreach (Star star in _stars.Keys) sprites.Add(new Rect(star.Location, StarSize));

            int step = 10;

            IEnumerable<Point> points =
                from x in Enumerable.Range(0, ((int)(_playAreaSize.Width - size.Width) / step) + 1).Select(a => a * step)
                from y in Enumerable.Range(0, ((int)(_playAreaSize.Height - size.Height) / step) + 1).Select(b => b * step)
                where !(sprites.Any(s => s.IntersectsWith(new Rect(x, y, size.Width, size.Height))))
                select new Point(x, y);

            if (points.Count() > 0)
            {
                point = points.ElementAt(_random.Next(points.Count()));
                return true;
            }
            else
            {
                point = new Point();
                return false;
            }
        }

        private void MoveOneBee(Bee bee = null)
        {
            if (_bees.Count == 0) return;
            if (bee == null) bee = _bees.ElementAt(_random.Next(_bees.Count)).Key;

            if (TryFindNonOverlappingPoint(bee.Size, out Point location))
            {
                bee.Location = location;
                _bees[bee] = location;
                OnBeeMoved(bee, location.X, location.Y);
            }
        }

        private void AddOrRemoveAStar()
        {
            if (_stars.Count < 5 || (_stars.Count < 20 && _random.Next(2) == 0))
            {
                CreateAStar();
            }
            else
            {
                Star starToRemove = _stars.ElementAt(_random.Next(_stars.Count)).Key;
                _stars.Remove(starToRemove);
                OnStarChanged(starToRemove, true);
            }
        }

        public event EventHandler<BeeMovedEventArgs> BeeMoved;
        private void OnBeeMoved(Bee beeThatMoved, double x, double y)
        {
            BeeMoved?.Invoke(this, new BeeMovedEventArgs(beeThatMoved, x, y));
        }

        public event EventHandler<StarChangedEventArgs> StarChanged;
        private void OnStarChanged(Star starThatChanged, bool removed)
        {
            StarChanged?.Invoke(this, new StarChangedEventArgs(starThatChanged, removed));
        }
    }
}
