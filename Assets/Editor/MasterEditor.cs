using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

public static class MasterEditor
{
    public static readonly string MASTER_GENERATED_PATH = Application.dataPath + "/Master/Generated";

    public const string GAS_URL = "https://script.google.com/macros/s/[YOUR_ENV]/exec?sheetName=";
    public const string PRIMARY_KEY = "id";

    private static System.Action onFinishCompile;

    [MenuItem("Master/Create MasterClass")]
    public static void CreateMasterClass()
    {
        IEnumerator t = GetCoroutine(
            GAS_URL + "Test",
            CreateMasterClass);
        while (t.MoveNext()) {
            
        }
    }

    [MenuItem("Master/Create MasterData")]
    public static void CreateMasterData()
    {
        IEnumerator t = GetCoroutine(
            GAS_URL + "Test",
            CreateMasterData);
        while (t.MoveNext())
        {

        }
    }

    private static IEnumerator GetCoroutine(string url, System.Action<string, string> onFinish)
    {
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        while (!request.isDone) {
            yield return null;
        }
        if (request.isHttpError || request.isNetworkError) {
            EditorUtility.DisplayDialog("Error", request.error, "OK");
        } else {
            EditorUtility.DisplayDialog("Success", request.downloadHandler.text, "OK");
            string sheetName = "";
            if (TryGetMasterName(url, out sheetName))
            {
                sheetName = char.ToUpper(sheetName[0]) + sheetName.Substring(1);
            }
            if (onFinish != null) {
                onFinish(sheetName, request.downloadHandler.text);
            }
        }
    }

    private static void CreateMasterClass(string masterName, string jsonString)
    {
        CreateMasterScript(masterName, jsonString);
        AssetDatabase.ImportAsset(MasterElementClassCreator.MASTER_GENERATED_PATH + "/" + masterName + ".cs");
        AssetDatabase.ImportAsset(MasterElementClassCreator.MASTER_GENERATED_PATH + "/" + masterName + "Collection.cs");
        AssetDatabase.Refresh();
    }

    private static void CreateMasterScript(string masterName, string jsonString)
    {
        MasterElementClassCreator.Create(masterName, jsonString);
        MasterCollectionClassCreator.Create(masterName, jsonString);
    }

    private static void CreateMasterData(string masterName, string jsonString)
    {
        LoadScriptableObject(masterName, jsonString);
    }

    private static void LoadScriptableObject(string masterName, string jsonString)
    {
        string masterElementCollectionType = string.Format("{0}Collection", masterName);
        try
        {
            Type type = GetTypeByClassName(masterElementCollectionType);
            object result = type.InvokeMember("LoadFromJson", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { jsonString });

            var masterCollection = (UnityEngine.Object)Convert.ChangeType(result, type);
            AssetDatabase.CreateAsset(masterCollection, string.Format("Assets/Resources/MasterData/{0}.asset", masterName));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        } catch (Exception e) {
            Debug.LogError(e.ToString());
        }
    }

    private static bool TryGetMasterName(string url, out string sheetName)
    {
        sheetName = "";
        var r = new System.Text.RegularExpressions.Regex(@"sheetName=(?<sheetName>.+)", System.Text.RegularExpressions.RegexOptions.ECMAScript);
        var m = r.Match(url);
        if (m.Success)
        {
            sheetName = m.Groups["sheetName"].Value;
            sheetName = string.Format("master{0}", sheetName);
            return true;
        }
        return false;
    }

    private static Type GetTypeByClassName(string className)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (var t in asm.GetTypes()) {
                if (t.Name == className) {
                    return t;
                }
            }
        }
        return null;
    }
}
