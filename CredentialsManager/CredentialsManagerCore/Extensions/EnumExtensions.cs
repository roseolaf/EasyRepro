using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Testautomation.CredentialsManagerCore.Extensions
{
    public static class EnumExtensions
    {
        public static string ToEnumString(this Enum @enum)
        {
            var enumType = @enum.GetType();
            var name = Enum.GetName(enumType, @enum);
            var member = enumType.GetMember(name ?? throw new InvalidOperationException("Enum.GetName() returned a null value in ToEnumString Extension Method"));
            var attribute = (member.First() ?? throw new InvalidOperationException("Attribute not set for given Enum value")).GetCustomAttribute<EnumMemberAttribute>();
            return attribute?.Value;
        }

        public static T ToEnum<T>(this string @string)
        {
            var enumType = typeof(T);
            var members = enumType.GetMembers();
            foreach (var member in members)
            {
                var attr = member.GetCustomAttribute<EnumMemberAttribute>();
                if (attr?.Value == @string)
                {
                    return (T) Enum.Parse(enumType, member.Name);
                }
            }

            return default;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name).GetCustomAttribute<TAttribute>();
        }

    }
}
