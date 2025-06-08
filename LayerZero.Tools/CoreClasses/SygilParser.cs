using LayerZero.Tools.Guard;
using System.Globalization;
using System.Text.Json;

namespace LayerZero.Tools.CoreClasses
{
    public static class SygilParser
    {

        public static T? Parse<T>(this string Input,
                                    bool strict = false,
                                    string? format = null,
                                    CultureInfo? culture = null)
        {
            Input.IsNotNullNorEmptyOrWhiteSpace();


            culture ??= CultureInfo.InvariantCulture;
            var type = typeof(T);


            if (type == typeof(string))
                return (T)(object)Input;


            if (type.IsClass)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(Input);
                }
                catch(Exception ex)
                {
                    if (strict) throw ex;
                    return default;
                }
            }
            else if (type.IsEnum)
            {
                try
                {
                    return (T?)Enum.Parse(type, Input, ignoreCase: true);
                }
                catch
                {
                    if (strict) throw new ArgumentException($"Cannot parse strict to {type.Name}");
                    return default;
                }
            }
            else if (type.IsPrimitive)
            {
                try
                {
                    return (T?)Convert.ChangeType(Input, type);
                }
                catch (Exception ex)
                {
                    if (strict) throw ex;

                    return default;
                }
            }
            else if (type.IsValueType)
            {
                
                if (type == typeof(DateTime))
                {
                    try
                    {
                        if (strict)
                            format.IsNotNullNorEmptyOrWhiteSpace();
                        var result = format != null
                            ? DateTime.ParseExact(Input, format, culture)
                            : DateTime.Parse(Input, culture);

                        return (T)(object)result;
                    }
                    catch
                    {
                        if (strict) throw new FormatException($"Cannot parse strict to {type.Name}");
                        return default;
                    }
                }

                if (type == typeof(DateOnly))
                {
                    try
                    {
                        if (strict)
                            format.IsNotNullNorEmptyOrWhiteSpace();
                        var result = format != null
                            ? DateOnly.ParseExact(Input, format, culture)
                            : DateOnly.Parse(Input, culture);

                        return (T)(object)result;
                    }
                    catch
                    {
                        if (strict) throw new FormatException($"Cannot parse strict to {type.Name}");
                        return default;
                    }

                }

                if (type == typeof(TimeOnly))
                {
                    try
                    {
                        if (strict)
                            format.IsNotNullNorEmptyOrWhiteSpace();
                        var result = format != null
                            ? TimeOnly.ParseExact(Input, format, culture)
                            : TimeOnly.Parse(Input, culture);

                        return (T)(object)result;
                    }
                    catch
                    {
                        if (strict) throw new FormatException($"Cannot parse strict to {type.Name}");
                        return default;
                    }
                }


                var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod != null && parseMethod.IsStatic)
                {
                    return (T?) parseMethod.Invoke(null, new object[] { Input });
                }


                if (Input.Trim().StartsWith("{") && type.IsValueType)
                {
                    return JsonSerializer.Deserialize<T>(Input);
                }

                throw new NotSupportedException($"Cannot parse to {type.Name}");
            }
            else
            {
                return JsonSerializer.Deserialize<T>(Input);
            }


        }
    }
}
