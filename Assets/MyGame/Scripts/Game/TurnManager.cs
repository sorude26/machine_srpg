using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public const float BASE_ACTIVITY = 1f;
    public const float MOVE_ACTIVITY = 2f;
    public const float ATTACK_ACTIVITY = 3f;

    private bool _isButtleNow = false;
    private bool CheckBattleEnd()
    {
        return true;
    }
    /// <summary>
    /// ��̏��ʂ����肷��
    /// </summary>
    private void SortPieces()
    {

    }
    public IEnumerator Battle()
    {
        PieceController turnPiece = default;
        while (_isButtleNow)
        {
            yield return turnPiece.TurnActionSequence();
            _isButtleNow = CheckBattleEnd();
            SortPieces();
            turnPiece = null;//���ɓ������ݒ肷��
            if (turnPiece == null)
            {
                break;
            }
        }
    }
}
