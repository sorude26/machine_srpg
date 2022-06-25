using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceBrain : ScriptableObject
{
    public abstract MapPoint GetMoveTarget(SearchMap searchMap);
}
