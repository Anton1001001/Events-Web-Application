using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Extensions;

public static class EventFilterExtensions
{
    public static IQueryable<Event> FilterByTitle(this IQueryable<Event> query, string? title)
    {
        if (title is not null)
        {
            query = query.Where(e => e.Name.ToLower().Contains(title.ToLower()));
        }

        return query;
    }

    public static IQueryable<Event> FilterByDate(this IQueryable<Event> query, DateTime? date)
    {
        if (date is not null)
        {
            query = query.Where(e => e.DateTime == date);
        }

        return query;
    }

    public static IQueryable<Event> FilterByLocation(this IQueryable<Event> query, string? location)
    {
        if (location is not null)
        {
            query = query.Where(e => e.Location.ToLower().Contains(location.ToLower()));
        }

        return query;
    }

    public static IQueryable<Event> FilterByCategory(this IQueryable<Event> query, string? category)
    {
        if (category is not null)
        {
            query = query.Where(e => e.Category.ToLower() == category.ToLower());
        }

        return query;
    }
    
}