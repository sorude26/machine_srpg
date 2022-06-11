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
    /// 戦闘継続か確認する
    /// </summary>
    /// <returns></returns>
    private bool CheckContinueBattle()
    {
        return true;
    }
    /// <summary>
    /// 次に行動する駒を返す
    /// </summary>
    /// <returns></returns>
    private PieceController NextActivityPiece()
    {
        //行動力更新
        StageManager.Instance.UpdateAllActivty();
        //行動力最高値の駒を返す
        return StageManager.Instance.AllPieces.OrderBy(p => p.Activity).FirstOrDefault();
    }
    /// <summary>
    /// 戦闘処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator Battle()
    {
        //最初に行動する駒を設定する
        TurnPiece = NextActivityPiece();
        while (_isButtleNow)
        {
            yield return TurnPiece.TurnActionSequence();
            TurnPiece = NextActivityPiece();
            _isButtleNow = CheckContinueBattle();
        }
    }
}
