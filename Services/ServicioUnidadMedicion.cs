using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FrontBlazor.Services
{
    public class ServicioUnidadMedicion
    {
        private readonly HttpClient _clienteHttp;
        private readonly string baseUrl = "http://localhost:5239";
        private readonly JsonSerializerOptions _opcionesJson;

        public ServicioUnidadMedicion(HttpClient clienteHttp)
        {
            _clienteHttp = clienteHttp;
            _opcionesJson = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true, 
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
            };
        }

        public async Task<List<Dictionary<string, object>>?> ObtenerTodosAsync()
        {
            try
            {
                var url = $"{baseUrl}/unidadmedicion";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al obtener datos: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener datos: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        public async Task<bool> CrearAsync(Dictionary<string, object> entidad)
        {
            try
            {
                var url = $"{baseUrl}/unidadmedicion";
                var contenido = new StringContent(JsonSerializer.Serialize(entidad, _opcionesJson), Encoding.UTF8, "application/json");
                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                respuesta.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al crear entidad: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al crear entidad: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(string id, Dictionary<string, object?> datos)
        {
            try
            {
                var url = $"{baseUrl}/unidadmedicion/{id}";
                var contenido = new StringContent(JsonSerializer.Serialize(datos, _opcionesJson), Encoding.UTF8, "application/json");
                var respuesta = await _clienteHttp.PutAsync(url, contenido);
                respuesta.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al actualizar entidad con ID {id}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al actualizar entidad con ID {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarAsync(string id)
        {
            try
            {
                var url = $"{baseUrl}/unidadmedicion/{id}";
                var respuesta = await _clienteHttp.DeleteAsync(url);
                respuesta.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al eliminar entidad con ID {id}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al eliminar entidad con ID {id}: {ex.Message}");
                return false;
            }
        }
    }
}
