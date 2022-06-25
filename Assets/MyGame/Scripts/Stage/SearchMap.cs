using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// マップ座標
/// </summary>
public struct MapPoint
{
    public int X;
    public int Y;
    public int Level;
    public MapPoint(int x,int y,int level = 0)
    {
        this.X = x;
        this.Y = y;
        this.Level = level;
    }
    #region OperatorOverride
    public static int operator +(MapPoint startPos,MapPoint pos)
    {
        return Distance(startPos, pos);
    }
    public static int operator -(MapPoint startPos, MapPoint pos)
    {
        return Distance(startPos, pos);
    }
    public static bool operator ==(MapPoint posA,MapPoint posB)
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
    #endregion
}

/// <summary>
/// 探索マップ
/// </summary>
public class SearchMap : IEnumerable<SearchMap.Point>
{
    #region Field
    #region Enum
    /// <summary>
    /// A-Star方式の探索で使用するState
    /// </summary>
    public enum SearchStateType
    {
        Idle,
        Open,
        Close,
    }
    #endregion
    #region ConstField
    /// <summary> 探索開始時の足跡値 </summary>
    private const int START_FOOTPRINTS = -1;
    /// <summary> 移動不可の座標のコスト </summary>
    public const int CANNOT_MOVE_COST = 9999;
    #endregion
    #region ReadonlyField
    /// <summary> 移動不可の座標 </summary>
    private readonly MapPoint NG_POINT = new MapPoint(-1, -1);
    /// <summary> 最大X座標 </summary>
    public readonly int MAX_X;
    /// <summary> 最大Y座標 </summary>
    public readonly int MAX_Y;
    #endregion
    #region PrivateField
    /// <summary> 探索用目標座標 </summary>
    private MapPoint _target;
    /// <summary> 目標座標までの道のり </summary>
    private Stack<Point> _route;
    #endregion
    #region PublicField
    /// <summary> 昇降能力 </summary>
    public int LiftingPower;
    #endregion
    #region Property
    /// <summary> 座標データ </summary>
    public Point[] MapData { get; }
    /// <summary> 足跡付き座標リスト </summary>
    public List<MapPoint> FootprintsPoints { get; }
    #endregion
    #region Indexer
    public Point this[int index] => MapData[index];
    public Point this[int x, int y] => MapData[x + y * MAX_X];
    public Point this[MapPoint pos] => MapData[pos.X + pos.Y * MAX_X];
    #endregion
    #region Class
    /// <summary>
    /// 探索用座標クラス
    /// </summary>
    public class Point
    {
        public readonly int X;
        public readonly int Y;
        #region PrivateField
        /// <summary> 移動に掛かるコスト </summary>
        private readonly int _moveCost;
        #endregion
        #region PublicField
        /// <summary> 座標の状況に応じて変更可能なコスト </summary>
        public int CurrentMoveCost;
        /// <summary> 足跡 </summary>
        public int Footprints;
        /// <summary> 探索用コスト </summary>
        public int Cost;
        /// <summary> 探索用距離コスト </summary>
        public int DistanceCost;
        /// <summary> 探索用合計コスト</summary>
        public int TotalCost;
        /// <summary> 探索用親座標 </summary>
        public MapPoint Parent;
        /// <summary> 探索用状態 </summary>
        public SearchStateType State;
        /// <summary> 高度 </summary>
        public int Level;
        #endregion
        #region Property
        /// <summary> 座標 </summary>
        public MapPoint Pos { get; }
        /// <summary> 移動計算時に掛かるコスト </summary>
        public int MoveCost { get => CurrentMoveCost + _moveCost; }
        #endregion
        public Point(int x = 0, int y = 0, int cost = 1)
        {
            X = x;
            Y = y;
            Pos = new MapPoint(x, y);
            _moveCost = cost;
        }
        public static explicit operator Point(MapPoint pos)
        {
            return new Point(pos.X, pos.Y);
        }
        public static explicit operator MapPoint(Point pos)
        {
            return new MapPoint(pos.X, pos.Y);
        }        
    }
    #endregion
    #region Constructor
    /// <summary>
    ///  最大座標のみ設定し生成する
    /// </summary>
    /// <param name="maxX">最大X座標</param>
    /// <param name="maxY">最大Y座標</param>
    public SearchMap(int maxX, int maxY)
    {
        MAX_X = maxX;
        MAX_Y = maxY;
        MapData = new Point[maxX * maxY];
        _route = new Stack<Point>();
        CreateMap();
    }
    /// <summary>
    /// 最大座標、地形コストを設定し生成する
    /// </summary>
    /// <param name="maxX">最大X座標</param>
    /// <param name="maxY">最大Y座標</param>
    /// <param name="costData">地形コストデータ</param>
    public SearchMap(int maxX, int maxY, int[] costData)
    {
        MAX_X = maxX;
        MAX_Y = maxY;
        MapData = new Point[maxX * maxY];
        _route = new Stack<Point>();
        CreateMap(costData);
    }
    public SearchMap(int maxX, int maxY, int[] costData, int[] levels)
    {
        MAX_X = maxX;
        MAX_Y = maxY;
        MapData = new Point[maxX * maxY];
        _route = new Stack<Point>();
        CreateMap(costData, levels);
        FootprintsPoints = new List<MapPoint>();
    }
    public SearchMap(int[] levels,int maxX,int maxY)
    {
        MAX_X = maxX;
        MAX_Y = maxY;
        MapData = new Point[maxX * maxY];
        _route = new Stack<Point>();
        CreateLevelMap(levels);
        FootprintsPoints = new List<MapPoint>();
    }
    #endregion
    #endregion
    #region PrivateMethod
    /// <summary>
    /// Mapデータを作成する
    /// </summary>
    private void CreateMap()
    {
        for (int y = 0; y < MAX_Y; y++)
        {
            for (int x = 0; x < MAX_X; x++)
            {
                MapData[x + y * MAX_X] = new Point(x, y);
            }
        }
    }
    /// <summary>
    /// コスト設定込みのMapデータを作成する
    /// </summary>
    /// <param name="costData"></param>
    private void CreateMap(int[] costData)
    {
        for (int y = 0; y < MAX_Y; y++)
        {
            for (int x = 0; x < MAX_X; x++)
            {
                MapData[x + y * MAX_X] = new Point(x, y, costData[x + y * MAX_X]);
            }
        }
    }
    private void CreateMap(int[] costData, int[] levels)
    {
        for (int y = 0; y < MAX_Y; y++)
        {
            for (int x = 0; x < MAX_X; x++)
            {
                MapData[x + y * MAX_X] = new Point(x, y, costData[x + y * MAX_X]);
                MapData[x + y * MAX_X].Level = levels[x + y * MAX_X];
            }
        }
    }
    private void CreateLevelMap(int[] levels)
    {
        for (int y = 0; y < MAX_Y; y++)
        {
            for (int x = 0; x < MAX_X; x++)
            {
                MapData[x + y * MAX_X] = new Point(x, y);
                MapData[x + y * MAX_X].Level = levels[x + y * MAX_X];
            }
        }
    }
    /// <summary>
    /// 指定地点の周囲に足跡を付ける
    /// </summary>
    /// <param name="point"></param>
    /// <param name="movePower"></param>
    private void MakeFootprintsNeighor(Point point, int movePower)
    {
        foreach (var neighorPoint in GetNeighorMap(point))
        {
            //高低差が昇降力を超える時は足跡を付けない
            if (Difference(point.Level, neighorPoint.Level) > LiftingPower) { continue; }
            MakeFootprintsPoint(neighorPoint, movePower, point.Pos);
        }
    }
    /// <summary>
    /// 指定地点に足跡を付ける
    /// </summary>
    /// <param name="point"></param>
    /// <param name="movePower"></param>
    private void MakeFootprintsPoint(Point point, int movePower, MapPoint parent)
    {
        //指定地点への移動コストを引いた移動力がその座標の足跡より高い時のみ足跡を付ける
        if (movePower - point.MoveCost <= point.Footprints) { return; }
        //足跡を付ける
        point.Footprints = movePower - point.MoveCost;
        //一つ前の座標を登録
        point.Parent = parent;
        FootprintsPoints.Add(point.Pos);
        //足跡0以下で終了
        if (point.Footprints <= 0) { return; }
        //再度周囲に足跡を付ける
        MakeFootprintsNeighor(point, point.Footprints);
    }
    /// <summary>
    /// 指定座標の周囲に目標座標があるか再帰的に確認する
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private bool CheckNeighor(MapPoint start)
    {
        foreach (var neigher in GetNeighorMap(this[start]))
        {
            //目標座標があればtrue
            if (CheckPoint(neigher, this[start])) { return true; }
        }
        //次の確認座標がなければ終了
        var next = GetMinimumCostOpenPoint();
        if (next == NG_POINT) { return false; }
        //次の座標に移る前に閉じる
        this[start].State = SearchStateType.Close;
        return CheckNeighor(next);
    }
    /// <summary>
    /// 指定座標が目標か確認する
    /// </summary>
    /// <param name="point"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private bool CheckPoint(Point point, Point parent)
    {
        //高低差が昇降力を超える時は侵入不可
        if (Difference(point.Level, parent.Level) > LiftingPower) { return false; }
        //目標地点であれば終了
        if (point.Pos == _target)
        {
            point.Parent = parent.Pos;
            _route.Push(point);
            //最短経路に足跡を付ける
            return SaveRoutePoint(parent);
        }
        //座標が侵入不可か確認
        if (point.MoveCost >= CANNOT_MOVE_COST) { return false; }
        //待機状態であればOpenする
        if (point.State == SearchStateType.Idle)
        {
            point.State = SearchStateType.Open;
            //探索座標の評価を付ける
            point.DistanceCost = Difference(point.X, _target.X) + Difference(point.Y, _target.Y);//単純な距離
            point.Cost = parent.Cost + point.MoveCost;//移動コスト計算
            point.TotalCost = point.DistanceCost + point.Cost;//座標の評価
            point.Parent = parent.Pos;
        }
        return false;
    }
    /// <summary>
    /// StateがOpenのCost最小の座標を返す
    /// </summary>
    /// <returns></returns>
    private MapPoint GetMinimumCostOpenPoint()
    {
        MapPoint pos = NG_POINT;
        int score = 0;
        int cost = CANNOT_MOVE_COST;
        foreach (var point in MapData)
        {
            if (point.State != SearchStateType.Open) //Openの座標のみ評価を行う
            {
                continue;
            }
            if (point.TotalCost < cost) //より総コストが低い座標を保持する
            {
                cost = point.TotalCost;
                pos = point.Pos;
                score = point.Cost;
            }
            else if (point.TotalCost == cost && point.Cost < score) //総コストが同率であればよりコストが低い座標を保持する
            {
                pos = point.Pos;
                score = point.Cost;
            }
        }
        return pos;
    }
    /// <summary>
    /// ゴール地点までの道を記録する
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool SaveRoutePoint(Point point)
    {
        //道のりを保存
        _route.Push(point);
        //開始地点であれば終了
        if (point.Parent == NG_POINT) { return true; }
        return SaveRoutePoint(this[point.Parent]);
    }
    /// <summary>
    /// 差の値を返す
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int Difference(int x, int y) => x > y ? x - y : y - x;
    #endregion
    #region PublicMethod
    /// <summary>
    /// 二点のマスの距離を返す
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static int Distance(MapPoint start, MapPoint end)
    {
        return start.X > end.X ? start.X - end.X : end.X - start.X + start.Y > end.Y ? start.Y - end.Y : end.Y - start.Y;
    }
    /// <summary>
    /// ダイクストラ法の範囲検索で足跡を付ける
    /// </summary>
    /// <param name="startPoint">開始地点</param>
    /// <param name="movePower">開始時の行動力</param>
    public void MakeFootprints(MapPoint startPoint, int movePower)
    {
        //足跡の初期化
        foreach (var point in MapData)
        {
            point.Footprints = START_FOOTPRINTS;
            point.Parent = NG_POINT;
        }
        FootprintsPoints.Clear();
        this[startPoint].Footprints = movePower;
        FootprintsPoints.Add(startPoint);
        MakeFootprintsNeighor(this[startPoint], movePower);
    }
    /// <summary>
    /// AStar法の最短経路検索を行い足跡を付ける
    /// </summary>
    /// <param name="start">開始地点</param>
    /// <param name="goal">目標地点</param>
    /// <returns>到達可能ならtrue</returns>
    public bool SearchShortestPath(MapPoint start, MapPoint goal, int movePower)
    {
        //探索データの初期化
        foreach (var point in MapData)
        {
            point.State = SearchStateType.Idle;
            point.TotalCost = 0;
            point.Cost = 0;
            point.Footprints = START_FOOTPRINTS;
            point.Parent = NG_POINT;
        }
        //探索開始
        this[start].State = SearchStateType.Close;
        _target = goal;
        if (CheckNeighor(start))
        {
            //目標地点に到達可能であれば道のりにそって足跡を付ける
            while (_route.Count > 0)
            {
                var point = _route.Pop();
                point.Footprints = movePower;
                movePower -= point.MoveCost;
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 指定座標の周囲の座標を返す
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public IEnumerable<Point> GetNeighorMap(Point point)
    {
        if (point.Y > 0 && point.Y < MAX_Y)//上
        {
            yield return this[point.X, point.Y - 1];
        }
        if (point.Y >= 0 && point.Y < MAX_Y - 1)//下
        {
            yield return this[point.X, point.Y + 1];
        }
        if (point.X > 0 && point.X < MAX_X)//右
        {
            yield return this[point.X - 1, point.Y];
        }
        if (point.X >= 0 && point.X < MAX_X - 1)//左
        {
            yield return this[point.X + 1, point.Y];
        }
    }
    /// <summary>
    /// 指定地点までの道のりを返す
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    public IEnumerable<MapPoint> GetRoutePoints(MapPoint targetPoint)
    {
        while (true)
        {
            yield return this[targetPoint].Pos;
            if (this[targetPoint].Parent == NG_POINT) { break; }
            targetPoint = this[targetPoint].Parent;
        }
    }
    #region IEnumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
        return MapData.GetEnumerator();
    }

    public IEnumerator<Point> GetEnumerator()
    {
        int count = 0;
        while (count < MapData.Length)
        {
            yield return MapData[count];
            count++;
        }
    }   
    #endregion
}
#endregion
