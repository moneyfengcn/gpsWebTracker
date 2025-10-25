using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{
    static public class EntityFrameworkExtension
    {
        static public IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> expression)
        {
            return condition ? source.Where(expression) : source;
        }

        public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData)
        {
            var methodToCall = typeof(EntityFrameworkExtension)
                ?.GetMethod(nameof(GetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(entityData.ClrType);

            var filter = methodToCall?.Invoke(null, new object[] { });

            if (filter != null)
            {
                entityData.SetQueryFilter((LambdaExpression)filter);
            }
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : class, ISoftDelete
        {
            Console.WriteLine("软删除:{0}", typeof(TEntity).Name);
            Expression<Func<TEntity, bool>> filter = x => x.IsDeleted == 0;
            return filter;
        }
    }
}
