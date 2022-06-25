using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoveController : MonoBehaviour
{
    private const float TARGET_DISTANCE = 0.1f;
    [SerializeField]
    private float _moveSpeed = 2f;
    private float _walkLiftingLevel = 0.3f;
    private float _jumpLiftingLevel = 2f;
    private float _currentLevel = default;
    private Vector3 _targetPos = default;
    /// <summary>
    /// 移動経路地点
    /// </summary>
    public Stack<Vector3> MovePoints = new Stack<Vector3>();
    /// <summary>
    /// 移動中フラグ
    /// </summary>
    public bool IsMoveing { get; private set; }

    /// <summary>
    /// 目標へ向けての移動処理、目標地点到達までtrueを返す
    /// </summary>
    /// <returns></returns>
    private bool IsMoveingForTarget()
    {
        if (Vector3.Distance(transform.position, _targetPos) < TARGET_DISTANCE) { return false; }
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);
        return true;
    }

    /// <summary>
    /// 目標地点の方向を向く
    /// </summary>
    private void LookTarget()
    {
        if (Vector3.Distance(transform.position, _targetPos) < TARGET_DISTANCE) { return; }
        var targetDir = _targetPos - transform.position;
        targetDir.y = 0;
        transform.forward = targetDir;
    }
    /// <summary>
    /// 一マス分の移動を待つ
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitOneMove()
    {
        while (IsMoveingForTarget())
        {
            yield return null;
        }
    }
    /// <summary>
    /// 移動処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator Move()
    {
        IsMoveing = true;
        _currentLevel = transform.position.y;
        _targetPos = transform.position;
        while (MovePoints.Count > 0)//移動目標が残っている間移動する
        {
            _targetPos = MovePoints.Pop();
            LookTarget();
            yield return WaitOneMove();
        }
        IsMoveing = false;
    }
    /// <summary>
    /// 瞬間移動する
    /// </summary>
    /// <param name="target"></param>
    public void Warp(Vector3 target)
    {
        transform.position = target;
        IsMoveing = false;
    }
    void MoveTest()
    {
        var p = _targetPos - transform.position;
        p.y = 0;
        transform.forward = p.normalized;
        if (Mathf.Abs(_currentLevel - _targetPos.y) <= _walkLiftingLevel)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);
        }
        else if (Mathf.Abs(_currentLevel - _targetPos.y) <= _jumpLiftingLevel)
        {
            var a = new Calculation.Parabola(transform.position, new Vector2(p.x, _targetPos.y + 1f), _targetPos);
        }
    }
    
}
