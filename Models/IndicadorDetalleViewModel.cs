namespace FrontBlazor.Models
{
    public class IndicadorDetalleViewModel
    {
        public Dictionary<string, object?>? Indicador { get; set; }
        public List<Dictionary<string, object?>>? Fuentes { get; set; }
        public List<Dictionary<string, object?>>? Responsables { get; set; }
        public List<Dictionary<string, object?>>? Variables { get; set; }
        public List<Dictionary<string, object?>>? RepresentacionesVisuales { get; set; }
        public List<Dictionary<string, object?>>? Resultados { get; set; }
    }
}
