using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    private const uint COLOR_MAX = 255;
    [SerializeField]
    private Sprite _mapSprite = default;
    private int _mapSizeX = default;
    private int _mapSizeY = default;
    private int[] _levelMap = default;
    private int[] _costMap = default;

    [SerializeField]
    StageCreator _stageCreater = default;
    private void Start()
    {
        LoadMap();
        _stageCreater.CreateStage(_mapSizeX,_mapSizeY, _levelMap);
    }
    private void LoadMap()
    {
        _mapSizeX = (int)_mapSprite.rect.width;
        _mapSizeY = (int)_mapSprite.rect.height;
        _levelMap = new int[_mapSizeY * _mapSizeY];
        _costMap = new int[_mapSizeY * _mapSizeY];
        Color[] mapData = _mapSprite.texture.GetPixels();
        for (int y = 0; y < _mapSizeY; y++)
        {
            for (int x = 0; x < _mapSizeX; x++)
            {
                _levelMap[x + y * _mapSizeX] = (int)COLOR_MAX - (int)(mapData[x + y * _mapSizeX].r * COLOR_MAX);
                _costMap[x + y * _mapSizeX] = (int)COLOR_MAX - (int)(mapData[x + y * _mapSizeX].g * COLOR_MAX);
            }
        }
    }
}
