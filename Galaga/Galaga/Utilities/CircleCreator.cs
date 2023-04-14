using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Galaga.Utilities;

public static class CircleCreator {
    
    public static IEnumerable<Vector2> CreateCounterClockwiseCircle(int x, int y, int radius)
    {
        var circle = CreateCounterClockwiseCirclePoints(x, y, radius, 40).ToList();
        circle.Reverse();
        return circle;
    }
    
    public static IEnumerable<Vector2> CreateClockwiseCircle(int x, int y, int radius)
    {
        var circle =  CreateClockwiseCirclePoints(x, y, radius, 40).ToList();
        return circle;
    }
    
    public static IEnumerable<Vector2> CreateClockwiseSemiCircle(int x, int y, int radius)
    {
        var fullCircle = CreateClockwiseCirclePoints(x, y, radius, 40).ToList();
        fullCircle.RemoveRange(0, 20);
        return fullCircle;
    }

    public static IEnumerable<Vector2> CreateCounterClockwiseSemiCircle(int x, int y, int radius)
    {
        var fullCircle = CreateCounterClockwiseCirclePoints(x, y, radius, 40).ToList();
        fullCircle.RemoveRange(20, 20);
        fullCircle.Reverse();
        return fullCircle;
    }

    private static IEnumerable<Vector2> CreateCounterClockwiseCirclePoints(float centerX, float centerY, float radius, int numPoints)
    {
        var points = new List<Vector2>();
        var angleIncrement = 2 * Math.PI / numPoints;
        
        for (var i = 0; i < numPoints; i++)
        {
            var angle = i * angleIncrement;
            var x = (float)(centerX + radius * Math.Cos(angle));
            var y = (float)(centerY + radius * Math.Sin(angle));
            points.Add(new Vector2(Convert.ToInt32(x), Convert.ToInt32(y)));
        }
    
        return points;
    }
    
    private static IEnumerable<Vector2> CreateClockwiseCirclePoints(float centerX, float centerY, float radius, int numPoints)
    {
        var points = new List<Vector2>();
        var angleIncrement = 2 * Math.PI / numPoints;
        
        for (var i = 0; i < numPoints; i++)
        {
            var angle = i * angleIncrement;
            var x = (float)(centerX - radius * Math.Cos(angle));
            var y = (float)(centerY - radius * Math.Sin(angle));
            points.Add(new Vector2(Convert.ToInt32(x), Convert.ToInt32(y)));
        }
    
        return points;
    }
}