using System;
using System.Collections.Generic;
using System.Linq;

namespace Buttr.Core {
    /// <summary>
    /// An example repository for CRUD operations
    /// </summary>
    /// <remarks>
    /// a repository should definitily be more than just a dictionary, but it's always good to keep an example around ay.
    /// Repositories should be an entry point into a larger whole. A database. Define said database as you wish.
    /// </remarks>
    /// <typeparam name="TKey">The Key used to access data</typeparam>
    /// <typeparam name="TData">The data to be accessed</typeparam>
    public abstract class RepositoryBase<TKey, TData> : IRepository<TKey, TData> where TData : IEntity<TKey> {
        private readonly object @lock = new();
        private readonly IDictionary<TKey, TData> dataStore = new Dictionary<TKey, TData>();

        public ICollection<TData> RetrieveAll() {
            lock (@lock) return dataStore.Values;
        }

        public IEnumerable<TData> RetrieveByCondition(Func<TData, bool> condition) {
            lock (@lock) return dataStore.Values.Where(condition.Invoke);
        }

        public void Create(TData entity) {
            lock (@lock) dataStore[entity.ID] = entity;
        }

        public TData Read(TKey id) {
            lock (@lock) return dataStore.TryGetValue(id, out var data) ? data : default;
        }

        public void Update(TData entity) {
            lock (@lock) {
                if (dataStore.ContainsKey(entity.ID))
                    dataStore[entity.ID] = entity;
            }
        }

        public bool Delete(TData entity) {
            lock (@lock) {
                return dataStore.Remove(entity.ID);
            }
        }

        public bool Delete(TKey id) {
            lock (@lock) {
                return dataStore.Remove(id);
            }
        }

        public void Clear() {
            lock (@lock) dataStore.Clear();
        }
    }
}