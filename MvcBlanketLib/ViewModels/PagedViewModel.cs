using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using MvcBlanketLib.Extensions;
using MvcBlanketLib.PageFilters;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;

namespace MvcBlanketLib.ViewModels
{
    public class PagedViewModel<T>
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 10;

        private ViewDataDictionary viewData;
        public ViewDataDictionary ViewData
        {
            get { return viewData ?? ControllerContext.Controller.ViewData; }
            set { viewData = value; }
        }
        public ControllerContext ControllerContext { get; set; }
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

        public PagedViewModel<T> Apply(IQueryable<T> data)
        {
            Query = data;
            return this;
        }

        public PagedViewModel<T> Apply(Func<Dictionary<string, string>, IQueryable<T>> selector)
        {
            Query = selector(ViewData["FiltersModel"] as Dictionary<string, string>);
            return this;
        }

        public PagedViewModel<T> Apply(Func<IPageFiltersModel, IQueryable<T>> selector)
        {
            Query = selector(ViewData["FiltersModel"] as IPageFiltersModel);
            return this;
        }

        public PagedViewModel<T> Apply<TS>(Func<TS, IQueryable<T>> selector)
        {
            Query = selector((TS)(ViewData["FiltersModel"]));
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

        public PagedViewModel<T> SetupByExpressions(params Expression<Func<T, object>>[] orderLambdas)
        {
            if (orderLambdas == null || !orderLambdas.Any()) return this;
            var first = orderLambdas.First();
            var query = GridSortOptions.Direction == SortDirection.Ascending ? Query.OrderBy(first) : Query.OrderByDescending(first);
            query = orderLambdas.Skip(1).Aggregate(query, (current, next) => GridSortOptions.Direction == SortDirection.Ascending ? current.ThenBy(next) : current.ThenByDescending(next));
            PagedList = query.AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize);
            return this;
        }

        public PagedViewModel<T> SetupByNames(params string[] columnNames)
        {

            if (string.IsNullOrWhiteSpace(GridSortOptions.Column))
            {
                GridSortOptions.Column = DefaultSortColumn;
                GridSortOptions.Direction = DefaultSortDirection;
            }

            if (columnNames == null || !columnNames.Any())
            {
                columnNames = new[] {GridSortOptions.Column};
            }
            IEnumerable<SortMapping> mappings = ControllerContext != null ? GetSortMapping() : null;
            
            IQueryable<T> query;
            var first = columnNames.First();
            if (mappings != null)
            {
                var cn = SortMapping.GetColumnNames(mappings, first).ToList();
                query = Query.OrderBy(cn.First(), GridSortOptions.Direction, true);
                foreach(var c in cn.Skip(1))
                    query = query.OrderBy(c, GridSortOptions.Direction, false);
            }
            else
            {
                query = Query.OrderBy(first, GridSortOptions.Direction, true);
            }

            query = columnNames.Skip(1).Aggregate(query, (current, next) =>
                                                         {
                                                             if (mappings != null)
                                                             {
                                                                 var cn = SortMapping.GetColumnNames(mappings, next);
                                                                 current = cn.Aggregate(current, (current1, c) => current1.OrderBy(c, GridSortOptions.Direction, false));
                                                             }
                                                             else
                                                                 current = current.OrderBy(next, GridSortOptions.Direction, false);
                                                             return current;
                                                         });
            PagedList = query.AsPagination(Page ?? DefaultPageNumber, PageSize ?? DefaultPageSize);
            return this;
        }

        private IEnumerable<SortMapping> GetSortMapping()
        {
            var mapping = ControllerContext.HttpContext.Items[SortMapping.ContentItemName] as IEnumerable<SortMapping>;
            return mapping;
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

    internal class SortMapping
    {
        public const string ContentItemName = "__sortmapping";

        public string CommonName { get; set; }
        public IEnumerable<string> ColumnNames { get; set; }

        public static IEnumerable<string> GetColumnNames(IEnumerable<SortMapping> mappings, string commonName)
        {
            var bestFit = mappings.FirstOrDefault(m => m.CommonName == commonName);
            if (bestFit != null) return bestFit.ColumnNames;
            return new[] {commonName};
        }
    }
}
