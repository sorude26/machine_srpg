using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePoint : MonoBehaviour
{
    public (int x, int y,float level) Pos;
    public event Action<(int, int,float)> DelSelect = default;
    private void OnMouseDown()
    {
        Debug.Log(Pos);
        DelSelect?.Invoke(Pos);
    }
}