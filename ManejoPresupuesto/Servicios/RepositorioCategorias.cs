using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacion);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectonString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            connectonString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectonString);
            var id = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                Values (@Nombre, @TipoOperacionId, @UsuarioId);

                SELECT SCOPE_IDENTITY();
                ", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
        {
            using var conecction = new SqlConnection(connectonString);
            return await conecction.QueryAsync<Categoria>(
                "SELECT * FROM Categorias WHERE UsuarioId = @usuarioId", new {usuarioId});
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var conecction = new SqlConnection(connectonString);
            return await conecction.QueryAsync<Categoria>(
                @"SELECT * 
                    FROM Categorias 
                    WHERE UsuarioId = @usuarioId AND TipoOperacionId = @TipoOperacionId", 
                new { usuarioId, tipoOperacionId });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectonString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(
                @"Select * FROM Categorias WHERE Id = @Id AND UsuarioId = @UsuarioId",
                new { id, usuarioId });
        }

        public async Task  Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectonString);
            await connection.ExecuteAsync(@"UPDATE Categorias
                SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                WHERE Id = @Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectonString);
            await connection.ExecuteAsync("DELETE Categorias WHERE Id = @Id", new { id });
        }
    }
}
