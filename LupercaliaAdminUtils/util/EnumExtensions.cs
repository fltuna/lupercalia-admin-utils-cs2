using System.Runtime.Serialization;

namespace LupercaliaAdminUtils.util;

public static class EnumExtensions
{
    public static bool TryGetEnumByEnumMemberValue<T>(string value, out T result) where T : struct, Enum
    {
        result = default;
    
        var found = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Where(e => 
            {
                var memberInfo = typeof(T).GetMember(e.ToString()).FirstOrDefault();
                if (memberInfo == null) return false;
            
                var attribute = memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                    .FirstOrDefault() as EnumMemberAttribute;
                return attribute?.Value == value;
            })
            .ToList();

        if (found.Count <= 0)
            return false;
        
        result = found.First();
        return true;

    }
    
    
    public static bool TryGetAllEnumMemberValues<T>(out List<string> values) where T : struct, Enum
    {
        try
        {
            values = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => 
                {
                    var memberInfo = typeof(T).GetMember(e.ToString()!).FirstOrDefault();
                    if (memberInfo == null) return null;
                
                    var attribute = memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                        .FirstOrDefault() as EnumMemberAttribute;
                    return attribute?.Value;
                })
                .Where(value => !string.IsNullOrEmpty(value))
                .Distinct()
                .ToList()!;
            
            return values.Count > 0;
        }
        catch
        {
            values = [];
            return false;
        }
    }
}