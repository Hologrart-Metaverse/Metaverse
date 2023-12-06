using Newtonsoft.Json;

public static class JsonHelper<T>
{
    public static string Serialize(T obj, JsonSerializerSettings serializerSettings = default)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented, serializerSettings);
    }
    public static T Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}
