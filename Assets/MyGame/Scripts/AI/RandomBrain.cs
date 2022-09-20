using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TestBrain")]
public class RandomBrain : PieceBrain
{
    public override MapPoint GetMoveTarget(SearchMap searchMap)
    {
        return searchMap.FootprintsPoints[Random.Range(0, searchMap.FootprintsPoints.Count)];
    }
}
