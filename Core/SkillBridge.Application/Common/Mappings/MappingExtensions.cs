using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TElement>> ToPaginatedListAsync<TElement>(
        this IQueryable<TElement> queryable,
        int pageNumber,
        int pageSize)
        where TElement : class
        => PaginatedList<TElement>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
}
