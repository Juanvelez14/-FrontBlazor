using Microsoft.JSInterop;

namespace FrontBlazor.Services
{
    public class SesionUsuarioService
    {
        private readonly IJSRuntime _js;

        public string? Email { get; private set; }
        public List<string> Roles { get; private set; } = new();

        public bool EsAdministrador => Roles.Contains("Administrador(a) del Sistema");
        public bool EsValidador => Roles.Contains("Validador(a)");
        public bool EsVerificador => Roles.Contains("Verificador(a)");

        public bool EstaLogueado => !string.IsNullOrEmpty(Email);

        public SesionUsuarioService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task CargarDesdeLocalStorageAsync()
        {
            var roles = await _js.InvokeAsync<string>("localStorage.getItem", "roles");
            var email = await _js.InvokeAsync<string>("localStorage.getItem", "usuario");

            Roles = roles?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new();
            Email = email;
        }

        public async Task CargarSesionAsync()
        {
            await CargarDesdeLocalStorageAsync();
        }

        public async Task CerrarSesionAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "usuario");
            await _js.InvokeVoidAsync("localStorage.removeItem", "roles");
            Email = null;
            Roles.Clear();
        }

        public bool TieneAcceso() => EsAdministrador || EsValidador || EsVerificador;
    }
}
