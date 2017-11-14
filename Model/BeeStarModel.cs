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

        /// <summary>
        /// Makes some changes to the play area: Makes one bee move and adds or removes one star.
        /// </summary>
        public void Update()
        {
            MoveOneBee();
            AddOrRemoveAStar();
        }

        /// <summary>
        /// DEPRECATED
        /// Checks if two rectangles overlap. Use rectangle.IntersectsWith() instead.
        /// </summary>
        /// <param name="r1">The first rectangle</param>
        /// <param name="r2">The second rectangle</param>
        /// <returns></returns>
        private static bool RectsOverlap(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            if (r1.Width > 0 || r1.Height > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the bees when the playarea resizes.
        /// Moves all of the bees, if none are available spawns a random number of new bees.
        /// </summary>
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

        /// <summary>
        /// Updates the stars when the playarea resizes.
        /// Finds a new spot for each star or spawns a random number of stars of none are available yet.
        /// </summary>
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

        /// <summary>
        /// Creates one star. Does nothing if not enough space is available for another star.
        /// </summary>
        private void CreateAStar()
        {
            if (TryFindNonOverlappingPoint(StarSize, out Point location))
            {
                Star star = new Star(location);
                _stars[star] = location;
                OnStarChanged(star, false);
            }
        }

        /// <summary>
        /// Tries to find one point, where a new sprite can be spanwed without overlapping any other sprite
        /// or extending over the playarea's borders.
        /// </summary>
        /// <param name="size">The size of the sprite to spawn.</param>
        /// <param name="point">A point where the sprite can be spawned, if available.</param>
        /// <returns>Returns true, if at least one spawnpoint is available. Returns false if there is not enough
        /// space to spawn the sprite.</returns>
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

        /// <summary>
        /// Move the specified bee, or, if none is specified a random one, to a random point where it will not overlap
        /// any other sprite. Does nothing if not enough space for the bee in question is available.
        /// </summary>
        /// <param name="bee">The bee to move.</param>
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

        /// <summary>
        /// Adds or removes a star. If there are less than 5 stars present, always adds one. If there are more than 20
        /// present, always remove one. Otherwise 50/50% chance to add or remove one.
        /// </summary>
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
