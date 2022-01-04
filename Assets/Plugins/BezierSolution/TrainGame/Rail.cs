using UnityEngine;
using System.Collections.Generic;

namespace BezierSolution
{
    public class Rail
    {
        public class Tile
        {
            public string type;
            public BezierSpline bezierSpline;
        }

        public class Segment
        {
            public List<Rail.Tile> tile;
        }
        
        //public List<Rail.Tile> tile = new();
        public static List<Rail.Segment> segment = new List<Rail.Segment>();

        public static GameObject currentA;
        public static GameObject currentB;
        public static bool duo;
        public static BezierSpline GetRail(int indexSegment, int indexTile)
        {
            return segment[indexSegment].tile[indexTile].bezierSpline;
        }

    }

    
    
}