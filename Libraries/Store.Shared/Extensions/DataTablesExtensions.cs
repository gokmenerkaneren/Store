using System;
using System.Linq;
using System.Linq.Expressions;
using Store.Shared.Dtos;

namespace Store.Shared.Extensions
{
    public static class DataTablesExtensions
    {
        public static IQueryable<T> ApplyDataTables<T>(this IQueryable<T> query, DataTablesRequest request, out int totalCount, out int filteredCount)
        {
            totalCount = query.Count();

            // 1. Arama (Search) - Global Search
            if (request.Search != null && !string.IsNullOrWhiteSpace(request.Search.Value))
            {
                var searchValue = request.Search.Value.ToLower();
                var param = Expression.Parameter(typeof(T), "x");
                Expression combined = null;

                foreach (var col in request.Columns.Where(c => c.Searchable && !string.IsNullOrEmpty(c.Data)))
                {
                    var prop = typeof(T).GetProperty(col.Data, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null && prop.PropertyType == typeof(string))
                    {
                        var member = Expression.Property(param, prop);
                        
                        // toLower
                        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        var toLowerCall = Expression.Call(member, toLowerMethod);

                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var constant = Expression.Constant(searchValue, typeof(string));
                        var containsCall = Expression.Call(toLowerCall, containsMethod, constant);

                        if (combined == null)
                        {
                            combined = containsCall;
                        }
                        else
                        {
                            combined = Expression.OrElse(combined, containsCall);
                        }
                    }
                }

                if (combined != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(combined, param);
                    query = query.Where(lambda);
                }
            }

            filteredCount = query.Count();

            // 2. Sıralama (Order)
            bool isOrdered = false;

            // Önce IsDeleted ve IsActive varsa onlara göre sırala (Aktif -> Pasif -> Silinmiş)
            var propIsDeleted = typeof(T).GetProperty("IsDeleted", System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var propIsActive = typeof(T).GetProperty("IsActive", System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (propIsDeleted != null && propIsActive != null)
            {
                var param = Expression.Parameter(typeof(T), "x");
                
                // OrderBy(x => x.IsDeleted)
                var memberIsDeleted = Expression.Property(param, propIsDeleted);
                var lambdaIsDeleted = Expression.Lambda(memberIsDeleted, param);
                var resultExpIsDeleted = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { typeof(T), propIsDeleted.PropertyType }, query.Expression, Expression.Quote(lambdaIsDeleted));
                query = query.Provider.CreateQuery<T>(resultExpIsDeleted);

                // ThenByDescending(x => x.IsActive)
                var memberIsActive = Expression.Property(param, propIsActive);
                var lambdaIsActive = Expression.Lambda(memberIsActive, param);
                var resultExpIsActive = Expression.Call(typeof(Queryable), "ThenByDescending", new Type[] { typeof(T), propIsActive.PropertyType }, query.Expression, Expression.Quote(lambdaIsActive));
                query = query.Provider.CreateQuery<T>(resultExpIsActive);

                isOrdered = true;
            }

            if (request.Order != null && request.Order.Any())
            {
                var order = request.Order.First();
                var colIndex = order.Column;
                var col = request.Columns.ElementAtOrDefault(colIndex);
                if (col != null && col.Orderable && !string.IsNullOrEmpty(col.Data))
                {
                    var param = Expression.Parameter(typeof(T), "x");
                    var prop = typeof(T).GetProperty(col.Data, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null)
                    {
                        var member = Expression.Property(param, prop);
                        var lambda = Expression.Lambda(member, param);

                        string methodName;
                        if (isOrdered)
                        {
                            methodName = order.Dir.ToLower() == "desc" ? "ThenByDescending" : "ThenBy";
                        }
                        else
                        {
                            methodName = order.Dir.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                        }

                        var resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), prop.PropertyType }, query.Expression, Expression.Quote(lambda));
                        query = query.Provider.CreateQuery<T>(resultExp);
                    }
                }
            }

            // 3. Sayfalama (Pagination)
            if (request.Length > 0)
            {
                query = query.Skip(request.Start).Take(request.Length);
            }

            return query;
        }
    }
}
