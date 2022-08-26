using System;
using System.Threading.Tasks;
using LibrotechInspection.Desktop.ViewModels;

namespace LibrotechInspection.Desktop.Services;

public interface IViewModelCache
{
    public Task<ViewModelBase> GetOrCreate(Type viewModelType, Func<ViewModelBase> createViewModel);

    public Task<ViewModelBase> Save(ViewModelBase viewModel);
}