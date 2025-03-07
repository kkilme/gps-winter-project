using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Data;
using System;


public class SkillDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(SkillData).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);

        // Type 필드 값을 확인하여 적절한 타입 결정
        string typeName = jObject["Type"]?.ToString();
        Type targetType = null;

        if (typeName == "AttackSkillData")
        {
            targetType = typeof(AttackSkillData);
        }
        else if (typeName == "SkillData")
        {
            targetType = typeof(SkillData);
        }
        else
        {
            targetType = typeof(SkillData);
        }

        // 현재 컨버터를 제거하여 무한 재귀 방지
        var currentConverter = this;
        serializer.Converters.Remove(currentConverter);

        // targetType으로 역직렬화
        var result = jObject.ToObject(targetType, serializer); // 컨버터 제거하지 않을 시 여기서 무한 재귀 발생

        // 다시 컨버터 추가
        serializer.Converters.Add(currentConverter);

        return result;
    }

    // 쓰기는 필요 없음
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
