namespace PruebaDesarrolloSoftware_OjedaGutierrezMauricio.Models
{
    public class Trabajadores
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = "";
        public string NumeroDocumento { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string Sexo { get; set; } = "";
        public Departamento IdDepartamento { get; set; } 
        public Provincia IdProvincia { get; set; } 
        public Distrito IdDistrito { get; set; } 
    }
}
