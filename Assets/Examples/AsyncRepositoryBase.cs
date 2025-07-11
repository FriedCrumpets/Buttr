using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Buttr.Core {
    public abstract class AsyncRepositoryBase<TKey, TData> : IAsyncRepository<TKey, TData> where TData : IEntity<TKey> {
        protected readonly object @lock = new();
        protected readonly IDictionary<TKey, TData> dataStore = new Dictionary<TKey, TData>();

        public Awaitable<ICollection<TData>> RetrieveAllAsync(CancellationToken token) {
            lock (@lock) return AwaitableUtility.FromResult(dataStore.Values);
        }

        public Awaitable<IEnumerable<TData>> RetrieveByConditionAsync(Func<TData, bool> condition, CancellationToken token) {
            lock (@lock) return AwaitableUtility.FromResult(dataStore.Values.Where(condition.Invoke));
        }

        public Awaitable CreateAsync(TData entity, CancellationToken token) {
            lock (@lock) dataStore[entity.ID] = entity;
            return AwaitableUtility.CompletedTask;
        }

        public Awaitable<TData> ReadAsync(TKey id, CancellationToken token) {
            lock (@lock) return AwaitableUtility.FromResult(dataStore.TryGetValue(id, out var data) ? data : default);
        }

        public Awaitable<bool> UpdateAsync(TData entity, CancellationToken token) {
            lock (@lock) {
                var exists = dataStore.ContainsKey(entity.ID);
                
                if (exists) {
                    dataStore[entity.ID] = entity;
                }

                return AwaitableUtility.FromResult(exists);
            }
        }

        public Awaitable<bool> DeleteAsync(TData entity, CancellationToken token) {
            lock (@lock) {
                return AwaitableUtility.FromResult(dataStore.Remove(entity.ID));
            }
        }

        public Awaitable<bool> DeleteAsync(TKey id, CancellationToken token) {
            lock (@lock) {
                return AwaitableUtility.FromResult(dataStore.Remove(id));
            }
        }

        public Awaitable<bool> ClearAsync(CancellationToken token) {
            lock (@lock) dataStore.Clear();
            return AwaitableUtility.FromResult(true);
        }
    }
}