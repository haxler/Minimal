using Microsoft.AspNetCore.Components;

namespace Minimal.FrontEnd.Shared;

public partial class Filter
{
    [Parameter, SupplyParameterFromQuery] public string TextToFilter { get; set; } = string.Empty;
    [Parameter] public string PlaceHolder { get; set; } = string.Empty;
    [Parameter] public Func<string, Task> Callback { get; set; } = async (text) => await Task.CompletedTask;

    private async Task CleanFilterAsync()
    {
        await Callback(string.Empty);
    }

    private async Task ApplyFilterAsync()
    {
        await Callback(TextToFilter);
    }
}