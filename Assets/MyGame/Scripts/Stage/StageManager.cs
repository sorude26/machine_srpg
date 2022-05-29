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
    public Vector3 GetStagePos((int x,int y) Pos)
    {
        return new Vector3(Pos.x * _stageScale, Levels[Pos.x + Pos.y * MaxSizeX] * _levelScale, Pos.y * _stageScale);
    }
}
