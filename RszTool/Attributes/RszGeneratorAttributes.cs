using System.Text.Json;
using RszTool.Efx;

namespace RszTool
{
    /// <summary>
    /// Generate the read/write code for a given class - DefaultRead(FileHandler) and DefaultWrite(FileHandler).
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RszGenerateAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Automate the read method for an IModel fully.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RszAutoReadAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Automate the write method for an IModel fully.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RszAutoWriteAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Automate both the read and write methods for an IModel fully.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RszAutoReadWriteAttribute : System.Attribute
    {
    }


    /// <summary>
    /// Ignore the marked field for serializer purposes, for fields that can't be easily automated.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszIgnoreAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Mark a string as being an inline wstring (UTF-16). If no size is provided, a length integer prefix is expected.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszInlineWStringAttribute : System.Attribute
    {
        public object[] SizeProvider { get; }

        public RszInlineWStringAttribute(params object[] sizeProvider)
        {
            SizeProvider = sizeProvider;
        }
    }

    /// <summary>
    /// Mark a string as being an int64 offset to a wstring (UTF-16) in the string table.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszOffsetWStringAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Mark a string as being an inline ascii string. If no size is provided, a length integer prefix is expected.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszInlineStringAttribute : System.Attribute
    {
        public object[] SizeProvider { get; }

        public RszInlineStringAttribute(params object[] sizeProvider)
        {
            SizeProvider = sizeProvider;
        }
    }

    /// <summary>
    /// Add a fixed size padding after the field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszPaddingAfterAttribute : System.Attribute
    {
        public int Amount { get; }
        public object[] Condition { get; }

        public RszPaddingAfterAttribute(int amount, params object[] condition)
        {
            Amount = amount;
            Condition = condition;
        }
    }

    /// <summary>
    /// Mark a field as containing the UTF-16 MurMur3 hash of the given string field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszStringHashAttribute : System.Attribute
    {
        public string HashedStringField { get; init; }

        public RszStringHashAttribute(string field)
        {
            HashedStringField = field;
        }
    }

    /// <summary>
    /// Mark a field as containing the ascii MurMur3 hash of the given string field.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszStringAsciiHashAttribute : System.Attribute
    {
        public string HashedStringField { get; init; }

        public RszStringAsciiHashAttribute(string field)
        {
            HashedStringField = field;
        }
    }


    /// <summary>
    /// Conditionally deserialize the marked field. EndAt can be used to share the condition for multiple sequential fields.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    internal sealed class RszConditionalAttribute : System.Attribute
    {
        public object[] Condition { get; }
        public string? EndAt { get; init; }

        public RszConditionalAttribute(params object[] condition)
        {
            Condition = condition;
        }
    }


    /// <summary>
    /// Marks a TStruct[] field with an optional size. If size parameters are not given, a length integer prefix is expected.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszFixedSizeArrayAttribute : System.Attribute
    {
        public object[] SizeFunc { get; }

        public RszFixedSizeArrayAttribute(params object[] sizeFunc)
        {
            SizeFunc = sizeFunc;
        }
    }


    /// <summary>
    /// Marks a List<IModel> field. If size parameters are not given, a length prefixed integer is expected.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszListAttribute : System.Attribute
    {
        public object[] SizeFunc { get; }

        public RszListAttribute(params object[] sizeFunc)
        {
            SizeFunc = sizeFunc;
        }
    }


    /// <summary>
    /// Mark the type as needing to be instantiated with specific constructor parameters.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszConstructorParamsAttribute : System.Attribute
    {
        public object[] Parameters { get; }

        public RszConstructorParamsAttribute(params object[] parameters)
        {
            Parameters = parameters;
        }
    }

    /// <summary>
    /// Marks an IModel class.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszClassInstanceAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Marks an abstract type based on conditions. Target field must be an IModel class.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class RszSwitchAttribute : System.Attribute
    {
        public object[] Args { get; }

        public RszSwitchAttribute(params object[] args)
        {
            Args = args;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class EfxStructAttribute : System.Attribute
    {
        public EfxAttributeType AttributeType { get; }
        public EfxVersion[] GameVersions { get; }

        public EfxStructAttribute(EfxAttributeType attributeType, params EfxVersion[] gameVersions)
        {
            AttributeType = attributeType;
            GameVersions = gameVersions;
        }
    }

    /// <summary>
    /// Mark the class type as having versioning support based on the specified version type
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RszVersionedObjectAttribute : System.Attribute
    {
        public Type VersionValueType { get; }
        public string VersionFieldName { get; }

        public RszVersionedObjectAttribute(Type versionValueType, string versionFieldName = "Version")
        {
            VersionValueType = versionValueType;
            VersionFieldName = versionFieldName;
        }
    }

    /// <summary>
    /// Conditionally deserialize the marked field based on a version condition. EndAt can be used to share the condition for multiple sequential fields.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    internal sealed class RszVersionAttribute : System.Attribute
    {
        public object[] Condition { get; }
        public string? EndAt { get; init; }

        public RszVersionAttribute(params object[] condition)
        {
            Condition = condition;
        }
    }


    public class FieldListInput
    {
        public int FileVersion { get; set; }
    }
}
