using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using MvcBlanketLib.Extensions;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;

namespace MvcBlanketLib.ViewModels
{
    public class PagedViewModel<T>
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 10;

        public ViewDataDictionary ViewData { get; set; }
        public IQueryable<T> Query { get; set; }
        public GridSortOptions GridSortOptions { get; set; }
        public string DefaultSortColumn { get; set; }
        public SortDirection DefaultSortDirection { get; set; }
        public IPagination<T> PagedList { get; private set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public PagedViewModel<T> AddFilter(Expression<Func<T, bool>> predicate)
        {
            Query = Query.Where(predicate);
            return this;
        }

        public PagedViewModel<T> AddFilter<TValue>(string key, TValue value, Expression<Func<T, bool>> predicate)
        {
            ProcessQuery(value, predicate);
            ViewData[key] = value;
            return this;
        }

        public PagedViewModel<T> AddFilter<TValue>(string keyField, object value, Expression<Func<T, bool>> predicate,
             IQueryable<TValue> query, string textField)
        {
            ProcessQuery(value, predicate);
            var selectList = query.ToSelectList(keyField, textField, value);
            ViewData[keyField] = selectList;
            return this;
        }

        public PagedViewModel<T> Setup()
        {
            if (string.IsNullOrWhiteSpace(GridSortOptions.Column))
            {
                GridSortOptions.Column = DefaultSortColumn;
                GridSortOptions.Direction = DefaultSortDirection;
            }

            PagedList = Query.OrderBy(GridSortOptions.Column, GridSortOptions.Direction)
                 .AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize);
            return this;
        }

        public PagedViewModel<T> Setup<TK>(Func<T, TK> order)
        {
            PagedList = GridSortOptions.Direction == SortDirection.Ascending
                ? Query.OrderBy(order).AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize)
                : Query.OrderByDescending(order).AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize);
            return this;
        }

        public PagedViewModel<T> SetupEx(params Expression<Func<T, object>>[] orderLambdas)
        {
            if (orderLambdas == null || !orderLambdas.Any()) return this;
            var first = orderLambdas.First();
            var query = GridSortOptions.Direction == SortDirection.Ascending ? Query.OrderBy(first) : Query.OrderByDescending(first);
            query = orderLambdas.Skip(1).Aggregate(query, (current, next) => GridSortOptions.Direction == SortDirection.Ascending ? current.ThenBy(next) : current.ThenByDescending(next));
            PagedList = query.AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize);
            return this;
        }

        public PagedViewModel<T> SetupEx(params string[] columnNames)
        {
            if (columnNames == null || !columnNames.Any()) return this;
            var first = columnNames.First();
            var query = Query.OrderBy(first, GridSortOptions.Direction, true);
            query = columnNames.Skip(1).Aggregate(query, (current, next) => current.OrderBy(next, GridSortOptions.Direction, false));
            PagedList = query.AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize);
            return this;
        }
        
        public PagedViewModel<T> Skip(IPagination<T> data)
        {
            PagedList = data;
            return this;
        }

        private void ProcessQuery<TValue>(TValue value, Expression<Func<T, bool>> predicate)
        {
            if (value == null) return;
            if (typeof(TValue) == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(value as string)) return;
            }

            Query = Query.Where(predicate);
        }
    }

}
