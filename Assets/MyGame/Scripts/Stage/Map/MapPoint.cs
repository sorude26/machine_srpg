using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// マップ座標
/// </summary>
public struct MapPoint
{
    public static readonly MapPoint NG_POINT = new MapPoint(-1, -1);
    public readonly int X;
    public readonly int Y;
    public MapPoint(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    #region OperatorOverride
    public static int operator +(MapPoint startPos, MapPoint pos)
    {
        return Distance(startPos, pos);
    }
    public static int operator -(MapPoint startPos, MapPoint pos)
    {
        return Distance(startPos, pos);
    }
    public static bool operator ==(MapPoint posA, MapPoint posB)
    {
        return posA.X == posB.X && posA.Y == posB.Y;
    }
    public static bool operator !=(MapPoint posA, MapPoint posB)
    {
        return posA.X != posB.X || posA.Y != posB.Y;
    }
    public override bool Equals(object obj)
    {
        if (obj is MapPoint point)
        {
            return Equals(point);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return this.X ^ this.Y;
    }
    #endregion
    #region StaticMethod
    /// <summary>
    /// 座標間の距離を返す
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static int Distance(MapPoint start, MapPoint end)
    {
        return start.X > end.X ? start.X - end.X : end.X - start.X + start.Y > end.Y ? start.Y - end.Y : end.Y - start.Y;
    }
    #endregion
    #region PublicMethod
    /// <summary>
    /// この座標と等しい場合はtrue
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool Equals(MapPoint pos)
    {
        return this.X == pos.X && this.Y == pos.Y;
    }
    /// <summary>
    /// マップの最大X座標から一元配列上での配列番号を返す
    /// </summary>
    /// <param name="maxMapSizeX"></param>
    /// <returns></returns>
    public int Index(int maxMapSizeX)
    {
        return X + Y * maxMapSizeX;
    }
    public bool IsInMap(int maxMapSizeX, int maxMapSizeY)
    {
        return X > 0 && Y > 0 && X < maxMapSizeX && Y < maxMapSizeY;
    }
    public MapPoint CheckInMapPoint(int maxMapSizeX, int maxMapSizeY, out MapPoint point)
    {
        if (IsInMap(maxMapSizeX, maxMapSizeY))
        {
            point = this;
            return point;
        }
        point = NG_POINT;
        return point;
    }
    #endregion
}