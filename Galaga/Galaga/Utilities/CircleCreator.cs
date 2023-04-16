using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Galaga.Utilities;

public static class CircleCreator {
    
    public static IEnumerable<Vector2> CreateCounterClockwiseCircle(int x, int y, int radius)
    {
        List<Vector2> circle = CreateCounterClockwiseCirclePoints(x, y, radius, 40).ToList();
        circle.Reverse();
        return circle;
    }
    
    public static IEnumerable<Vector2> CreateClockwiseCircle(int x, int y, int radius)
    {
        List<Vector2> circle =  CreateClockwiseCirclePoints(x, y, radius, 40).ToList();
        return circle;
    }
    
    public static IEnumerable<Vector2> CreateClockwiseSemiCircle(int x, int y, int radius)
    {
        List<Vector2> fullCircle = CreateClockwiseCirclePoints(x, y, radius, 40).ToList();
        fullCircle.RemoveRange(0, 20);
        return fullCircle;
    }

    public static IEnumerable<Vector2> CreateCounterClockwiseSemiCircle(int x, int y, int radius)
    {
        List<Vector2> fullCircle = CreateCounterClockwiseCirclePoints(x, y, radius, 40).ToList();
        fullCircle.RemoveRange(20, 20);
        fullCircle.Reverse();
        return fullCircle;
    }

    public static IEnumerable<Vector2> CreateSinWavePath(float amplitude, float frequency, float phaseShift, float startX, float endX, float yOffset)
    {
        return CreateSinWavePoints(amplitude, frequency, phaseShift, startX, endX, yOffset).ToList();
    }

    public static List<Vector2> RotatePath(List<Vector2> points, float radians, float rotationCenterX, float rotationCenterY)
    {
        Matrix rotationMatrix = Matrix.CreateRotationZ(radians);
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            Vector2 roatedPoint = Vector2.Transform(point - new Vector2(rotationCenterX, rotationCenterY), rotationMatrix) + new Vector2(rotationCenterX, rotationCenterY);
            points[i] = roatedPoint;
        }
        return points;

/*        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);

        return new Vector2(
            vector.X * cos - vector.Y * sin,
            vector.X * sin + vector.Y * cos
        );*/

    }

    public static float GetAngleRadians(Vector2 start, Vector2 end)
    {
        return (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
    }


    private static IEnumerable<Vector2> CreateCounterClockwiseCirclePoints(float centerX, float centerY, float radius, int numPoints)
    {
        List<Vector2> points = new();
        double angleIncrement = 2 * Math.PI / numPoints;
        
        for (int i = 0; i < numPoints; i++)
        {
            double angle = i * angleIncrement;
            float x = (float)(centerX + radius * Math.Cos(angle));
            float y = (float)(centerY + radius * Math.Sin(angle));
            points.Add(new Vector2(Convert.ToInt32(x), Convert.ToInt32(y)));
        }
    
        return points;
    }
    
    private static IEnumerable<Vector2> CreateClockwiseCirclePoints(float centerX, float centerY, float radius, int numPoints)
    {
        List<Vector2> points = new();
        double angleIncrement = 2 * Math.PI / numPoints;
        
        for (int i = 0; i < numPoints; i++)
        {
            double angle = i * angleIncrement;
            float x = (float)(centerX - radius * Math.Cos(angle));
            float y = (float)(centerY - radius * Math.Sin(angle));
            points.Add(new Vector2(Convert.ToInt32(x), Convert.ToInt32(y)));
        }
    
        return points;
    }

    private static IEnumerable<Vector2> CreateSinWavePoints(float amplitude, float frequency, float phaseShift, float startX, float endX, float yOffset)
    {
        List<Vector2> points = new();
        for (float x = startX; x <= endX; x += 2f) 
        {
            float y = (amplitude * (float)Math.Sin(frequency * (x + phaseShift))) + yOffset; // It is + yOffset since larger values make it go down
            points.Add(new Vector2(x, y));
        }

        return points;
    }

}