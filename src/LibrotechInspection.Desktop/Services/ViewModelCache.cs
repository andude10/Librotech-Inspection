using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Desktop.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace LibrotechInspection.Desktop.Services;

public class ViewModelCache : IViewModelCache
{
    public ViewModelCache()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    public async Task<ViewModelBase> GetOrCreate(Type viewModelType, Func<ViewModelBase> createViewModel)
    {
        _cache.TryGetValue(viewModelType, out string fileName);

        if (fileName is not null) return await Get(fileName, viewModelType);

        Logger.Debug($"Cache not found for ViewModel of type '{viewModelType}', creating a new cache");
        var cacheViewModel = createViewModel();
        await Create(cacheViewModel);

        return cacheViewModel;
    }

    public async Task<ViewModelBase> Save(ViewModelBase viewModel)
    {
        _cache.Remove(viewModel.GetType());
        await Create(viewModel);

        return viewModel;
    }

    private async Task Create(ViewModelBase viewModelBase)
    {
        var type = viewModelBase.GetType();
        var fileName = $"{type.Name}.json";

        await using var createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, viewModelBase, viewModelBase.GetType(), _serializerOptions);
        await createStream.DisposeAsync();
        File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.Hidden);
        
        _cache.Set(type, fileName);
    }

    private async Task<ViewModelBase> Get(string fileName, Type viewModelBase)
    {
        await using var openStream = File.OpenRead(fileName);
        var viewModel = await JsonSerializer.DeserializeAsync(openStream, viewModelBase, _serializerOptions)
            as ViewModelBase;
        await openStream.DisposeAsync();

        if (viewModel is null)
        {
            const string message = "An error occurred while getting the ViewModel from the cache";
            Logger.Error(message);
            throw new Exception(message);
        }

        return viewModel;
    }

#region Fields

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly JsonSerializerOptions _serializerOptions;

#endregion
}