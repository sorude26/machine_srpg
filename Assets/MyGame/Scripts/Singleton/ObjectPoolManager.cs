using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトプールの管理クラス
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    /// <summary> 基本初期生成オブジェクト数 </summary>
    private const int DEFAULT_POOL_COUNT = 5;
    private static ObjectPoolManager s_instance = default;
    private static Dictionary<string, List<GameObject>> s_poolObjects = default;
    public static ObjectPoolManager Instance
    {
        get 
        {
            if (s_instance == null)
            {
                var obj = new GameObject("ObjectPool");
                var ins = obj.AddComponent<ObjectPoolManager>();
                s_instance = ins;
                s_poolObjects = new Dictionary<string, List<GameObject>>();
                DontDestroyOnLoad(obj);
            }
            return s_instance; 
        }
    }
    /// <summary>
    /// 指定オブジェクトのプールを生成する
    /// </summary>
    /// <param name="poolObject">プールを生成するオブジェクト</param>
    /// <param name="createCount">生成数</param>
    public void CreatePool(GameObject poolObject,int createCount = DEFAULT_POOL_COUNT)
    {
        if (s_poolObjects.ContainsKey(poolObject.ToString()))
        {
            return;
        }
        var poolObjects = new List<GameObject>();
        for (int i = 0; i < createCount; i++)
        {
            var obj = Instantiate(poolObject,this.transform);
            obj.gameObject.SetActive(false);
            poolObjects.Add(obj);
        }
        s_poolObjects.Add(poolObject.ToString(), poolObjects);
    }
    /// <summary>
    /// 使用可能なオブジェクトを返す
    /// </summary>
    /// <param name="useObject"></param>
    /// <returns></returns>
    public GameObject Use(GameObject useObject)
    {
        if (!s_poolObjects.ContainsKey(useObject.ToString())) //プールが生成されていないオブジェクトの場合、プールを生成する
        {
            CreatePool(useObject);
        }
        foreach (var poolObject in s_poolObjects[useObject.ToString()])
        {
            if (poolObject.gameObject.activeInHierarchy)
            {
                continue;
            }
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }
        var obj = Instantiate(useObject, this.transform);
        s_poolObjects[useObject.ToString()].Add(obj);
        return obj;
    }
    public void CleanUp()
    {
        foreach (var poolObject in s_poolObjects.Values)
        {
            foreach (var pool in poolObject)
            {
                pool.gameObject.SetActive(false);
            }
        }
    }
}
