using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceMoveController))]
public class PieceController : MonoBehaviour
{
    [SerializeField]
    private int _movePower = 5;
    [SerializeField]
    private int _liftingPower = 5;
    public Transform _base = default;
    private SearchMap _searchMap = default;
    private PieceMoveController _moveController = default;
    private StageCreator _stage = default;
    public Vector2Int CurrentPos { get; protected set; }
    /// <summary>
    /// çsìÆåàíËèàóù
    /// </summary>
    /// <returns></returns>
    private IEnumerator SearchSequence()
    {
        _searchMap.MakeFootprints(CurrentPos, _movePower);
        yield return null;
    }
    /// <summary>
    /// à⁄ìÆèàóù
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator MoveSequence(Vector2Int target)
    {
        foreach (var point in _searchMap.GetRoutePoints(target))
        {
            _moveController.MovePoints.Push(StageManager.Instance.GetStagePos(point));
        }
        yield return _moveController.Move();
        CurrentPos = target;
    }
    /// <summary>
    /// êÌì¨èàóù
    /// </summary>
    /// <returns></returns>
    private IEnumerator BattleSequence()
    {
        yield return null;
    }
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
    public void StartSet(SearchMap stageMap,Vector2Int startPos)
    {
        _searchMap = stageMap;
        CurrentPos = startPos;
        _moveController.Warp(StageManager.Instance.GetStagePos(startPos));
    }
    public IEnumerator TurnActionSequence()
    {
        yield return SearchSequence();
        Vector2Int target = CurrentPos; 
        yield return MoveSequence(target);
        yield return BattleSequence();
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
