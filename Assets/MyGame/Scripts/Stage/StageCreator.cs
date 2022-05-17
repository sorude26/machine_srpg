using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    [SerializeField]
    private Transform _base = default;
    [SerializeField]
    private GameObject _stagePrefab = default;
    [SerializeField]
    private float _stageScale = 1f;
    [SerializeField]
    private int _maxSize = 15;
    [SerializeField]
    private float _level = 0.5f;
    public void CreateStage()
    {
        for (int y = 0; y < _maxSize; y++)
        {
            for (int x = 0; x < _maxSize; x++)
            {
                var stage = Instantiate(_stagePrefab, _base);
                stage.transform.position = new Vector3(_stageScale * x, Random.Range(0,_level), _stageScale * y);
            }
        }
        MeshControl.Combine(transform);
        _base.gameObject.SetActive(false);
    }
}
