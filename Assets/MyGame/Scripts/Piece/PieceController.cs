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
    private SearchMap _searchMap = default;
    private PieceMoveController _moveController = default;
    private StageCreator _stage = default;
    private (int x, int y) _currentPos = default;
    public void StartSet(StageCreator stage,(int x,int y) startPos)
    {
        _moveController = GetComponent<PieceMoveController>();
        _searchMap = new SearchMap(stage.MaxSize, stage.MaxSize, stage.Costs, stage.Levels);
        _searchMap.LiftingPower = _liftingPower;
        _currentPos = startPos;
        _stage = stage;
        _moveController.Warp(new Vector3((_currentPos.x + _stage.StageSizeX) * _stage.StageScale, _stage.Levels[_currentPos.x + _currentPos.y * _stage.MaxSize] * _stage.Scale, (_currentPos.y + _stage.StageSizeY) * _stage.StageScale));
    }
    public void Search()
    {
        if (_moveController.IsMove)
        {
            return;
        }
        _searchMap.MakeFootprints(_currentPos, _movePower);
        foreach (var point in _searchMap.FootprintsPoints)
        {
            _stage.GetPoint(point).OpenMark();
        }
    }
    public void StartMove((int x,int y) target) 
    {
        if (_moveController.IsMove)
        {
            return;
        }
        foreach (var point in _searchMap.FootprintsPoints)
        {
            _stage.GetPoint(point).CloseMark();
        }
        foreach (var point in _searchMap.GetRoutePoints(target))
        {
            _moveController.MovePoints.Push(new Vector3((point.x + _stage.StageSizeX) * _stage.StageScale, _stage.Levels[point.x + point.y * _stage.MaxSize] * _stage.Scale, (point.y + _stage.StageSizeY) * _stage.StageScale));
        }
        StartCoroutine(_moveController.Move());
        _currentPos = target;
    }
}
