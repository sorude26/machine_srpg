using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoveController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 2f;
    private float _walkLiftingLevel = 0.3f;
    private float _jumpLiftingLevel = 2f;
    private float _currentLevel = default;
    private Vector3 _targetPos = default;
    public Stack<Vector3> MovePoints = new Stack<Vector3>();
    public bool IsMove { get; private set; }
    private bool IsMoveing()
    {
        if (transform.position  == _targetPos)
        {
            return false;
        }
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);
        return true;
    }
    public IEnumerator Move()
    {
        IsMove = true;
        _currentLevel = transform.position.y;
        _targetPos = transform.position;
        while (MovePoints.Count > 0)
        {
            _targetPos = MovePoints.Pop();
            SetAngle();
            yield return OneMove();
        }
        IsMove = false;
    }
    IEnumerator OneMove()
    {
        while (IsMoveing())
        {
            yield return null;
        }
    }
    public void Warp(Vector3 target)
    {
        transform.position = target;
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
            var a = new Calculation.Parabola(transform.position, new Vector2(p.x, _targetPos.y + 1f), (Vector2)_targetPos);
        }
    }
    void SetAngle()
    {
        if (_targetPos == transform.position)
        {
            return;
        }
        var p = _targetPos - transform.position;
        p.y = 0;
        transform.forward = p.normalized;
    }
}
