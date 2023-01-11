using System;
using System.Text.Json;
using UnityEngine;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace MiniGameFramework
{
    public static class JsonUtil
    {
        public static T FromJson<T>(string json)
        {
            if (GameApp.Inst.Platform == PlatformEnum.PlatformAndroid)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return JsonSerializer.Deserialize<T>(json);
            }
        }

        public static object FromJson(string json, System.Type type)
        {
            if (GameApp.Inst.Platform == PlatformEnum.PlatformAndroid)
                return JsonConvert.DeserializeObject(json, type);
            else
                return JsonSerializer.Deserialize(json, type);
        }

        public static string ToJson(object obj, Type type)
        {
            if (GameApp.Inst.Platform == PlatformEnum.PlatformAndroid)
            {
                return JsonConvert.SerializeObject(obj);
            }
            else
                return JsonSerializer.Serialize(obj, type);
        }
    }
}