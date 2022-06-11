using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    [SerializeField]
    private StageCreator _stageCreator = default;
    [SerializeField]
    private MapLoader _mapLoader = default;
    private List<PieceController> _stageAllPieces = default;
    private Stack<Vector2Int> _costChangePos = default;
    private float _stageScale = 8f;
    private float _levelScale = 0.2f;
    public SearchMap StageMoveMap { get; private set; }
    public SearchMap AttackMap { get; private set; }
    public List<PieceController> AllPieces { get => _stageAllPieces; }
    public int MaxSizeX { get; private set; }
    public int MaxSizeY { get; private set; }
    public int[] Levels { get; private set; }
    public int[] Costs { get; private set; }
    public bool IsCreateStage { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CreateStage();
    }
    private void CreateStage()
    {
        _mapLoader.LoadMap();
        _stageCreator.CreateStage(_mapLoader.MapSizeX, _mapLoader.MapSizeY, _mapLoader.LevelMap, _mapLoader.CostMap);
        MaxSizeX = _stageCreator.MaxSize;
        MaxSizeY = _stageCreator.MaxSize;
        Levels = _stageCreator.Levels;
        Costs = _stageCreator.Costs;
        _stageScale = _stageCreator.StageScale;
        _levelScale = _stageCreator.Scale;
        StageMoveMap = new SearchMap(MaxSizeX, MaxSizeY, Costs, Levels);
        AttackMap = new SearchMap(Levels, MaxSizeX, MaxSizeY);
        _stageAllPieces = new List<PieceController>();
        _costChangePos = new Stack<Vector2Int>();
        IsCreateStage = true;
    }
    public Vector3 GetStagePos(Vector2Int pos)
    {
        return new Vector3(pos.x * _stageScale, Levels[pos.x + pos.y * MaxSizeX] * _levelScale, pos.y * _stageScale);
    }
    public void UpdateAllActivty()
    {
        foreach (var piece in _stageAllPieces)
        {
            piece.UpdateActivity();
        }
    }
    /// <summary>
    /// ��̔z�u����
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="pos"></param>
    public void PlaceAnPiece(PieceController piece,Vector2Int pos,BelongType belong)
    {
        var setPiece = Instantiate(piece);
        setPiece.transform.position = Vector3.zero;
        setPiece.transform.rotation = Quaternion.identity;
        setPiece.transform.SetParent(transform);
        setPiece.StartSet(StageMoveMap, pos, belong);
        _stageAllPieces.Add(setPiece);
    }
    /// <summary>
    /// ��Q���̃R�X�g��z�u����
    /// </summary>
    /// <param name="pos"></param>
    public void PlaceAnObstacle(Vector2Int pos)
    {
        StageMoveMap[pos].CurrentMoveCost = SearchMap.CANNOT_MOVE_COST;
    }
    /// <summary>
    /// �ړ��O�̃R�X�g�ݒ菈��
    /// </summary>
    /// <param name="belong">��������</param>
    public void SetCost(BelongType belong)
    {
        while (_costChangePos.Count > 0)
        {
            //�ύX�ӏ��̃R�X�g�����Z�b�g����
            StageMoveMap[_costChangePos.Pop()].CurrentMoveCost = 0;
        }
        foreach (var piece in _stageAllPieces)
        {
            //�ޏꒆ�y�ѓ������͏������Ȃ�
            if (piece.State == PieceStateType.Exit || piece.Belong == belong) { continue; }
            //��̂���ꏊ�Ɉړ��s�̃R�X�g��ݒ肷��
            StageMoveMap[piece.CurrentPos].CurrentMoveCost = SearchMap.CANNOT_MOVE_COST;
            //�ύX�ӏ���ۑ�
            _costChangePos.Push(piece.CurrentPos);
        }
    }
}
public enum BelongType
{
    Player1,
    Player2,
    Other,
}