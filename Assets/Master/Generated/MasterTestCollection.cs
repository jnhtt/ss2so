// GENERATED BY SCRIPT! DO NOT EDIT!
using System;

namespace Master
{
    [Serializable]
    public partial class MasterTestCollection : MasterCollection<uint, MasterTest>
    {
        
        public static MasterTestCollection LoadFromJson(string jsonString)
        {
            var instance = CreateInstance<MasterTestCollection>();
            return LoadFromJson<MasterTestCollection>(instance, jsonString);
        }
        
    }
}
            