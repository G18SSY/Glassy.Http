using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Hides standard Object members to make fluent interfaces
    ///     easier to read.
    ///     <remarks>
    ///         Taken unmodified from Autofac (https://github.com/autofac/Autofac)
    ///         Based on blog post by @kzu here:
    ///         http://www.clariusconsulting.net/blogs/kzu/archive/2008/03/10/58301.aspx.
    ///     </remarks>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMembers
    {
        /// <summary>
        ///     Standard System.Object member.
        /// </summary>
        /// <returns>Standard result.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary>
        ///     Standard System.Object member.
        /// </summary>
        /// <returns>Standard result.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        ///     Standard System.Object member.
        /// </summary>
        /// <returns>Standard result.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>
        ///     Standard System.Object member.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Standard result.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object other);
    }
}