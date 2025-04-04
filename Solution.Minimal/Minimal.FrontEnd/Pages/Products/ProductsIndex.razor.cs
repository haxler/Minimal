using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Minimal.FrontEnd.Repositories;
using Minimal.Shared.Entities;

namespace Minimal.FrontEnd.Pages.Products;

public partial class ProductsIndex
{
    private int currentPage = 1;
    private int totalPages;

    [Inject] private IRepository Repository { get; set; } = null!;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    private List<Product>? Products { get; set; }
    [Parameter, SupplyParameterFromQuery] public int RecordsNumber { get; set; } = 10;
    [CascadingParameter] private IModalService Modal { get; set; } = default!;

    private async Task SelectedRecordsNumberAsync(int recordsnumber)
    {
        RecordsNumber = recordsnumber;
        int page = 1;
        await LoadAsync(page);
        await SelectedPageAsync(page);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        IModalReference modalReference;

        if (isEdit)
        {
            modalReference = Modal.Show<ProductEdit>(string.Empty, new ModalParameters().Add("Id", id));
        }
        else
        {
            modalReference = Modal.Show<ProductCreate>();
        }

        var result = await modalReference.Result;
        if (result.Confirmed)
        {
            await LoadAsync();
        }
    }

    private void ValidateRecordsNumber(int recordsnumber)
    {
        if (recordsnumber == 0)
        {
            RecordsNumber = 10;
        }
    }

    private async Task FilterCallBack(string filter)
    {
        Filter = filter;
        await ApplyFilterAsync();
        StateHasChanged();
    }

    private async Task SelectedPageAsync(int page)
    {
        currentPage = page;
        await LoadAsync(page);
    }

    private async Task LoadAsync(int page = 1)
    {
        if (!string.IsNullOrWhiteSpace(Page))
        {
            page = Convert.ToInt32(Page);
        }

        var ok = await LoadListAsync(page);
        if (ok)
        {
            await LoadPagesAsync();
        }
    }

    private async Task<bool> LoadListAsync(int page)
    {
        ValidateRecordsNumber(RecordsNumber);
        var url = $"api/products/?page={page}&recordsnumber={RecordsNumber}";
        if (!string.IsNullOrEmpty(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Product>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return false;
        }
        Products = responseHttp.Response;
        return true;
    }

    private async Task LoadPagesAsync()
    {
        ValidateRecordsNumber(RecordsNumber);
        var url = $"api/products/totalPages?recordsnumber ={RecordsNumber}";
        if (!string.IsNullOrEmpty(Filter))
        {
            url += $"?filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        totalPages = responseHttp.Response;
    }

    private async Task CleanFilterAsync()
    {
        Filter = string.Empty;
        await ApplyFilterAsync();
    }

    private async Task ApplyFilterAsync()
    {
        int page = 1;
        await LoadAsync(page);
        await SelectedPageAsync(page);
    }

    private async Task DeleteAsync(Product product)
    {
        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = "Confirmación",
            Text = string.Format("¿Está seguro de borrar el {0}: {1}?", "Productos", product.Name),
            Icon = SweetAlertIcon.Question,
            ShowCancelButton = true,
            CancelButtonText = "Cancelar"
        });

        var confirm = string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/products/{product.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                var mensajeError = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
            }
            return;
        }

        await LoadAsync();
        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        {
            Toast = true,
            Position = SweetAlertPosition.BottomEnd,
            ShowConfirmButton = true,
            Timer = 3000,
            ConfirmButtonText = "SI"
        });
        toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro borrado con éxito.");
    }
}