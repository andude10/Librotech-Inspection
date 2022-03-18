using System.Collections.Generic;

namespace Librotech_Inspection.Models;

/// <summary>
///     Stamp represents a timestamp from a file
/// </summary>
public class Stamp
{
    public Stamp(string stampName, IEnumerable<StampItem> items)
    {
        StampName = stampName;
        Items = items;
    }

    public string StampName { get; set; }
    public IEnumerable<StampItem> Items { get; set; }
}

public class StampItem
{
    public string Name { get; set; }
    public string Value { get; set; }
}