using System;

namespace LibrotechInspection.Desktop.Utilities.Exceptions;

[Serializable]
public class NoServiceFound : Exception
{
    public NoServiceFound() : base("The Locator did not find service")
    {
    }

    public NoServiceFound(string serviceInterface) : base(
        $"The Locator did not find any service with interface {serviceInterface}")
    {
    }

    public NoServiceFound(string serviceInterface, Exception inner) : base(
        $"The Locator did not find any service with interface {serviceInterface}", inner)
    {
    }
}