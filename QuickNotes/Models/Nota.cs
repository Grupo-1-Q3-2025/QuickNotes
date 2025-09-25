using SQLite;

namespace QuickNotes.Models;

public class Nota
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
}

