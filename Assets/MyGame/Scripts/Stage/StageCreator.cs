using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    [SerializeField]
    private Transform _base = default;
    [SerializeField]
    private StagePoint _stagePrefab = default;
    [SerializeField]
    private float _stageScale = 1f;
    [SerializeField]
    private int _maxSize = 15;
    [SerializeField]
    private int _level = 2;
    [SerializeField]
    private float _scale = 0.5f;
    public void CreateStage()
    {
        for (int y = 0; y < _maxSize; y++)
        {
            for (int x = 0; x < _maxSize; x++)
            {
                var stage = Instantiate(_stagePrefab, _base);
                float level = Random.Range(0, _level) * _scale;
                stage.transform.position = new Vector3(_stageScale * x, level, _stageScale * y);
                stage.Pos = (x, y, level);
            }
        }
        MeshControl.Combine(transform);
    }
}
