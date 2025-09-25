namespace QuickNotes.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickNotes.Models;
using QuickNotes.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

public partial class NotaViewModel : ObservableObject
{
    readonly NotaService _service;

    [ObservableProperty]
    ObservableCollection<Nota> notaCollection = new();

    [ObservableProperty]
    ObservableCollection<Nota> filteredNotaCollection = new();

    [ObservableProperty]
    Nota notaSeleccionada = new();

    [ObservableProperty]
    string searchText;

    public NotaViewModel()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notas.db3");
        _service = new NotaService(dbPath);
        CargarNotas();
    }

    [RelayCommand]
    async void GuardarNota()
    {
        if (string.IsNullOrWhiteSpace(NotaSeleccionada.Titulo) && string.IsNullOrWhiteSpace(NotaSeleccionada.Descripcion))
            return;

        if (NotaSeleccionada.Id == 0)
            NotaSeleccionada.FechaCreacion = DateTime.Now;

        await _service.SaveNotaAsync(NotaSeleccionada);
        await CargarNotas();
        NotaSeleccionada = new Nota();
    }

    [RelayCommand]
    void CrearNota()
    {
        NotaSeleccionada = new Nota();
    }

    [RelayCommand]
    async void EliminarNota()
    {
        if (NotaSeleccionada?.Id > 0)
        {
            await _service.DeleteNotaAsync(NotaSeleccionada);
            await CargarNotas();
            NotaSeleccionada = new Nota();
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        AplicarFiltro();
    }

    void AplicarFiltro()
    {
        if (notaCollection is null) return;

        var criterio = (searchText ?? string.Empty).Trim();
        if (criterio.Length == 0)
        {
            FilteredNotaCollection = new ObservableCollection<Nota>(notaCollection);
            return;
        }

        var filtradas = notaCollection.Where(n =>
            (!string.IsNullOrEmpty(n.Titulo) && n.Titulo.Contains(criterio, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(n.Descripcion) && n.Descripcion.Contains(criterio, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        FilteredNotaCollection = new ObservableCollection<Nota>(filtradas);
    }

    async Task CargarNotas()
    {
        var notas = await _service.GetNotasAsync();
        NotaCollection = new ObservableCollection<Nota>(notas);
        AplicarFiltro();
    }
}
