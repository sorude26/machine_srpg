using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePoint : MonoBehaviour
{
    [SerializeField]
    private GameObject _selectMark = default; 
    public (int x, int y,float level) Pos;
    public event Action<(int, int,float)> DelSelect = default;
    private void OnMouseDown()
    {
        Debug.Log(Pos);
        OpenMark();
        DelSelect?.Invoke(Pos);
    }
    public void OpenMark()
    {
        if (_selectMark != null)
        {
            _selectMark.SetActive(true);
        }
    }
    public void CloseMark()
    {
        if (_selectMark != null)
        {
            _selectMark.SetActive(false);
        }
    }
}