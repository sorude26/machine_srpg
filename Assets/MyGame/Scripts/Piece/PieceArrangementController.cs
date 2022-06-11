using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceArrangementController : MonoBehaviour
{
    [SerializeField]
    PieceController _testPiece = default;
    [SerializeField]
    Vector2Int _startPosition = Vector2Int.zero;
    [SerializeField]
    BelongType _belong = BelongType.Player1;
    List<Vector2Int> _deployPositions = default;
    private void Start()
    {
        _deployPositions = new List<Vector2Int>();
    }
    public void DeployPiece()
    {
        if (_deployPositions.Contains(_startPosition))
        {
            return;
        }
        StageManager.Instance.PlaceAnPiece(_testPiece, _startPosition, _belong);
        _deployPositions.Add(_startPosition);
    }
}
