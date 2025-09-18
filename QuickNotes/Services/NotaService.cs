namespace QuickNotes.Services;
using QuickNotes.Models;
using SQLite;

public class NotaService
{
    readonly SQLiteAsyncConnection _db;

    public NotaService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<Nota>().Wait();
    }

    public Task<List<Nota>> GetNotasAsync() =>
        _db.Table<Nota>().OrderByDescending(n => n.FechaCreacion).ToListAsync();

    public Task<int> SaveNotaAsync(Nota nota)
    {
        if (nota.Id != 0)
            return _db.UpdateAsync(nota);
        return _db.InsertAsync(nota);
    }

    public Task<int> DeleteNotaAsync(Nota nota) =>
        _db.DeleteAsync(nota);
}