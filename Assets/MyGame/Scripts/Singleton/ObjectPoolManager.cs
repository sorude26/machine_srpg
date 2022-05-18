using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �I�u�W�F�N�g�v�[���̊Ǘ��N���X
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    /// <summary> ��{���������I�u�W�F�N�g�� </summary>
    private const int DEFAULT_POOL_COUNT = 5;
    private static ObjectPoolManager s_instance = default;
    private static Dictionary<int,List<GameObject>> s_poolObjects = default;
    private static Dictionary<string,int> s_keysDic = default;
    public static ObjectPoolManager Instance
    {
        get 
        {
            if (s_instance == null)
            {
                var obj = new GameObject("ObjectPool");
                var ins = obj.AddComponent<ObjectPoolManager>();
                s_instance = ins;
                s_keysDic = new Dictionary<string, int>();
                s_poolObjects = new Dictionary<int, List<GameObject>>();
                DontDestroyOnLoad(obj);
            }
            return s_instance; 
        }
    }
    /// <summary>
    /// �w��I�u�W�F�N�g�̃v�[���𐶐�����
    /// </summary>
    /// <param name="poolObject">�v�[���𐶐�����I�u�W�F�N�g</param>
    /// <param name="createCount">������</param>
    public void CreatePool(GameObject poolObject,int createCount = DEFAULT_POOL_COUNT)
    {
        if (s_keysDic.ContainsKey(poolObject.ToString()))
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
        s_keysDic.Add(poolObject.ToString(), s_poolObjects.Count);
        s_poolObjects.Add(s_keysDic[poolObject.ToString()], poolObjects);
    }
    /// <summary>
    /// �g�p�\�ȃI�u�W�F�N�g��Ԃ�
    /// </summary>
    /// <param name="useObject"></param>
    /// <returns></returns>
    public GameObject Use(GameObject useObject)
    {
        if (!s_keysDic.ContainsKey(useObject.ToString())) //�v�[������������Ă��Ȃ��I�u�W�F�N�g�̏ꍇ�A�v�[���𐶐�����
        {
            CreatePool(useObject);
        }
        foreach (var poolObject in s_poolObjects[s_keysDic[useObject.ToString()]])
        {
            if (poolObject.gameObject.activeInHierarchy)
            {
                continue;
            }
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }
        var obj = Instantiate(useObject, this.transform);
        s_poolObjects[s_keysDic[useObject.ToString()]].Add(obj);
        return obj;
    }
    public void CleanUp()
    {
        for (int i = 0; i < s_poolObjects.Count; i++)
        {
            foreach (var pool in s_poolObjects[i])
            {
                pool.gameObject.SetActive(false);
            }
        }
    }
}
