using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �T���}�b�v
/// </summary>
public class SearchMap : IEnumerable<SearchMap.Point>
{
    #region Field
    #region Enme
    /// <summary>
    /// A-Star�����̒T���Ŏg�p����State
    /// </summary>
    public enum SearchStateType
    {
        Idle,
        Open,
        Close,
    }
    #endregion
    #region ConstField
    /// <summary> �T���J�n���̑��Ւl </summary>
    private const int START_FOOTPRINTS = -1;
    /// <summary> �ړ��s�̍��W�̃R�X�g </summary>
    public const int CANNOT_MOVE_COST = 9999;
    /// <summary> �ړ��s�̍��W </summary>
    private readonly (int, int) NG_POINT = (-1, -1);
    /// <summary> �ő�X���W </summary>
    public readonly int MAX_X;
    /// <summary> �ő�Y���W </summary>
    public readonly int MAX_Y;
    #endregion
    #region PrivateField
    /// <summary> �T���p�ڕW���W </summary>
    private (int x, int y) _target;
    /// <summary> �ڕW���W�܂ł̓��̂� </summary>
    private Stack<Point> _route;
    #endregion
    #region PublicField
    /// <summary> ���~�\�� </summary>
    public int LiftingPower;
    #endregion
    #region Property
    /// <summary> ���W�f�[�^ </summary>
    public Point[] MapData { get; }
    #endregion
    #region Indexer
    public Point this[int index] => MapData[index];
    public Point this[int x, int y] => MapData[x + y * MAX_X];
    public Point this[(int x, int y) pos] => MapData[pos.x + pos.y * MAX_X];
    #endregion
    #region Class
    /// <summary>
    /// �T���p���W�N���X
    /// </summary>
    public class Point
    {
        public readonly int X;
        public readonly int Y;
        #region PrivateField
        /// <summary> �ړ��Ɋ|����R�X�g </summary>
        private int _moveCost;
        #endregion
        #region PublicField
        /// <summary> ���W�̏󋵂ɉ����ĕύX�\�ȃR�X�g </summary>
        public int CurrentMoveCost;
        /// <summary> ���� </summary>
        public int Footprints;
        /// <summary> �T���p�R�X�g </summary>
        public int Cost;
        /// <summary> �T���p�����R�X�g </summary>
        public int DistanceCost;
        /// <summary> �T���p���v�R�X�g</summary>
        public int TotalCost;
        /// <summary> �T���p�e���W </summary>
        public (int x, int y) Parent;
        /// <summary> �T���p��� </summary>
        public SearchStateType State;
        /// <summary> ���x </summary>
        public int Level;
        #endregion
        #region Property
        /// <summary> ���W </summary>
        public (int x, int y) Pos { get => (X, Y); }
        /// <summary> �ړ��v�Z���Ɋ|����R�X�g </summary>
        public int MoveCost { get => CurrentMoveCost + _moveCost; }
        #endregion
        public Point(int x = 0, int y = 0, int cost = 1)
        {
            X = x;
            Y = y;
            _moveCost = cost;
        }
    }
    #endregion
    #region Constructor
    /// <summary>
    ///  �ő���W�̂ݐݒ肵��������
    /// </summary>
    /// <param name="maxX">�ő�X���W</param>
    /// <param name="maxY">�ő�Y���W</param>
    public SearchMap(int maxX, int maxY)
    {
        MAX_X = maxX;
        MAX_Y = maxY;
        MapData = new Point[maxX * maxY];
        _route = new Stack<Point>();
        CreateMap();
    }
    /// <summary>
    /// �ő���W�A�n�`�R�X�g��ݒ肵��������
    /// </summary>
    /// <param name="maxX">�ő�X���W</param>
    /// <param name="maxY">�ő�Y���W</param>
    /// <param name="costData">�n�`�R�X�g�f�[�^</param>
    public SearchMap(int maxX, int maxY, int[] costData)
    {
        MAX_X = maxX;
        MAX_Y = maxY;
        MapData = new Point[maxX * maxY];
        _route = new Stack<Point>();
        CreateMap(costData);
    }
    #endregion
    #endregion
    #region PrivateMethod
    /// <summary>
    /// Map�f�[�^���쐬����
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
    /// �R�X�g�ݒ荞�݂�Map�f�[�^���쐬����
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
    /// <summary>
    /// �w��n�_�̎��͂ɑ��Ղ�t����
    /// </summary>
    /// <param name="point"></param>
    /// <param name="movePower"></param>
    private void MakeFootprintsNeighor(Point point, int movePower)
    {
        foreach (var neighorPoint in GetNeighorMap(point))
        {
            //���፷�����~�͂𒴂��鎞�͑��Ղ�t���Ȃ�
            if (Difference(point.Level, neighorPoint.Level) > LiftingPower) { continue; }
            MakeFootprintsPoint(neighorPoint, movePower);
        }
    }
    /// <summary>
    /// �w��n�_�ɑ��Ղ�t����
    /// </summary>
    /// <param name="point"></param>
    /// <param name="movePower"></param>
    private void MakeFootprintsPoint(Point point, int movePower)
    {
        //�w��n�_�ւ̈ړ��R�X�g���������ړ��͂����̍��W�̑��Ղ�荂�����̂ݑ��Ղ�t����
        if (movePower - point.MoveCost <= point.Footprints) { return; }
        //���Ղ�t����
        point.Footprints = movePower - point.MoveCost;
        //����0�ȉ��ŏI��
        if (point.Footprints <= 0) { return; }
        //�ēx���͂ɑ��Ղ�t����
        MakeFootprintsNeighor(point, point.Footprints);
    }
    /// <summary>
    /// �w����W�̎��͂ɖڕW���W�����邩�ċA�I�Ɋm�F����
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private bool CheckNeighor((int x, int y) start)
    {
        foreach (var neigher in GetNeighorMap(this[start]))
        {
            //�ڕW���W�������true
            if (CheckPoint(neigher, this[start])) { return true; }
        }
        //���̊m�F���W���Ȃ���ΏI��
        var next = GetMinimumCostOpenPoint();
        if (next == NG_POINT) { return false; }
        //���̍��W�Ɉڂ�O�ɕ���
        this[start].State = SearchStateType.Close;
        return CheckNeighor(next);
    }
    /// <summary>
    /// �w����W���ڕW���m�F����
    /// </summary>
    /// <param name="point"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private bool CheckPoint(Point point, Point parent)
    {
        //���፷�����~�͂𒴂��鎞�͐N���s��
        if (Difference(point.Level, parent.Level) > LiftingPower) { return false; }
        //�ڕW�n�_�ł���ΏI��
        if (point.Pos == _target)
        {
            point.Parent = parent.Pos;
            _route.Push(point);
            //�ŒZ�o�H�ɑ��Ղ�t����
            return SaveRoutePoint(parent);
        }
        //���W���N���s���m�F
        if (point.MoveCost >= CANNOT_MOVE_COST) { return false; }
        //�ҋ@��Ԃł����Open����
        if (point.State == SearchStateType.Idle)
        {
            point.State = SearchStateType.Open;
            //�T�����W�̕]����t����
            point.DistanceCost = Difference(point.X, _target.x) + Difference(point.Y, _target.y);//�P���ȋ���
            point.Cost = parent.Cost + point.MoveCost;//�ړ��R�X�g�v�Z
            point.TotalCost = point.DistanceCost + point.Cost;//���W�̕]��
            point.Parent = parent.Pos;
        }
        return false;
    }
    /// <summary>
    /// State��Open��Cost�ŏ��̍��W��Ԃ�
    /// </summary>
    /// <returns></returns>
    private (int, int) GetMinimumCostOpenPoint()
    {
        (int, int) pos = NG_POINT;
        int score = 0;
        int cost = CANNOT_MOVE_COST;
        foreach (var point in MapData)
        {
            if (point.State != SearchStateType.Open) //Open�̍��W�̂ݕ]�����s��
            {
                continue;
            }
            if (point.TotalCost < cost) //��葍�R�X�g���Ⴂ���W��ێ�����
            {
                cost = point.TotalCost;
                pos = point.Pos;
                score = point.Cost;
            }
            else if (point.TotalCost == cost && point.Cost < score) //���R�X�g�������ł���΂��R�X�g���Ⴂ���W��ێ�����
            {
                pos = point.Pos;
                score = point.Cost;
            }
        }
        return pos;
    }
    /// <summary>
    /// �S�[���n�_�܂ł̓����L�^����
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool SaveRoutePoint(Point point)
    {
        //���̂��ۑ�
        _route.Push(point);
        //�J�n�n�_�ł���ΏI��
        if (point.Parent == NG_POINT) { return true; }
        return SaveRoutePoint(this[point.Parent]);
    }
    /// <summary>
    /// ���̒l��Ԃ�
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int Difference(int x, int y) => x > y? x - y : y - x;
    #endregion
    #region PublicMethod
    /// <summary>
    /// �_�C�N�X�g���@�͈̔͌����ő��Ղ�t����
    /// </summary>
    /// <param name="startPoint">�J�n�n�_</param>
    /// <param name="movePower">�J�n���̍s����</param>
    public void MakeFootprints((int x, int y) startPoint, int movePower)
    {
        //���Ղ̏�����
        foreach (var mapPoint in MapData)
        {
            mapPoint.Footprints = START_FOOTPRINTS;
        }
        this[startPoint].Footprints = movePower;
        MakeFootprintsNeighor(this[startPoint], movePower);
    }
    /// <summary>
    /// AStar�@�̍ŒZ�o�H�������s�����Ղ�t����
    /// </summary>
    /// <param name="start">�J�n�n�_</param>
    /// <param name="goal">�ڕW�n�_</param>
    /// <returns>���B�\�Ȃ�true</returns>
    public bool SearchShortestPath((int x, int y) start, (int x, int y) goal, int movePower)
    {
        //�T���f�[�^�̏�����
        foreach (var point in MapData)
        {
            point.State = SearchStateType.Idle;
            point.TotalCost = 0;
            point.Cost = 0;
            point.Footprints = START_FOOTPRINTS;
            point.Parent = NG_POINT;
        }
        //�T���J�n
        this[start].State = SearchStateType.Close;
        _target = goal;
        if (CheckNeighor(start))
        {
            //�ڕW�n�_�ɓ��B�\�ł���Γ��̂�ɂ����đ��Ղ�t����
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
    /// �w����W�̎��͂̍��W��Ԃ�
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public IEnumerable<Point> GetNeighorMap(Point point)
    {
        if (point.Y > 0 && point.Y < MAX_Y)//��
        {
            yield return this[point.X, point.Y - 1];
        }
        if (point.Y >= 0 && point.Y < MAX_Y - 1)//��
        {
            yield return this[point.X, point.Y + 1];
        }
        if (point.X > 0 && point.X < MAX_X)//�E
        {
            yield return this[point.X - 1, point.Y];
        }
        if (point.X >= 0 && point.X < MAX_X - 1)//��
        {
            yield return this[point.X + 1, point.Y];
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
