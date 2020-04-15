using AnamSoft.ReadOnlySet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnamSoft.DependencyGraph
{
    /// <summary>
    /// Generic DependencyGraph
    /// </summary>
    /// <typeparam name="T">A type of the <see cref="DependencyGraph{T}"/> node.</typeparam>
    public class DependencyGraph<T> : IDependencyGraph<T>, ICloneable
    {
        /// <summary>
        /// Dictionary that represents the dependency graph.
        /// The key is a dependent.
        /// The value is a <see cref="HashSet{T}"/> of direct dependencies.
        /// </summary>
        protected readonly Dictionary<T, HashSet<T>> _graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGraph{T}"/> class that is empty.
        /// </summary>
        /// <param name="allowCyclic"></param>
        public DependencyGraph(bool allowCyclic = true)
        {
            _graph = new Dictionary<T, HashSet<T>>();
            AllowCyclic = allowCyclic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGraph{T}"/> class that is empty and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="allowCyclic"></param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
        public DependencyGraph(IEqualityComparer<T> comparer, bool allowCyclic = true)
        {
            if (comparer is null) throw new ArgumentNullException(nameof(comparer));
            _graph = new Dictionary<T, HashSet<T>>(comparer);
            AllowCyclic = allowCyclic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGraph{T}"/> class that uses the same comparer as specified graph and contains elements copied from the specified graph.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="allowCyclic"></param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        public DependencyGraph(DependencyGraph<T> other, bool allowCyclic = true)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));

            if (!allowCyclic && other.HasCyclic())
                throw new ArgumentException("Other graph has cyclic dependencies that this graph does not allow", nameof(other));

            AllowCyclic = allowCyclic;

            _graph = new Dictionary<T, HashSet<T>>(other._graph.Count, other._graph.Comparer);
            foreach (var item in other._graph)
                _graph.Add(item.Key, new HashSet<T>(item.Value, other._graph.Comparer));
        }

        /// <summary>
        /// Determines if the <see cref="DependencyGraph{T}"/> is empty.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="DependencyGraph{T}"/> is empty; otherwise, <see langword="false"/>.</value>
        public bool IsEmpty => _graph.Count == 0;

        /// <summary>
        /// Returns <see langword="true"/> if the <see cref="DependencyGraph{T}"/> allows cyclic dependencies.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="DependencyGraph{T}"/> allows cyclic dependencies; otherwise, <see langword="false"/>.</value>
        public bool AllowCyclic { get; }

        /// <summary>
        /// Adds new dependency to the <see cref="DependencyGraph{T}"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="AllowCyclic"/> it <see langword="false"/> and new dependency will cause cyclic, the dependency will not be added and the <see langword="return"/> value will be <see langword="false"/>
        /// </remarks>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the dependency was added; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dependent"/> is equal to <paramref name="dependency"/>.</exception>
        public virtual bool AddDependency(T dependent, T dependency)
        {
            if (dependent is null) throw new ArgumentNullException(nameof(dependent));
            if (dependency is null) throw new ArgumentNullException(nameof(dependency));
            if (dependent.Equals(dependency))
                throw new ArgumentException($"{nameof(dependent)} cannot depends on itself", nameof(dependency));

            if (!AllowCyclic && IsDependsTrusted(dependency, dependent))  // opposite dependency
                return false;

            if (_graph.TryGetValue(dependent, out var dependencies))
                return dependencies.Add(dependency);

            _graph.Add(dependent, new HashSet<T>(_graph.Comparer) { dependency });
            return true;
        }

        /// <summary>
        /// Removes a dependency from the <see cref="DependencyGraph{T}"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the dependency was removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        public virtual bool RemoveDependency(T dependent, T dependency)
        {
            if (dependent is null) throw new ArgumentNullException(nameof(dependent));
            if (dependency is null) throw new ArgumentNullException(nameof(dependency));

            if (!_graph.TryGetValue(dependent, out var dependencies) || !dependencies.Remove(dependency))
                return false;

            if (dependencies.Count == 0)
                _graph.Remove(dependent);

            return true;
        }

        /// <summary>
        /// Determines if a <paramref name="dependent"/> directly depends on a <paramref name="dependency"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the <paramref name="dependent"/> directly depends on <paramref name="dependency"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        public bool IsDirectlyDepends(T dependent, T dependency)
        {
            if (dependent is null) throw new ArgumentNullException(nameof(dependent));
            if (dependency is null) throw new ArgumentNullException(nameof(dependency));
            return _graph.TryGetValue(dependent, out var dependenciew) && dependenciew.Contains(dependency);
        }

        /// <summary>
        /// Determines if a <paramref name="dependent"/> depends on a <paramref name="dependency"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <param name="dependency">The dependency</param>
        /// <returns><see langword="true"/> if the <paramref name="dependent"/> depends on <paramref name="dependency"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> or <paramref name="dependency"/> is <see langword="null"/>.</exception>
        public bool IsDepends(T dependent, T dependency)
        {
            if (dependent is null) throw new ArgumentNullException(nameof(dependent));
            if (dependency is null) throw new ArgumentNullException(nameof(dependency));
            return IsDependsTrusted(dependent, dependency);
        }

        private bool IsDependsTrusted(T dependent, T dependency)
            => _graph.TryGetValue(dependent, out var dependencies) && (dependencies.Contains(dependency) || dependencies.Any(o => IsDependsTrusted(o, dependency)));

        /// <summary>
        /// Gets all direct dependencies of <paramref name="dependent"/>.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <returns><see cref="ReadOnlySet{T}"/> that contains all direct dependencies of <paramref name="dependent"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> is <see langword="null"/>.</exception>
        public virtual ReadOnlySet<T> GetDirectDependencies(T dependent)
        {
            if (dependent is null) throw new ArgumentNullException(nameof(dependent));
            return _graph.TryGetValue(dependent, out var dependencies) ? dependencies.AsReadOnlySet() : ReadOnlySet<T>.Empty;
        }

        IReadOnlySet<T> IDependencyGraph<T>.GetDirectDependencies(T dependent) => GetDirectDependencies(dependent);

        /// <summary>
        /// Gets all dependencies of <paramref name="dependent"/> including indirect.
        /// </summary>
        /// <param name="dependent">The dependent</param>
        /// <returns><see cref="IReadOnlySet{T}"/> that contains all dependencies of <paramref name="dependent"/> including indirect.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependent"/> is <see langword="null"/>.</exception>
        public ReadOnlySet<T> GetAllDependencies(T dependent)
        {
            if (dependent is null)
                throw new ArgumentNullException(nameof(dependent));

            if (!_graph.TryGetValue(dependent, out var dependencies) || dependencies.Count == 0)
                return ReadOnlySet<T>.Empty;

            dependencies = new HashSet<T>(dependencies, dependencies.Comparer);

            var processed = new HashSet<T>(dependencies.Comparer) { dependent };
            var stack = new Stack<T>(dependencies);
            while (stack.Count > 0)
            {
                var dependency = stack.Pop();
                if (!processed.Contains(dependency))
                {
                    var subDependees = GetDirectDependencies(dependency);

                    dependencies.UnionWith(subDependees);

                    foreach (var subDependee in subDependees)
                        stack.Push(subDependee);

                    processed.Add(dependency);
                }
            }

            return dependencies.AsReadOnlySet();
        }

        IReadOnlySet<T> IDependencyGraph<T>.GetAllDependencies(T dependent) => GetAllDependencies(dependent);

        /// <summary>
        /// Determines if the <see cref="IDependencyGraph{T}"/> has cyclic dependencies.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="IDependencyGraph{T}"/> has cyclic dependencies; otherwise, <see langword="false"/>.</returns>
        public bool HasCyclic() => AllowCyclic && _graph.Count > 1 && _graph.Keys.Any(dependent => IsDependsTrusted(dependent, dependent));

        /// <summary>
        /// Clears the <see cref="IDependencyGraph{T}"/>.
        /// </summary>
        public virtual void Clear()
        {
            _graph.Clear();
        }

        /// <summary>
        /// Clones the current <see cref="DependencyGraph{T}"/>.
        /// </summary>
        /// <returns>The cloned instance of <see cref="DependencyGraph{T}"/>.</returns>
        public DependencyGraph<T> Clone() => new DependencyGraph<T>(this, AllowCyclic);
        object ICloneable.Clone() => Clone();
    }
}
