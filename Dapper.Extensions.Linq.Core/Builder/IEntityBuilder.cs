using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Extensions.Linq.Core.Builder
{
    public interface IEntityBuilder<T> where T : class, IEntity
    {
        bool Any();
        IEnumerable<T> AsEnumerable();
        IList<T> ToList();
        int Count();
        T Single();
        T SingleOrDefault();
        T FirstOrDefault();
        IEntityBuilder<T> Take(int number);
        // add by m8989@qq.com
        IEntityBuilder<T> Where(Expression<Func<T, bool>> predicate = null);

        IEntityBuilder<T> OrderBy(Expression<Func<T, object>> expression);
        IEntityBuilder<T> OrderByDescending(Expression<Func<T, object>> expression);

        /// <summary>
        /// SqlCe cannot have a non zero timeout.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        IEntityBuilder<T> Timeout(int timeout);
        IEntityBuilder<T> Nolock();
    }
}