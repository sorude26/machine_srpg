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
    private TerrainMeshCreator[] _stagePrefab = default;
    [SerializeField]
    private float _stageScale = 1f;
    [SerializeField]
    private int _maxSize = 15;
    [SerializeField]
    private float _scale = 0.5f;
    [SerializeField]
    PieceController _testPlayer = default;
    [SerializeField]
    Vector2Int _playerPosition = default;
    [SerializeField]
    Transform _cameraTarget = default;
    private int[] _levels = default;
    private int[] _levelsAll = default;
    private int[] _costs = default;
    private int[] _costsAll = default;
    private int _stageSizeX = default;
    private int _stageSizeY = default;
    private StagePoint[] _stagePoints = default;
    PieceController _player = default;
    public float StageScale { get => _stageScale; }
    public float Scale { get => _scale; }
    public int MaxSize { get => _maxSize; }
    public int[] Levels { get => _levels; }
    public int[] Costs { get => _costs; }
    private int StageSizeX;
    private int StageSizeY;
    public void CreateStage()
    {
        //_player = Instantiate(_testPlayer);
        _costs = new int[_maxSize * _maxSize];
        _levels = new int[_maxSize * _maxSize];
        _stagePoints = new StagePoint[_maxSize * _maxSize];
        for (int y = StageSizeY; y < StageSizeY + _maxSize; y++)
        {
            for (int x = StageSizeX; x < StageSizeX + _maxSize; x++)
            {
                var stage = Instantiate(_pointPrefab, _stageBase);
                float level = _levelsAll[x + y * _stageSizeX] * _scale;
                stage.transform.position = new Vector3(_stageScale * (x -StageSizeX), level, _stageScale * (y - StageSizeY));
                stage.Pos = new Vector2Int(x - StageSizeX, y - StageSizeY);
                _stagePoints[x - StageSizeX + (y - StageSizeY) * _maxSize] = stage;
                _costs[x - StageSizeX + (y - StageSizeY) * _maxSize] = 1 + _costsAll[x + y * _stageSizeX];
                _levels[x - StageSizeX + (y - StageSizeY) * _maxSize] = _levelsAll[x + y * _stageSizeX];
                //stage.DelSelect += _player.StartMove;
            }
        }
        Vector2Int start = new Vector2Int();
        start.x = _playerPosition.x;
        start.y = _playerPosition.y;
        //_player.StartSet(this, start);
        //_cameraTarget.position = _player.transform.position;
        //_cameraTarget.SetParent(_player.transform);
    }
    public void CreateStage(int sizeX,int sizeY, int[] levels,int[] costs)
    {
        _levelsAll = levels;
        _costsAll = costs;
        _stageSizeX = sizeX;
        _stageSizeY = sizeY;
        StageSizeX = (_stageSizeX - _maxSize) / 2;
        StageSizeY = (_stageSizeY - _maxSize) / 2;
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                var stage = Instantiate(_stagePrefab[_costsAll[x + y * sizeX]], _base);
                stage.transform.position = new Vector3(_stageScale * (x - StageSizeX), _levelsAll[x + y * sizeX] * _scale, _stageScale * (y - StageSizeY));
                float forwrd = _levelsAll[x + y * sizeX] * _scale;
                float back = forwrd;
                float left = forwrd;
                float right = forwrd;
                if (y >= 0 && y < sizeY - 1)//‰œ
                {
                    forwrd -= _levelsAll[x + (y + 1) * sizeX] * _scale;
                }
                if (y > 0 && y < sizeY)//‘O
                {
                    back -= _levelsAll[x + (y - 1) * sizeX] * _scale;
                }
                if (x > 0 && x < sizeX)//¶
                {
                    left -= _levelsAll[x - 1 + y * sizeX] * _scale;
                }
                if (x >= 0 && x < sizeX - 1)//‰E
                {
                    right -= _levelsAll[x + 1 + y * sizeX] * _scale;
                }
                stage.CreateTerrain(forwrd, back, left, right, _stageScale);
            }
        }
        MeshControl.Combine(transform);
        _base.gameObject.SetActive(false);
        CreateStage();
    }
    public StagePoint GetPoint(Vector2Int pos)
    {
        return _stagePoints[pos.x + pos.y * _maxSize];
    }
    public void Search()
    {
        //_player.Search();
    }
}
