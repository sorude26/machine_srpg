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
    /// 駒の配置処理
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
    /// 障害物のコストを配置する
    /// </summary>
    /// <param name="pos"></param>
    public void PlaceAnObstacle(Vector2Int pos)
    {
        StageMoveMap[pos].CurrentMoveCost = SearchMap.CANNOT_MOVE_COST;
    }
    /// <summary>
    /// 移動前のコスト設定処理
    /// </summary>
    /// <param name="belong">所属勢力</param>
    public void SetCost(BelongType belong)
    {
        while (_costChangePos.Count > 0)
        {
            //変更箇所のコストをリセットする
            StageMoveMap[_costChangePos.Pop()].CurrentMoveCost = 0;
        }
        foreach (var piece in _stageAllPieces)
        {
            //退場中及び同所属は処理しない
            if (piece.State == PieceStateType.Exit || piece.Belong == belong) { continue; }
            //駒のいる場所に移動不可のコストを設定する
            StageMoveMap[piece.CurrentPos].CurrentMoveCost = SearchMap.CANNOT_MOVE_COST;
            //変更箇所を保存
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