namespace LibrotechInspection.Core.Models;

/// <summary>
///     Stamp represents a timestamp from a file
/// </summary>
public struct Stamp
{
    public Stamp(string name, IEnumerable<StampItem> items)
    {
        Name = name;
        Items = items;
    }

    public string Name { get; set; }
    public IEnumerable<StampItem> Items { get; set; }
}

public struct StampItem
{
    public string Name { get; set; }
    public string Value { get; set; }
}