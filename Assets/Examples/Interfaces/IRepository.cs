using System;
using System.Collections.Generic;

namespace Buttr.Core {
    /// <summary>
    /// Repositories
    /// </summary>
    /// <remarks>
    /// This may seem redundant especially with the example implementations being simple dictionaries obviously this isn't a representation of how this should work.
    /// 
    /// These will want to be
    /// - tied to a central database and/or save load system.
    /// - work alongside services to simply store downloaded data via a key
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface IRepository<in TKey, TData> where TData : IEntity<TKey> {
        ICollection<TData> RetrieveAll();
        IEnumerable<TData> RetrieveByCondition(Func<TData, bool> condition);
        void Create(TData entity);
        TData Read(TKey id);
        void Update(TData entity);
        bool Delete(TData entity);
        bool Delete(TKey id);
        void Clear();
    }
}