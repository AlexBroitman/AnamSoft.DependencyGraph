using AnamSoft.ReadOnlySet;
using System;

namespace AnamSoft.DependencyGraph
{
    /// <summary>
    /// Interface for generic DependencyGraph
    /// </summary>
    /// <typeparam name="T">A type of the <see cref="IDependencyGraph{T}"/> node.</typeparam>
    public interface IDependencyGraph<T>
    {
        /// <summary>
        /// Determines if the <see cref="IDependencyGraph{T}"/> is empty.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="IDependencyGraph{T}"/> is empty; <see langword="false"/>, otherwise.</value>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns <see langword="true"/> if the <see cref="IDependencyGraph{T}"/> allows cyclic dependencies.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="IDependencyGraph{T}"/> allows cyclic dependencies, <see langword="false"/> otherwise.</value>
        bool AllowCyclic { get; }

        /// <summary>
        /// Adds new dependency to the <see cref="IDependencyGraph{T}"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="AllowCyclic"/> it <see langword="false"/> and new dependency will cause cyclic, the dependency will not be added and the <see langword="return"/> value will be <see langword="false"/>
        /// </remarks>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the dependency was added; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dependent"/> is equal to <paramref name="dependency"/>.</exception>
        bool AddDependency(T dependent, T dependency);

        /// <summary>
        /// Removes a dependency from the <see cref="IDependencyGraph{T}"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the dependency was removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        bool RemoveDependency(T dependent, T dependency);

        /// <summary>
        /// Determines if a <paramref name="dependent"/> directly depends on a <paramref name="dependency"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the <paramref name="dependent"/> directly depends on <paramref name="dependency"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        bool IsDirectlyDepends(T dependent, T dependency);

        /// <summary>
        /// Determines if a <paramref name="dependent"/> depends on a <paramref name="dependency"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the <paramref name="dependent"/> depends on <paramref name="dependency"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        bool IsDepends(T dependent, T dependency);

        /// <summary>
        /// Gets all direct dependencies of <paramref name="dependent"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <returns><see cref="IReadOnlySet{T}"/> that contains all direct dependencies of <paramref name="dependent"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> is <see langword="null"/>.</exception>
        IReadOnlySet<T> GetDirectDependencies(T dependent);

        /// <summary>
        /// Gets all dependencies of <paramref name="dependent"/> including indirect.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <returns><see cref="IReadOnlySet{T}"/> that contains all dependencies of <paramref name="dependent"/> including indirect.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> is <see langword="null"/>.</exception>
        IReadOnlySet<T> GetAllDependencies(T dependent);

        /// <summary>
        /// Determines if the <see cref="IDependencyGraph{T}"/> has cyclic dependencies.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="IDependencyGraph{T}"/> has cyclic dependencies; otherwise, <see langword="false"/>.</returns>
        bool HasCyclic();

        /// <summary>
        /// Clears the <see cref="IDependencyGraph{T}"/>.
        /// </summary>
        void Clear();
    }
}