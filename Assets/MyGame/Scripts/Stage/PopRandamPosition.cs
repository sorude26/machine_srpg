using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopRandamPosition : MonoBehaviour
{
    [SerializeField]
    private float _range = 1f;
    private void OnEnable()
    {
        float rx = Random.Range(-_range, _range);
        float rz = Random.Range(-_range, _range);
        transform.position += new Vector3(rx, 0, rz);
    }
}
