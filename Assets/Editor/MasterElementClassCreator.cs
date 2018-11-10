﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MasterElementClassCreator
{
    public static readonly string MASTER_GENERATED_PATH = Application.dataPath + "/Master/Generated";
    public const string PRIMARY_KEY = "id";

    public static bool Create(string masterName, string jsonString)
    {
        try {
            var masterJsonData = LitJson.JsonMapper.ToObject<LitJson.JsonData>(jsonString);
            CreateMasterElementClassScript(masterName, masterJsonData);

        } catch (System.Exception e) {
            Debug.LogError(e.ToString());
            return false;
        }
        return true;
    }

    private static void CreateMasterElementClassScript(string masterName, LitJson.JsonData masterJsonData)
    {
        string classContent =
            @"// GENERATED BY SCRIPT! DO NOT EDIT!
using System;

namespace Master
{
    [Serializable]
    public class $0 : MasterElement<$1>
    {
        $2
        
        $3
    }
}
            ";

        classContent = classContent.Replace("$0", masterName);
        classContent = classContent.Replace("$1", GetClassPrimaryKeyType(masterJsonData));
        classContent = classContent.Replace("$2", GetClassProperties(masterJsonData));
        classContent = classContent.Replace("$3", GetClassConstructor(masterName, masterJsonData));

        string path = string.Format("{0}/{1}.cs", MASTER_GENERATED_PATH, masterName);
        System.IO.File.WriteAllText(path, classContent);
    }

    private static string GetClassPrimaryKeyType(LitJson.JsonData masterJsonData)
    {
        var primary = masterJsonData["masterHeader"][0];
        return primary.ToString();
    }

    private static string GetClassProperties(LitJson.JsonData masterJsonData)
    {
        string properties = "";
        foreach (KeyValuePair<string, LitJson.JsonData> t in masterJsonData["masterHeader"])
        {
            if (PRIMARY_KEY == t.Key)
            {
                continue;
            }
            properties += string.Format("public {0} {1};", t.Value, t.Key) + System.Environment.NewLine;
        }

        return properties;
    }

    private static string GetClassConstructor(string className, LitJson.JsonData masterJsonData)
    {
        string constructor = @"
        public $0($1) : base($2)
        {
$3
        }
        ";

        string args = "";
        foreach (KeyValuePair<string, LitJson.JsonData> t in masterJsonData["masterHeader"])
        {
            args += string.Format("{0} {1},", t.Value, t.Key);
        }
        args = args.TrimEnd(',');

        string properitiesSetter = "";
        foreach (KeyValuePair<string, LitJson.JsonData> t in masterJsonData["masterHeader"])
        {
            if (PRIMARY_KEY == t.Key)
            {
                continue;
            }
            properitiesSetter += string.Format(@"           this.{0} = {1};", t.Key, t.Key) + System.Environment.NewLine;
        }

        constructor = constructor.Replace("$0", className);
        constructor = constructor.Replace("$1", args);
        constructor = constructor.Replace("$2", PRIMARY_KEY);
        constructor = constructor.Replace("$3", properitiesSetter);
        return constructor;
    }
}