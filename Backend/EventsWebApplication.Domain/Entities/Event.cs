namespace EventsWebApplication.Domain.Entities;
public class Event
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime DateTime { get; private set; }
    public string Location { get; private set; }
    public string Category { get; private set; }
    public int MaxUsers { get; private set; }
    public ICollection<User> Users { get; private set; }
    
    public int AvailableSeats => MaxUsers - Users.Count;
    public string ImageUrl { get; private set; }

    public void RegisterUser(User user)
    {
        Users.Add(user);
    }

    public Event(Guid id, string name, string description, DateTime dateTime, string location, string category, int maxUsers)
    {
        Id = id;
        Name = name;
        Description = description;
        DateTime = dateTime;
        Location = location;
        Category = category;
        MaxUsers = maxUsers;
        Users = new List<User>();
    }

    public void AddImage(string imageUrl) 
    {
        ImageUrl = imageUrl;
    }
}

