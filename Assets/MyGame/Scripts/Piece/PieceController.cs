using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��̏��
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
    [SerializeField]
    private int _activity = 6;
    [SerializeField]
    private PieceBrain _brain = default;
    public Transform _base = default;
    private SearchMap _searchMap = default;
    private PieceMoveController _moveController = default;
    private StageCreator _stage = default;
    private PieceParameter _parameter = default;
    private MapPoint _targetPos = default;
    public MapPoint CurrentPos { get; protected set; }
    public BelongType Belong { get; protected set; }
    public PieceStateType State { get;protected set; }
    public int Activity { get; protected set; }
    #region SequenceIEnumeratorMethods
    /// <summary>
    /// �s�����菈��
    /// </summary>
    /// <returns></returns>
    private IEnumerator SearchSequence()
    {
        StageManager.Instance.SetCost(Belong);
        _searchMap.LiftingPower = _liftingPower;
        _searchMap.MakeFootprints(CurrentPos, _movePower);
        yield return null;
        _targetPos = _brain.GetMoveTarget(_searchMap);
    }
    /// <summary>
    /// �ڕW�n�_�ւ̈ړ������V�[�P���X
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveSequence()
    {
        //���ݒn�ƖڕW�������ꍇ�͏I������
        if (CurrentPos == _targetPos) { yield break; }
        foreach (var point in _searchMap.GetRoutePoints(_targetPos))
        {
            //�ړ����[�g��ݒ肷��
            _moveController.MovePoints.Push(StageManager.Instance.GetStagePos(point));
        }
        yield return _moveController.Move();
        CurrentPos = _targetPos;
        Activity -= MOVE_ACTIVITY_COST;
    }
    /// <summary>
    /// �퓬����
    /// </summary>
    /// <returns></returns>
    private IEnumerator BattleSequence()
    {
        yield return null;
        Activity -= ATTACK_ACTIVITY_COST;
    }
    /// <summary>
    /// �^�[���I��������
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnEndSequenc()
    {
        Activity -= TUNE_ACTIVITY_COST;
        yield return null;
    }
    /// <summary>
    /// �^�[���s�����̃V�[�P���X
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnActionSequence()
    {
        yield return SearchSequence();
        yield return MoveSequence();
        yield return BattleSequence();
        yield return TurnEndSequenc();
    }
    #endregion
    /// <summary>
    /// �����z�u����
    /// </summary>
    /// <param name="stageMap"></param>
    /// <param name="startPos"></param>
    public void StartSet(SearchMap stageMap,MapPoint startPos,BelongType belong)
    {
        _moveController = GetComponent<PieceMoveController>();
        _searchMap = stageMap;
        CurrentPos = startPos;
        _moveController.Warp(StageManager.Instance.GetStagePos(startPos));
        Belong = belong;
    }
    /// <summary>
    /// �s���͂��X�V����
    /// </summary>
    public void UpdateActivity()
    {
        if (State == PieceStateType.Exit) { return; }
        Activity += _activity;
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
        _searchMap.LiftingPower = _liftingPower;
        _searchMap.MakeFootprints(CurrentPos, _movePower);
        foreach (var point in _searchMap.FootprintsPoints)
        {
            _stage.GetPoint(point).OpenMark();
        }
    }
    /// <summary>
    /// �w��n�_�ֈړ��J�n����
    /// </summary>
    /// <param name="target"></param>
    public void StartMove(MapPoint target) 
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
            _moveController.MovePoints.Push(StageManager.Instance.GetStagePos(point));
        }
        StartCoroutine(_moveController.Move());
        CurrentPos = target;
    }
}
