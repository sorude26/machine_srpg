using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    [SerializeField]
    private StageCreator _stageCreator = default;
    [SerializeField]
    private MapLoader _mapLoader = default;

    private List<PieceController> _stageAllPieces = default;
    private float _stageScale = 8f;
    private float _levelScale = 0.2f;
    public SearchMap StageMoveMap { get; private set; }
    public int MaxSizeX { get; private set; }
    public int MaxSizeY { get; private set; }
    public int[] Levels { get; private set; }
    public int[] Costs { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void CreateStage()
    {
        _mapLoader.LoadMap();
        _stageCreator.CreateStage(_mapLoader.MapSizeX, _mapLoader.MapSizeY, _mapLoader.LevelMap, _mapLoader.CostMap);
    }
    public Vector3 GetStagePos(Vector2Int pos)
    {
        return new Vector3(pos.x * _stageScale, Levels[pos.x + pos.y * MaxSizeX] * _levelScale, pos.y * _stageScale);
    }
}
public enum BelongType
{
    Player1,
    Player2,
    Other,
}