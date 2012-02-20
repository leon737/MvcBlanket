/*******************************************************************\
* Module Name: Lt.Helpers
* 
* File Name: ViewModels/PagedViewModel.cs
*
* Warnings:
*
* Issues:
*
* Created:  09 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/

using System;
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
				 .AsPagination(Page ?? 1, PageSize ?? 10);
			return this;
		}

		public PagedViewModel<T> Setup<K> (Func<T,K> order)
		{
			if (GridSortOptions.Direction == SortDirection.Ascending)
				PagedList = Query.OrderBy(order).AsPagination(Page ?? 1, PageSize ?? 10);
			else
				PagedList = Query.OrderByDescending(order).AsPagination(Page ?? 1, PageSize ?? 10);
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
