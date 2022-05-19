using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    [SerializeField]
    private Transform _base = default;
    [SerializeField]
    private Transform _stageBase = default;
    [SerializeField]
    private StagePoint _pointPrefab = default;
    [SerializeField]
    private GameObject _stagePrefab = default;
    [SerializeField]
    private float _stageScale = 1f;
    [SerializeField]
    private int _maxSize = 15;
    [SerializeField]
    private int _level = 2;
    [SerializeField]
    private float _scale = 0.5f;
    private int[] _levels = default;
    private int _stageSizeX = default;
    private int _stageSizeY = default;
    public void CreateStage()
    {
        int startX = (_stageSizeX - _maxSize) / 2;
        int startY = (_stageSizeY - _maxSize) / 2;
        for (int y = startY; y < startY + _maxSize; y++)
        {
            for (int x = startX; x < startX + _maxSize; x++)
            {
                var stage = Instantiate(_pointPrefab, _stageBase);
                float level = _levels[x + y * _stageSizeX] * _scale; ;
                stage.transform.position = new Vector3(_stageScale * x, level, _stageScale * y);
                stage.Pos = (x - startX, y - startY, level);
            }
        }
        //MeshControl.Combine(transform);
    }
    public void CreateStage(int sizeX,int sizeY, int[] levels)
    {
        _levels = levels;
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                var stage = Instantiate(_stagePrefab, _base);
                float level = _levels[x + y * sizeX] * _scale;
                stage.transform.position = new Vector3(_stageScale * x, level, _stageScale * y);
            }
        }
        _stageSizeX = sizeX;
        _stageSizeY = sizeY;
        MeshControl.Combine(transform);
        _base.gameObject.SetActive(false);
        CreateStage();
    }
}
