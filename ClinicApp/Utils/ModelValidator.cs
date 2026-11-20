using System;
using System.Collections.Generic;
using System.Reflection;
using ClinicApp.Attributes;

namespace ClinicApp.Utils;

public static class ModelValidator
{
    public static ValidationResult Validate(object obj)
    {
        ValidationResult result = new ValidationResult();

        if (obj == null)
        {
            result.AddError("Object cannot be null.");
            return result;
        }

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo prop = properties[i];
            if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            object? value = prop.GetValue(obj);

            RequiredAttribute? requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttr != null)
            {
                bool isEmpty = value == null || (value is string strVal && string.IsNullOrWhiteSpace(strVal));
                if (isEmpty)
                {
                    string message = string.IsNullOrWhiteSpace(requiredAttr.ErrorMessage)
                        ? "Field is required."
                        : requiredAttr.ErrorMessage;
                    result.AddError($"{prop.Name}: {message}");
                }
            }

            MaxLengthAttribute? maxLengthAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLengthAttr != null)
            {
                if (value is string strVal && strVal.Length > maxLengthAttr.Length)
                {
                    string message = string.IsNullOrWhiteSpace(maxLengthAttr.ErrorMessage)
                        ? $"Must not exceed {maxLengthAttr.Length} characters."
                        : maxLengthAttr.ErrorMessage;
                    result.AddError($"{prop.Name}: {message}");
                }
            }

            MinValueAttribute? minValueAttr = prop.GetCustomAttribute<MinValueAttribute>();
            if (minValueAttr != null && value != null)
            {
                try
                {
                    double numericValue = Convert.ToDouble(value);
                    if (numericValue < minValueAttr.Min)
                    {
                        string message = string.IsNullOrWhiteSpace(minValueAttr.ErrorMessage)
                            ? $"Must be at least {minValueAttr.Min}."
                            : minValueAttr.ErrorMessage;
                        result.AddError($"{prop.Name}: {message}");
                    }
                }
                catch (FormatException)
                {
                }
                catch (InvalidCastException)
                {
                }
            }
        }

        return result;
    }

    public static void PrintInfo(Type type)
    {
        if (type == null)
        {
            Console.WriteLine("Тип не вказано.");
            return;
        }

        Console.WriteLine($"Тип: {type.Name}");
        PropertyInfo[] properties = type.GetProperties();

        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo prop = properties[i];
            List<string> attrList = new List<string>();

            foreach (Attribute attr in prop.GetCustomAttributes())
            {
                if (attr.GetType().Namespace == "System.Runtime.CompilerServices")
                {
                    continue;
                }

                string name = attr.GetType().Name;
                if (name.EndsWith("Attribute"))
                {
                    name = name.Substring(0, name.Length - "Attribute".Length);
                }

                if (attr is MaxLengthAttribute maxAttr)
                {
                    attrList.Add($"{name}({maxAttr.Length})");
                }
                else if (attr is MinValueAttribute minAttr)
                {
                    attrList.Add($"{name}({minAttr.Min})");
                }
                else
                {
                    attrList.Add(name);
                }
            }

            if (attrList.Count > 0)
            {
                Console.WriteLine($"  {prop.Name} ({prop.PropertyType.Name}) — [{string.Join(", ", attrList)}]");
            }
            else
            {
                Console.WriteLine($"  {prop.Name} ({prop.PropertyType.Name})");
            }
        }
    }
}