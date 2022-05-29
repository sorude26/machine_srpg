using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePoint : MonoBehaviour
{
    [SerializeField]
    private GameObject _selectMark = default; 
    public Vector2Int Pos;
    public event Action<Vector2Int> DelSelect = default;
    private bool _isOpen = false;
    private void OnMouseDown()
    {
        if (!_isOpen) { return; }
        Debug.Log(Pos);
        DelSelect?.Invoke(Pos);
    }
    public void OpenMark()
    {
        if (_selectMark != null)
        {
            _selectMark.SetActive(true);
            _isOpen = true;
        }
    }
    public void CloseMark()
    {
        if (_selectMark != null)
        {
            _selectMark.SetActive(false);
            _isOpen = false;
        }
    }
}