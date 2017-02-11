using System.Collections.Generic;

namespace DancingCube
{

    public class Plane
    {
        public List<Point> Points = new List<Point>();

        public Plane(int width, int height)
        {
            //Points = new Point[width,height];

            for (var x = 0; x <= width; x++)
                for (var y = 0; y <= height; y++)
                    //Points[x, y] = new Point(x, y);
                    Points.Add(new Point(x, y));
        }
        
    }

    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}