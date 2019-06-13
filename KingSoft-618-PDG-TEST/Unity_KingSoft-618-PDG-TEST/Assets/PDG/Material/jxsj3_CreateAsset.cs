using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;
/// <summary>
/// CreateAsset类为批量创建Prefab的窗口类，选择Hierarchy窗口的物体，点击创建Prefab即可在指定目录生成Prefab
/// </summary>
public class CreateAsset : EditorWindow
{
    [MenuItem("AssetsManager/批量生成资源")]

    static void AddWindow()
    {
        //创建窗口
        CreateAsset window = (CreateAsset)EditorWindow.GetWindow(typeof(CreateAsset), false, "批量生成Prefab");
        window.Show();

    }

    //输入文字的内容
    private string PrefabPath = "Assets/";
    private  string TerMPath = "Assets/PDG/Material/";
    
    private GameObject[] selectedGameObjects;


    [InitializeOnLoadMethod]
    public void Awake()
    {
        OnSelectionChange();
    }
    void OnGUI()
    {
        GUIStyle text_style = new GUIStyle();
        text_style.fontSize = 15;
        text_style.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Prefab导出路径:");
        PrefabPath = EditorGUILayout.TextField(PrefabPath);
        if (GUILayout.Button("浏览"))
        { EditorApplication.delayCall += OpenPrefabFolder; }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("地形材质路径:");
        TerMPath = EditorGUILayout.TextField(TerMPath);
        if (GUILayout.Button("浏览"))
        { EditorApplication.delayCall += OpenTMFolder; }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("当前选中了" + selectedGameObjects.Length + "个物体", text_style); 
        if (GUILayout.Button("生成当前选中物体的Prefab", GUILayout.MinHeight(20)))
        {
            if (selectedGameObjects.Length <= 0)
            {
                //打开一个通知栏  
                this.ShowNotification(new GUIContent("未选择所要导出的物体"));
                return;
            }
            if (!Directory.Exists(PrefabPath))
            {
                Directory.CreateDirectory(PrefabPath);
            }
           

            foreach (GameObject m in selectedGameObjects)
            {
                CreatePrefab(m, m.name);
            }
            AssetDatabase.Refresh();
        }

        //改变地形材质
        if (GUILayout.Button("改变地形材质", GUILayout.MinHeight(20)))
        {
            if (selectedGameObjects.Length <= 0)
            {
                //打开一个通知栏  
                this.ShowNotification(new GUIContent("未选择所要导出的物体"));
                return;
            }
            if (!Directory.Exists(TerMPath))
            {
                this.ShowNotification(new GUIContent("请选择地形材质"));
                return;
            }


            foreach (GameObject m in selectedGameObjects)
            {
                if (m.GetComponent<Terrain>())
                {
                    ModifyM(m);
                    Debug.Log(m.name + "的地形材质修改成功");
                }
                else
                {
                    Debug.Log(m.name + "不是地形");
                }
            }
            AssetDatabase.Refresh();
        }

    }  

    void OpenPrefabFolder()
    {
        string path = EditorUtility.OpenFolderPanel("选择要导出的路径", "", "");
        //判断路径是否在当前工程目录下
        //if (!path.Contains(Application.dataPath))
        //{
        //    Debug.LogError("导出路径应在当前工程目录下");
        //    return;
        //}
        if (path.Length != 0)
        {
            int firstindex = path.IndexOf("Assets/");
            PrefabPath = path.Substring(firstindex) + "/";
            EditorUtility.FocusProjectWindow();
        }
    }

    void OpenTMFolder()
    {
        string path = EditorUtility.OpenFolderPanel("选择要导出的路径", "", "");
        //判断路径是否在当前工程目录下
        //if (!path.Contains(Application.dataPath))
        //{
        //    Debug.LogError("导出路径应在当前工程目录下");
        //    return;
        //}
        if (path.Length != 0)
        {
            int firstindex = path.IndexOf("Assets/");
            TerMPath = path.Substring(firstindex) + "/";
            EditorUtility.FocusProjectWindow();
        }
    }

    void CreatePrefab(GameObject go, string name)
    {
        //先创建一个空的预制物体
        //预制物体保存在工程中路径，可以修改("Assets/" + name + ".prefab");
        GameObject tempPrefab = PrefabUtility.CreatePrefab(PrefabPath + name + ".prefab", go);
       
        //返回创建后的预制物体

    }

    void ModifyM(GameObject go)
    {
        go.GetComponent<Terrain>().materialType = Terrain.MaterialType.Custom;
        Material referencedMaterial = Resources.Load<Material>("TerrainMaterial");
        go.GetComponent<Terrain>().materialTemplate = referencedMaterial;
    }

    void OnInspectorUpdate()
    {
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        selectedGameObjects = Selection.gameObjects;

    }
}
