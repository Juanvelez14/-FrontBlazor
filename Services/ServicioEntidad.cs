using System.Net.Http;
using System.Net.Http.Json;     // Importar esta librería para usar métodos que ayudan a trabajar con JSON en solicitudes HTTP
using System.Text;              // Necesario para trabajar con codificación de texto (UTF-8 para JSON)
using System.Text.Json;         // Proporcionar funcionalidad para serializar y deserializar JSON
using Microsoft.JSInterop;


namespace FrontBlazor.Services  // Definir el espacio de nombres donde se ubicará esta clase
{
    /// <summary>
    /// Servicio genérico para realizar operaciones CRUD con cualquier entidad a través de la API genérica.
    /// </summary>
    public class ServicioEntidad // Declarar una clase pública llamada ServicioEntidad
    {
        private readonly HttpClient _clienteHttp;       // Cliente HTTP que se usará para comunicarse con la API
        private readonly string baseUrl = "http://localhost:5239";
        private readonly JsonSerializerOptions _opcionesJson;  // Opciones para configurar cómo se serializa/deserializa el JSON
        private readonly IJSRuntime _js;


        // Constructor: se ejecuta cuando se crea una instancia de esta clase
        public ServicioEntidad(HttpClient clienteHttp, IJSRuntime js)   // Recibir un HttpClient como parámetro mediante inyección de dependencias
        {
            _clienteHttp = clienteHttp;    // Guardar el HttpClient recibido para usarlo en los métodos
            _js = js;

            // Configurar las opciones para el serializador JSON
            _opcionesJson = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true   // Permitir que coincidan propiedades aunque tengan diferente capitalización 
                                                     // (por ejemplo: "Nombre" coincidirá con "nombre")
            };
        }

        /// <summary>
        /// Obtiene todas las entidades de una tabla específica.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla a consultar.</param>
        public async Task<List<Dictionary<string, object>>?> ObtenerTodosAsync(string nombreProyecto, string nombreTabla)
        {
            // Esta función devuelve una lista de diccionarios, donde cada diccionario representa una fila de la tabla
            // Cada diccionario tiene como clave el nombre de la columna y como valor el dato de esa columna

            try  // Intentar ejecutar el código dentro de este bloque
            {
                var url = $"{nombreProyecto}/{nombreTabla}";  // Construir la URL usando interpolación de strings

                // Realizar la petición GET y convertir la respuesta JSON a una lista de diccionarios
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url);
            }
            catch (Exception ex)  // Si ocurre algún error, capturar la excepción
            {
                // Mostrar el error en la consola para depuración
                Console.WriteLine($"Error al obtener datos: {ex.Message}");

                // Devolver una lista vacía en lugar de null para evitar errores en la interfaz
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Obtiene una entidad por su clave primaria.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla a consultar.</param>
        /// <param name="nombreClave">Nombre del campo clave.</param>
        /// <param name="valorClave">Valor de la clave a buscar.</param>
        public async Task<Dictionary<string, object>?> ObtenerPorClaveAsync(
            string nombreProyecto,
            string nombreTabla,
            string nombreClave,
            string valorClave)
        {
            // Esta función busca una entidad específica por su clave primaria
            // Devuelve un diccionario que representa la fila encontrada, o null si no se encuentra

            try
            {
                // Construir la URL incluyendo la clave primaria y su valor
                var url = $"{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";

                // Realizar la petición GET y convertir la respuesta JSON a una lista de diccionarios
                // (La API devuelve una lista aunque solo contenga un elemento)
                var resultado = await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url);

                // Devolver el primer elemento de la lista (o null si la lista está vacía)
                return resultado?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener entidad: {ex.Message}");
                return null;  // Devolver null si ocurre un error
            }
        }

        /// <summary>
        /// Crea una nueva entidad.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla donde crear la entidad.</param>
        /// <param name="entidad">Datos de la entidad a crear.</param>
        public async Task<bool> CrearAsync(
            string nombreProyecto,
            string nombreTabla,
            Dictionary<string, object> entidad)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}";
                var contenido = new StringContent(
                    JsonSerializer.Serialize(entidad),
                    Encoding.UTF8,
                    "application/json");

                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear entidad: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla a actualizar.</param>
        /// <param name="nombreClave">Nombre del campo clave.</param>
        /// <param name="valorClave">Valor de la clave de la entidad a actualizar.</param>
        /// <param name="entidad">Datos actualizados de la entidad.</param>
        public async Task<bool> ActualizarAsync(
            string nombreProyecto,
            string nombreTabla,
            string nombreClave,
            string valorClave,
            Dictionary<string, object?> datos)
        {
            try
            {
                string url = $"{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                var contenido = new StringContent(JsonSerializer.Serialize(datos), Encoding.UTF8, "application/json");
                var respuesta = await _clienteHttp.PutAsync(url, contenido);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar entidad: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina una entidad por su clave primaria.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla donde eliminar.</param>
        /// <param name="nombreClave">Nombre del campo clave.</param>
        /// <param name="valorClave">Valor de la clave de la entidad a eliminar.</param>
        public async Task<bool> EliminarAsync(
            string nombreProyecto,
            string nombreTabla,
            string nombreClave,
            string valorClave)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                var respuesta = await _clienteHttp.DeleteAsync(url);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar entidad: {ex.Message}");
                return false;
            }
        }



        public async Task<List<Dictionary<string, object>>?> EjecutarProcedimientoAsync(
            string nombreProyecto,
            string nombreTabla,
            string nombreSP,
            Dictionary<string, object> parametros)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}/ejecutar-sp";

                // Agregar el nombre del SP como parámetro adicional
                parametros["nombreSP"] = nombreSP;

                var contenido = new StringContent(
                    JsonSerializer.Serialize(parametros),
                    Encoding.UTF8,
                    "application/json"
                );

                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>(_opcionesJson);
                    return resultado;
                }

                Console.WriteLine($"Error al ejecutar procedimiento: {respuesta.ReasonPhrase}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en procedimiento: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Dictionary<string, object>>?> ObtenerPorCampoAsync(
            string nombreProyecto,
            string nombreTabla,
            string campo,
            string valor)
        {
            try
            {
                var url = $"{baseUrl}/api/{nombreProyecto}/{nombreTabla}/filtrar?campo={campo}&valor={valor}";


                var respuesta = await _clienteHttp.GetAsync(url);
                respuesta.EnsureSuccessStatusCode();

                var json = await respuesta.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener por campo: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifica el usuario y obtiene sus roles.
        /// </summary>
        public async Task<HttpResponseMessage> VerificarUsuarioAsync(string nombreProyecto, string nombreTabla, string email, string contrasena)
        {
            var datos = new Dictionary<string, string>
            {
                { "email", email },
                { "contrasena", contrasena }
            };

            var url = $"{baseUrl}/api/{nombreProyecto}/{nombreTabla}/verificar-usuario";
            return await _clienteHttp.PostAsJsonAsync(url, datos);
        }
        // ------------------------------------------
        //       MÉTODOS PARA LAS 6 CONSULTAS
        // ------------------------------------------

        /// <summary>
        /// Consulta 1: GET /api/consultas/1
        /// </summary>
        public async Task<List<Dictionary<string, object>>?> ObtenerConsulta1Async()
        {
            try
            {
                // La ruta en el servidor es “/api/consultas/1”
                // Dado que tu HttpClient base es "http://localhost:5239/api/",
                // aquí solo ponemos “consultas/1”
                var url = "consultas/1";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerConsulta1Async: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Consulta 2: GET /api/consultas/2
        /// </summary>
        public async Task<List<Dictionary<string, object>>?> ObtenerConsulta2Async()
        {
            try
            {
                var url = "consultas/2";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerConsulta2Async: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Consulta 3: GET /api/consultas/3
        /// </summary>
        public async Task<List<Dictionary<string, object>>?> ObtenerConsulta3Async()
        {
            try
            {
                var url = "consultas/3";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerConsulta3Async: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Consulta 4: GET /api/consultas/4
        /// </summary>
        public async Task<List<Dictionary<string, object>>?> ObtenerConsulta4Async()
        {
            try
            {
                var url = "consultas/4";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerConsulta4Async: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Consulta 5: GET /api/consultas/5
        /// </summary>
        public async Task<List<Dictionary<string, object>>?> ObtenerConsulta5Async()
        {
            try
            {
                var url = "consultas/5";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerConsulta5Async: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Consulta 6: GET /api/consultas/6
        /// </summary>
        public async Task<List<Dictionary<string, object>>?> ObtenerConsulta6Async()
        {
            try
            {
                var url = "consultas/6";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerConsulta6Async: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }
    }
}


    




