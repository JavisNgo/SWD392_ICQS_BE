using SWD_ICQS.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWD_ICQS.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
            );

        TEntity GetByID(object id);

        void Insert(TEntity entity);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);
        bool Any(Expression<Func<TEntity, bool>> predicate);

    }
}
