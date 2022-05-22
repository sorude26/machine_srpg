using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculation
{
    private struct ParabolaFormula
    {
        // Y = a * X * X + b * X + c ç°âÒÇÕcÇégÇÌÇ»Ç¢
        public float A; 
        public float B;
        public float Y;
        public ParabolaFormula(Vector2 point)
        {
            A = point.x * point.x;
            B = point.x;
            Y = point.y;
        }
    }
    public struct Parabola
    {
        public readonly float A;
        public readonly float B;
        public readonly float C;
        public Parabola(Vector2 startPos, Vector2 middlePos, Vector2 endPos)
        {
            ParabolaFormula first = new ParabolaFormula(startPos);
            ParabolaFormula middle = new ParabolaFormula(middlePos);
            ParabolaFormula last = new ParabolaFormula(endPos);
            A = GetOpenigCondition(SubFormula(first, middle), SubFormula(middle, last));
            B = GetTangentSlope(SubFormula(first, middle), SubFormula(middle, last));
            C = GetYIntercept(first, A, B);
        }
        private static ParabolaFormula SubFormula(ParabolaFormula a, ParabolaFormula b)
        {
            a.A = a.A - b.A;
            a.B = a.B - b.B;
            a.Y = a.Y - b.Y;
            return a;
        }
        private static float GetOpenigCondition(ParabolaFormula a, ParabolaFormula b)
        {
            if (a.A * b.B - b.A * a.B == 0) { return 0; }
            return a.Y * b.B - b.Y * a.B / a.A * b.B - b.A * a.B;
        }
        private static float GetTangentSlope(ParabolaFormula a, ParabolaFormula b)
        {
            if (a.B * b.A - b.B * a.A == 0) { return 0; }
            return a.Y * b.A - b.Y * a.A / a.B * b.A - b.B * a.A;
        }
        private static float GetYIntercept(ParabolaFormula formula, float a, float b)
        {
            return formula.Y - a * formula.A - b * formula.B;
        }
        /// <summary>
        /// Ç±ÇÃï˙ï®ê¸è„ÇÃXç¿ïWÇÃYÇï‘Ç∑
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float GetY(float x)
        {
            return A * x * x + B * x + C;
        }
    }
    
    
    
}
