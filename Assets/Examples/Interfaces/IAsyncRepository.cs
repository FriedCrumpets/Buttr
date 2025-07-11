using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Buttr.Core {
    public interface IAsyncRepository<in TKey, TData> where TData : IEntity<TKey> {
        Awaitable<ICollection<TData>> RetrieveAllAsync(CancellationToken token);
        Awaitable<IEnumerable<TData>> RetrieveByConditionAsync(Func<TData, bool> condition, CancellationToken token);
        Awaitable CreateAsync(TData entity, CancellationToken token);
        Awaitable<TData> ReadAsync(TKey id, CancellationToken token);
        Awaitable<bool> UpdateAsync(TData entity, CancellationToken token);
        Awaitable<bool> DeleteAsync(TData entity, CancellationToken token);
        Awaitable<bool> DeleteAsync(TKey id, CancellationToken token);
        Awaitable<bool> ClearAsync(CancellationToken token);
    }
}