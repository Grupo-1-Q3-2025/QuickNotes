namespace QuickNotes.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickNotes.Models;
using QuickNotes.Services;
using System.Collections.ObjectModel;

public partial class NotaViewModel : ObservableObject
{
    readonly NotaService _service;

    [ObservableProperty]
    ObservableCollection<Nota> notaCollection = new();

    [ObservableProperty]
    Nota notaSeleccionada = new();

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

    async Task CargarNotas()
    {
        var notas = await _service.GetNotasAsync();
        NotaCollection = new ObservableCollection<Nota>(notas);
    }
}