using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// ���b�V��������s���N���X
/// </summary>
public class MeshControl
{
    /// <summary> ���b�V���ۑ��� </summary>
    private const string FIRE_PATH = "Assets/MyGame/Meshs/";
    /// <summary>
    /// �w��I�u�W�F�N�g�̎q�I�u�W�F�N�g�̃��b�V������������
    /// </summary>
    /// <param name="target">�w��I�u�W�F�N�g</param>
    public static void Combine(Transform target)
    {
        var meshFilters = target.GetComponentsInChildren<MeshFilter>();
        var meshRenderers = target.GetComponentsInChildren<MeshRenderer>();

        //Material���ƂɃ��b�V����o�^
        var materialDic = new Dictionary<string, Material>();
        var meshFilterDic = new Dictionary<string,List<MeshFilter>>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            string materialName = meshRenderers[i].material.name;
            //�o�^����Ă��Ȃ��ꍇ��List�𐶐�
            if (!meshFilterDic.ContainsKey(materialName))
            {
                var filters = new List<MeshFilter>();
                meshFilterDic.Add(materialName, filters);
                materialDic.Add(materialName, meshRenderers[i].material);
            }
            meshFilterDic[materialName].Add(meshFilters[i]);
        }
        //Material���ƂɌ����������s��
        foreach(KeyValuePair<string,List<MeshFilter>> filterList in meshFilterDic)
        {
            //�������b�V���̓y��𐶐�
            var obj = new GameObject($"(Comb:{ filterList.Key})");
            obj.transform.SetParent(target);
            obj.transform.localPosition = Vector3.zero;
            //�`��p�R���|�[�l���g��ǉ�
            var combFilter = obj.AddComponent<MeshFilter>();
            var combRenderer = obj.AddComponent<MeshRenderer>();
            //��������I�u�W�F�N�g�̃��b�V���ƍ��W��ۑ�
            CombineInstance[] combines = new CombineInstance[filterList.Value.Count];
            for (int i = 0;i < filterList.Value.Count; i++)
            {
                combines[i].mesh = filterList.Value[i].sharedMesh;
                combines[i].transform = filterList.Value[i].transform.localToWorldMatrix;
                filterList.Value[i].gameObject.SetActive(false);
            }
            //��������
            combFilter.mesh = new Mesh();
            combFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;//PC�p
            combFilter.mesh.CombineMeshes(combines);
            //Material���f
            combRenderer.material = materialDic[filterList.Key];
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// ���b�V����Editor�ɕۑ�����
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
