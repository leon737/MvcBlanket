/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using MvcBlanketLib.PageFilters;

namespace MvcBlanketLibTest.ViewModelTests
{
    public class FakeRepository
    {
        private readonly IEnumerable<FakeEntity> entities;
        public IEnumerable<FakeEntity> Entities { get { return entities; }}

        public FakeRepository()
        {
            entities = new List<FakeEntity>
                           {
                               new FakeEntity { Id = 1, Title = "Book", Price = 10, Status = true, Published = new DateTime(2000, 01, 01)},
                               new FakeEntity { Id = 2, Title = "Lamp", Price = 20, Status = true, Published = new DateTime(2000, 06, 01)},
                               new FakeEntity { Id = 3, Title = "Pen", Price = 5, Status = false, Published = new DateTime(2001, 01, 01)},
                               new FakeEntity { Id = 4, Title = "Notebook", Price = 7, Status = true, Published = new DateTime(2002, 01, 01)},
                               new FakeEntity { Id = 5, Title = "Table", Price = 100, Status = true, Published = new DateTime(1999, 01, 01)},
                               new FakeEntity { Id = 6, Title = "Pencil", Price = 2, Status = true, Published = new DateTime(2001, 04, 01)}
                           };
        }

        public IQueryable<FakeEntity> Get (FakeFiltersModel filters)
        {
            var query = Entities.AsQueryable();
            if (filters == null) return query;
            query = query.Where(filters.Title, c => c.Title.StartsWith(filters.Title.Value))
                .Where(filters.Price, c => c.Price <= filters.Price.Value)
                .Where(filters.Status, c => c.Status == filters.Status.Value)
                .Where(filters.Published,
                       c =>
                       c.Published >= filters.Published.Value.LowerBound &&
                       c.Published <= filters.Published.Value.UpperBound);
            return query;

        }
    }
}
