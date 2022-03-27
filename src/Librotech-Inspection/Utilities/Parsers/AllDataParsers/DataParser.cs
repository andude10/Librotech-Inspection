using System.Threading.Tasks;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.AllDataParsers;

public abstract class DataParser
{
    public abstract Task<Data?> ParseAsync(string path);
}