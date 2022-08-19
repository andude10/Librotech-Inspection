using System;

namespace LibrotechInspection.Desktop.Utilities.Exceptions;

[Serializable]
public class NoServiceFound : Exception
{
    public NoServiceFound() : base() { }
    public NoServiceFound(string message) : base(message) { }
    public NoServiceFound(string message, Exception inner) : base(message, inner) { }
}