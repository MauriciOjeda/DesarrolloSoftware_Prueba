namespace PruebaDesarrolloSoftware_OjedaGutierrezMauricio.Models
{
    public class Trabajadores
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = "";
        public string NumeroDocumento { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string Sexo { get; set; } = "";
        public string Departamento { get; set; } = "";
        public string Provincia { get; set; } = "";
        public string Distrito { get; set; } = "";
    }
}
