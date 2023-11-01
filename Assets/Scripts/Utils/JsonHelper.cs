using Newtonsoft.Json;

public static class JsonHelper<T>
{
    public static string Serialize(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
    public static T Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}
