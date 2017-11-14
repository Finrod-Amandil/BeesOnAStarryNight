using System.Windows;

namespace StarryNight.Model
{
    class Star
    {
        public Point Location { get; set; }
        public Star(Point location)
        {
            Location = location;
        }
    }
}
