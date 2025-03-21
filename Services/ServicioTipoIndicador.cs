using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FrontBlazor.Services
{
    public class ServicioTipoIndicador
    {
        private readonly HttpClient _clienteHttp;
        private readonly string baseUrl = "http://localhost:5239";
        private readonly JsonSerializerOptions _opcionesJson;

        public ServicioTipoIndicador(HttpClient clienteHttp)
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
                var url = $"{baseUrl}/tipoindicador";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        public async Task<bool> CrearAsync(Dictionary<string, object> entidad)
        {
            try
            {
                var url = $"{baseUrl}/tipoindicador";
                var contenido = new StringContent(JsonSerializer.Serialize(entidad, _opcionesJson), Encoding.UTF8, "application/json");
                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear entidad: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(string id, Dictionary<string, object?> datos)
        {
            try
            {
                var url = $"{baseUrl}/tipoindicador/{id}";
                var contenido = new StringContent(JsonSerializer.Serialize(datos, _opcionesJson), Encoding.UTF8, "application/json");
                var respuesta = await _clienteHttp.PutAsync(url, contenido);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar entidad con ID {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarAsync(string id)
        {
            try
            {
                var url = $"{baseUrl}/tipoindicador/{id}";
                var respuesta = await _clienteHttp.DeleteAsync(url);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar entidad con ID {id}: {ex.Message}");
                return false;
            }
        }
    }
}
