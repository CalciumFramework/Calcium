using System;
using System.Diagnostics.CodeAnalysis;

namespace Calcium.Reflection
{
    /// <summary>
    /// This attribute includes allows you to specify a type that will be ignored
    /// by the trimming process of the linker.
    /// It achieves this using the <see cref="DynamicallyAccessedMembersAttribute"/>.
    /// </summary>
    public class NoTrimAttribute : Attribute
    {
        readonly Type doNotTrimType;

        /// <summary>
        /// This attribute includes allows you to specify a type that will be ignored
        /// by the trimming process of the linker.
        /// It achieves this using the <see cref="DynamicallyAccessedMembersAttribute"/>.
        /// </summary>
        public NoTrimAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type doNotTrimType)
        {
            this.doNotTrimType = doNotTrimType;
        }
    }
}
