using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 駒の状態
/// </summary>
public enum PieceStateType
{
    Active,
    Inactive,
    Exit,
}
[RequireComponent(typeof(PieceMoveController))]
public class PieceController : MonoBehaviour
{
    const int MOVE_ACTIVITY_COST = 10;
    const int TUNE_ACTIVITY_COST = 5;
    const int ATTACK_ACTIVITY_COST = 15;
    const int NONE_ACTIVITY_COST = -999;
    [SerializeField]
    private int _movePower = 5;
    [SerializeField]
    private int _liftingPower = 5;
    public Transform _base = default;
    private SearchMap _searchMap = default;
    private PieceMoveController _moveController = default;
    private StageCreator _stage = default;
    private PieceParameter _parameter = default;
    public Vector2Int CurrentPos { get; protected set; }
    public BelongType Belong { get; protected set; }
    public PieceStateType State { get;protected set; }
    public int Activity { get; protected set; }
    #region SequenceIEnumeratorMethods
    /// <summary>
    /// 行動決定処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator SearchSequence()
    {
        _searchMap.MakeFootprints(CurrentPos, _movePower);
        yield return null;
    }
    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator MoveSequence(Vector2Int target)
    {
        if (CurrentPos == target) { yield break; }
        foreach (var point in _searchMap.GetRoutePoints(target))
        {
            _moveController.MovePoints.Push(StageManager.Instance.GetStagePos(point));
        }
        yield return _moveController.Move();
        CurrentPos = target;
        Activity -= MOVE_ACTIVITY_COST;
    }
    /// <summary>
    /// 戦闘処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator BattleSequence()
    {
        yield return null;
        Activity -= ATTACK_ACTIVITY_COST;
    }
    /// <summary>
    /// ターン終了時処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnEndSequenc()
    {
        Activity -= TUNE_ACTIVITY_COST;
        yield return null;
    }
    /// <summary>
    /// ターン行動中のシーケンス
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnActionSequence()
    {
        yield return SearchSequence();
        Vector2Int target = CurrentPos;
        yield return MoveSequence(target);
        yield return BattleSequence();
        yield return TurnEndSequenc();
    }
    #endregion
    /// <summary>
    /// 初期化処理（仮）
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="startPos"></param>
    public void StartSet(StageCreator stage, Vector2Int startPos)
    {
        _moveController = GetComponent<PieceMoveController>();
        _searchMap = new SearchMap(stage.MaxSize, stage.MaxSize, stage.Costs, stage.Levels);
        _searchMap.LiftingPower = _liftingPower;
        CurrentPos = startPos;
        _stage = stage; 
        MeshControl.Combine(_base);
        //_moveController.Warp(StageManager.Instance.GetStagePos(startPos));
        _moveController.Warp(new Vector3(CurrentPos.x * _stage.StageScale, _stage.Levels[CurrentPos.x + CurrentPos.y * _stage.MaxSize] * _stage.Scale, CurrentPos.y  * _stage.StageScale));
    }
    /// <summary>
    /// 初期配置処理
    /// </summary>
    /// <param name="stageMap"></param>
    /// <param name="startPos"></param>
    public void StartSet(SearchMap stageMap,Vector2Int startPos)
    {
        _searchMap = stageMap;
        CurrentPos = startPos;
        _moveController.Warp(StageManager.Instance.GetStagePos(startPos));
    }
    public void UpdateActivity()
    {
        if (State == PieceStateType.Exit) { return; }
        Activity += _parameter.Activity;
    }
    public void DeadPiece()
    {
        Activity = NONE_ACTIVITY_COST;
        State = PieceStateType.Exit;
        this.gameObject.SetActive(false);
    }
    public void Search()
    {
        if (_moveController.IsMoveing)
        {
            return;
        }
        _searchMap.MakeFootprints(CurrentPos, _movePower);
        foreach (var point in _searchMap.FootprintsPoints)
        {
            _stage.GetPoint(point).OpenMark();
        }
    }
    /// <summary>
    /// 指定地点へ移動開始する
    /// </summary>
    /// <param name="target"></param>
    public void StartMove(Vector2Int target) 
    {
        if (_moveController.IsMoveing)
        {
            return;
        }
        foreach (var point in _searchMap.FootprintsPoints)
        {
            _stage.GetPoint(point).CloseMark();
        }
        foreach (var point in _searchMap.GetRoutePoints(target))
        {
            //_moveController.MovePoints.Push(StageManager.Instance.GetStagePos(point));
            _moveController.MovePoints.Push(new Vector3(point.x * _stage.StageScale, _stage.Levels[point.x + point.y * _stage.MaxSize] * _stage.Scale, point.y * _stage.StageScale));
        }
        StartCoroutine(_moveController.Move());
        CurrentPos = target;
    }
}
