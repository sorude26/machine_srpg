using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    private const uint COLOR_MAX = 255;
    [SerializeField]
    private Sprite _mapSprite = default;
    [SerializeField]
    private Sprite _costSprite = default;
    private int _mapSizeX = default;
    private int _mapSizeY = default;
    private int[] _levelMap = default;
    private int[] _costMap = default;

    public int MapSizeX { get => _mapSizeX; }
    public int MapSizeY { get => _mapSizeY; }
    public int[] LevelMap { get => _levelMap; }
    public int[] CostMap { get => _costMap; }
    [SerializeField]
    StageCreator _stageCreater = default;
    private void Start()
    {
        //LoadMap();
        //_stageCreater.CreateStage(_mapSizeX,_mapSizeY, _levelMap,_costMap);
    }
    public void LoadMap()
    {
        _mapSizeX = (int)_mapSprite.rect.width;
        _mapSizeY = (int)_mapSprite.rect.height;
        _levelMap = new int[_mapSizeY * _mapSizeY];
        _costMap = new int[_mapSizeY * _mapSizeY];
        Color[] mapData = _mapSprite.texture.GetPixels();
        Color[] colors = _costSprite.texture.GetPixels();
        //for (int y = 0; y < _mapSizeY; y++)
        //{
        //    for (int x = 0; x < _mapSizeX; x++)
        //    {
        //        _levelMap[x + y * _mapSizeX] = (int)COLOR_MAX - (int)(mapData[x + y * _mapSizeX].r * COLOR_MAX);
        //        _costMap[x + y * _mapSizeX] = (int)COLOR_MAX - (int)(mapData[x + y * _mapSizeX].g * COLOR_MAX);
        //    }
        //}

        for (int i = 0; i < _mapSizeX * _mapSizeY; i++)
        {
            _levelMap[i] = (int)(COLOR_MAX - mapData[i].r * COLOR_MAX);
        }
        if (mapData.Length != colors.Length)
        {
            return;
        }
        for (int i = 0;i < _mapSizeY * _mapSizeX; i++)
        {
            _costMap[i] = RGBCheck(colors[i]);
        }
    }
    private int RGBCheck(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b; 
        if (g > r + b)
        {
            return 3;
        }
        else if (r > g + b)
        {
            return 2;
        }
        else if(b > r + g)
        {
            return 1;
        }
        return 0;
    }
}
