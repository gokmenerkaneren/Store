using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.DtoLayer.CommonDtos;

namespace Store.Location.Application.Extensions
{
    public static class DataTablesQueryExtensions
    {
        /// <summary>
        /// Applies DataTables dynamic ordering, global search, and server-side pagination.
        /// </summary>
        public static async Task<DataTablesResponse<TDestination>> ToDataTablesResultAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            DataTablesRequest request,
            Func<TSource, TDestination> mapper,
            Func<IQueryable<TSource>, string, IQueryable<TSource>>? customSearchFilter = null,
            Dictionary<string, string>? columnPropertyMappings = null)
        {
            var totalRecords = await query.CountAsync();
            var filteredQuery = query;

            // Global search
            if (!string.IsNullOrWhiteSpace(request.Search?.Value) && customSearchFilter != null)
            {
                filteredQuery = customSearchFilter(filteredQuery, request.Search.Value.Trim());
            }

            var recordsFiltered = await filteredQuery.CountAsync();

            // Dynamic Ordering
            if (request.Order != null && request.Order.Any())
            {
                var orderClauses = new List<string>();

                foreach (var order in request.Order)
                {
                    if (order.Column >= 0 && order.Column < request.Columns.Count)
                    {
                        var col = request.Columns[order.Column];
                        var propertyName = !string.IsNullOrWhiteSpace(col.Data) ? col.Data : col.Name;

                        if (!string.IsNullOrWhiteSpace(propertyName))
                        {
                            // Map DataTables column name to C# entity property if mapping is provided
                            if (columnPropertyMappings != null && columnPropertyMappings.TryGetValue(propertyName, out var mappedName))
                            {
                                propertyName = mappedName;
                            }

                            var dir = order.Dir?.ToLower() == "desc" ? "descending" : "ascending";
                            orderClauses.Add($"{propertyName} {dir}");
                        }
                    }
                }

                if (orderClauses.Any())
                {
                    var orderingString = string.Join(", ", orderClauses);
                    filteredQuery = filteredQuery.OrderBy(orderingString);
                }
            }

            // Pagination
            var start = request.Start >= 0 ? request.Start : 0;
            var length = request.Length > 0 ? request.Length : 10;

            var pagedItems = await filteredQuery
                .Skip(start)
                .Take(length)
                .ToListAsync();

            return new DataTablesResponse<TDestination>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = pagedItems.Select(mapper).ToList()
            };
        }
    }
}
