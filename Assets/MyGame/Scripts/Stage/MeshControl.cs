using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// メッシュ操作を行うクラス
/// </summary>
public class MeshControl
{
    /// <summary> メッシュ保存先 </summary>
    private const string FIRE_PATH = "Assets/MyGame/Meshs/";
    /// <summary>
    /// 指定オブジェクトの子オブジェクトのメッシュを結合する
    /// </summary>
    /// <param name="target">指定オブジェクト</param>
    public static void Combine(Transform target)
    {
        var meshFilters = target.GetComponentsInChildren<MeshFilter>();
        var meshRenderers = target.GetComponentsInChildren<MeshRenderer>();

        //Materialごとにメッシュを登録
        var materialDic = new Dictionary<string, Material>();
        var meshFilterDic = new Dictionary<string,List<MeshFilter>>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            string materialName = meshRenderers[i].material.name;
            //登録されていない場合はListを生成
            if (!meshFilterDic.ContainsKey(materialName))
            {
                var filters = new List<MeshFilter>();
                meshFilterDic.Add(materialName, filters);
                materialDic.Add(materialName, meshRenderers[i].material);
            }
            meshFilterDic[materialName].Add(meshFilters[i]);
        }
        //Materialごとに結合処理を行う
        foreach(KeyValuePair<string,List<MeshFilter>> filterList in meshFilterDic)
        {
            //結合メッシュの土台を生成
            var obj = new GameObject($"(Comb:{ filterList.Key})");
            obj.transform.SetParent(target);
            obj.transform.localPosition = Vector3.zero;
            //描画用コンポーネントを追加
            var combFilter = obj.AddComponent<MeshFilter>();
            var combRenderer = obj.AddComponent<MeshRenderer>();
            //結合するオブジェクトのメッシュと座標を保存
            CombineInstance[] combines = new CombineInstance[filterList.Value.Count];
            for (int i = 0;i < filterList.Value.Count; i++)
            {
                combines[i].mesh = filterList.Value[i].sharedMesh;
                combines[i].transform = filterList.Value[i].transform.localToWorldMatrix;
                filterList.Value[i].gameObject.SetActive(false);
            }
            //結合処理
            combFilter.mesh = new Mesh();
            combFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;//PC用
            combFilter.mesh.CombineMeshes(combines);
            //Material反映
            combRenderer.material = materialDic[filterList.Key];
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// メッシュをEditorに保存する
    /// </summary>
    /// <param name="meshName"></param>
    /// <param name="meshFilter"></param>
    public static void CreateMesh(string meshName, MeshFilter meshFilter)
    {
        AssetDatabase.CreateAsset(meshFilter.mesh, FIRE_PATH + meshName + ".asset");
        AssetDatabase.SaveAssets();
    }
#endif
}
