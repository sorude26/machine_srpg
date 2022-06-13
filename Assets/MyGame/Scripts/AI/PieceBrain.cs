using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceBrain : ScriptableObject
{
    public abstract Vector2Int GetMoveTarget(SearchMap searchMap);
}
