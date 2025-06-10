using System.ComponentModel;
using System.Runtime.CompilerServices;

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit { }

    // <summary>
    /// Indicates that a parameter captures the expression passed for another parameter as a string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class with the specified parameter name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter whose expression should be captured as a string.</param>439885
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the name of the parameter whose expression should be captured as a string.
        /// </summary>
        /// <value>
        /// The name of the parameter whose expression should be captured.
        /// </value>
        public string ParameterName { get; }
    }
}
#endif

#if !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }
        public bool   IsOptional  { get; init; }

        public const string RefStructs      = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);
    }
}
#endif // !NET7_0_OR_GREATER

#if !NET7_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute {}
}
#endif

#if !NET5_0_OR_GREATER
namespace RszTool
{
    // implementations below taken straight from c#
    public static class BitOperations
    {
        public static int LeadingZeroCount(uint value) => 31 ^ Log2(value);
        public static int PopCount(uint value)
        {
            return SoftwareFallback(value);

            static int SoftwareFallback(uint value)
            {
                const uint c1 = 0x_55555555u;
                const uint c2 = 0x_33333333u;
                const uint c3 = 0x_0F0F0F0Fu;
                const uint c4 = 0x_01010101u;

                value -= (value >> 1) & c1;
                value = (value & c2) + ((value >> 2) & c2);
                value = (((value + (value >> 4)) & c3) * c4) >> 24;

                return (int)value;
            }
        }

        private static ReadOnlySpan<byte> Log2DeBruijn => new byte[32]
        {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
        };

        public static int Log2(uint value)
        {
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;

            return Unsafe.AddByteOffset(
                ref System.Runtime.InteropServices.MemoryMarshal.GetReference(Log2DeBruijn),
                (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
        }
    }

    public static class BitConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SingleToInt32Bits(float value) => *(int*)(&value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Int32BitsToSingle(int value) => *(float*)(&value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(byte[] bytes, int startIndex) => System.BitConverter.ToInt32(bytes, startIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(byte[] value) => System.BitConverter.ToString(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(byte[] value, int startIndex) => System.BitConverter.ToString(value, startIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(byte[] value, int startIndex, int length) => System.BitConverter.ToString(value, startIndex, length);
    }
}
#endif
