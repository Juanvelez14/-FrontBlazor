using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;

namespace FrontBlazor.Services
{
    public class ServicioSesion
    {
        private readonly IJSRuntime _js;
        public string EmailUsuario { get; private set; } = string.Empty;
        public List<string> RolesUsuario { get; private set; } = new();

        public ServicioSesion(IJSRuntime js)
        {
            _js = js;
        }

        public async Task CargarSesionAsync()
        {
            var email = await _js.InvokeAsync<string>("localStorage.getItem", "usuario");
            var rolesString = await _js.InvokeAsync<string>("localStorage.getItem", "roles") ?? "";

            EmailUsuario = email ?? "";
            RolesUsuario = rolesString.Split(",", System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public bool EsAdministrador => RolesUsuario.Contains("Administrador(a) del Sistema");
        public bool EsValidador => RolesUsuario.Contains("Validador(a)");
        public bool EsVerificador => RolesUsuario.Contains("Verificador(a)");

        public bool EstaLogueado => !string.IsNullOrEmpty(EmailUsuario);

        public async Task CerrarSesionAsync()
        {
            await _js.InvokeVoidAsync("localStorage.clear");
            EmailUsuario = string.Empty;
            RolesUsuario.Clear();
        }
    }
}
