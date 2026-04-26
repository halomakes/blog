namespace Halomakes.Blog.Services;

public class FilamentTrackerService
{
    public uint Total { get; private set; }

    public void Use(uint amount)
    {
        Total += amount;
    }
}