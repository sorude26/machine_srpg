using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool _isButtleNow = false;
    public PieceController TurnPiece { get; private set; }
    public void StartBattle()
    {
        if (_isButtleNow)
        {
            return;
        }
        _isButtleNow = true;
        StartCoroutine(Battle());
    }
    /// <summary>
    /// �퓬�p�����m�F����
    /// </summary>
    /// <returns></returns>
    private bool CheckContinueBattle()
    {
        return true;
    }
    /// <summary>
    /// ���ɍs��������Ԃ�
    /// </summary>
    /// <returns></returns>
    private PieceController NextActivityPiece()
    {
        //�s���͍X�V
        StageManager.Instance.UpdateAllActivty();
        //�s���͍ō��l�̋��Ԃ�
        return StageManager.Instance.AllPieces.OrderBy(p => p.Activity).FirstOrDefault();
    }
    /// <summary>
    /// �퓬����
    /// </summary>
    /// <returns></returns>
    public IEnumerator Battle()
    {
        //�ŏ��ɍs��������ݒ肷��
        TurnPiece = NextActivityPiece();
        while (_isButtleNow)
        {
            yield return TurnPiece.TurnActionSequence();
            TurnPiece = NextActivityPiece();
            _isButtleNow = CheckContinueBattle();
        }
    }
}
