using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject _stagePrefab = default;
    [SerializeField]
    private float _stageScale = 1f;
    [SerializeField]
    private int _maxSize = 15;
    public void CreateStage()
    {
        for (int y = 0; y < _maxSize; y++)
        {
            for (int x = 0; x < _maxSize; x++)
            {
                var stage = Instantiate(_stagePrefab, transform);
                stage.transform.position = new Vector3(_stageScale * x, 0, _stageScale * y);
            }
        }

    }
}
