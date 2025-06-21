using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PruebaDesarrolloSoftware_OjedaGutierrezMauricio.Models;
using System.Data;

namespace PruebaDesarrolloSoftware_OjedaGutierrezMauricio.Controllers
{
    public class TrabajadoresController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly string cad_cn;

        public TrabajadoresController(IConfiguration config)
        {
            _configuration = config;
            cad_cn = config.GetConnectionString("cn1")!;
        }


        #region Obtener DEPARTAMENTO,DISTRITO,PROVINCIA
        
        [HttpGet]
        public IActionResult ObtenerDepartamentos()
        {
            var lista = new List<Departamento>();

            using (SqlConnection cnx = new SqlConnection(cad_cn))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand("listar_departamentos", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Departamento
                            {
                                Id = dr.GetInt32(0),
                                NombreDepartamento = dr.GetString(1)
                            });
                        }
                    }
                }
            }

            return Json(lista);
        }

        [HttpGet]
        public IActionResult ObtenerProvincias(int idDepartamento)
        {
            var lista = new List<Provincia>();

            using (SqlConnection cnx = new SqlConnection(cad_cn))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand("listar_provincia", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdDepartamento", idDepartamento);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Provincia
                            {
                                Id = dr.GetInt32(0),
                                IdDepartamento = dr.GetInt32(1),
                                NombreProvincia = dr.GetString(2)
                            });
                        }
                    }
                }
            }

            return Json(lista);
        }

        [HttpGet]
        public IActionResult ObtenerDistritos(int idProvincia)
        {
            var lista = new List<Distrito>();

            using (SqlConnection cnx = new SqlConnection(cad_cn))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand("listar_distrito", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProvincia", idProvincia);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Distrito
                            {
                                Id = dr.GetInt32(0),
                                IdProvincia = dr.GetInt32(1),
                                NombreDistrito = dr.GetString(2)
                            });
                        }
                    }
                }
            }

            return Json(lista);
        }


        #endregion

        #region PROCESOS PARA TRABAJADORES
        public List<Trabajadores> ListarTrabajadores(string sexo)
        {
            var lista = new List<Trabajadores>();

            string connectionString = cad_cn;

            using (SqlConnection cnx = new SqlConnection(connectionString))
            {
                cnx.Open();

                using (SqlCommand cmd = new SqlCommand("listar_trabajadores", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Sexo", string.IsNullOrEmpty(sexo) ? (object)DBNull.Value : sexo);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var obj = new Trabajadores
                            {
                                Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                TipoDocumento = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                NumeroDocumento = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
                                Nombres = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
                                Sexo = dr.IsDBNull(4) ? string.Empty : dr.GetString(4),
                                IdDepartamento = new Departamento
                                {
                                    Id = dr.GetInt32(5),
                                    NombreDepartamento = dr.GetString(6)
                                },
                                IdProvincia = new Provincia
                                {
                                    Id = dr.GetInt32(7),
                                    NombreProvincia = dr.GetString(8)
                                },
                                IdDistrito = new Distrito
                                {
                                    Id = dr.GetInt32(9),
                                    NombreDistrito = dr.GetString(10)
                                }
                            };

                            lista.Add(obj);
                        }
                    }
                }
            }

            return lista;
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarTrabajador(
            [FromForm] string tipoDocumento,
            [FromForm] string numeroDocumento,
            [FromForm] string nombres,
            [FromForm] string sexo,
            [FromForm] int idDepartamento,
            [FromForm] int idProvincia,
            [FromForm] int idDistrito)
            {
                try
                {
                if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                    string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(sexo))
                {
                    return Json(new { success = false, message = "Complete todos los campos obligatorios." });
                }

                using (SqlConnection cnx = new SqlConnection(cad_cn))
                {
                    await cnx.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("registrar_trabajador", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoDocumento", tipoDocumento);
                        cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                        cmd.Parameters.AddWithValue("@Nombres", nombres);
                        cmd.Parameters.AddWithValue("@Sexo", sexo);
                        cmd.Parameters.AddWithValue("@IdDepartamento", idDepartamento);
                        cmd.Parameters.AddWithValue("@IdProvincia", idProvincia);
                        cmd.Parameters.AddWithValue("@IdDistrito", idDistrito);

                        // Ejecutar el SP
                        int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                        bool success = filasAfectadas > 0;
                        string mensaje = success ? "Trabajador registrado correctamente." : "No se pudo registrar el trabajador.";

                        return Json(new { success = success, message = mensaje });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al registrar: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarTrabajador(
            [FromForm] int idTrabajador,
            [FromForm] string tipoDocumento,
            [FromForm] string numeroDocumento,
            [FromForm] string nombres,
            [FromForm] string sexo,
            [FromForm] int idDepartamento,
            [FromForm] int idProvincia,
            [FromForm] int idDistrito)
                {
                    try
                    {
                        if (idTrabajador <= 0 || string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento) ||
                            string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(sexo))
                        {
                            return Json(new { success = false, message = "Complete todos los campos obligatorios." });
                        }

                        using (SqlConnection cnx = new SqlConnection(cad_cn))
                        {
                            await cnx.OpenAsync();

                            using (SqlCommand cmd = new SqlCommand("actualizar_trabajador", cnx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@IdTrabajador", idTrabajador);
                                cmd.Parameters.AddWithValue("@TipoDocumento", tipoDocumento);
                                cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                                cmd.Parameters.AddWithValue("@Nombres", nombres);
                                cmd.Parameters.AddWithValue("@Sexo", sexo);
                                cmd.Parameters.AddWithValue("@IdDepartamento", idDepartamento);
                                cmd.Parameters.AddWithValue("@IdProvincia", idProvincia);
                                cmd.Parameters.AddWithValue("@IdDistrito", idDistrito);

                                int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                                bool success = filasAfectadas > 0;
                                string mensaje = success ? "Trabajador actualizado correctamente." : "No se encontró el trabajador o no se realizaron cambios.";

                                return Json(new { success = success, message = mensaje });
                            }
                        }
                    }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }




        [HttpPost]
        public async Task<IActionResult> EliminarTrabajador([FromForm] int id)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cad_cn))
                {
                    await cnx.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("eliminar_trabajador", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdTrabajador", id);

                        int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                        return Json(new { success = filasAfectadas > 0, message = filasAfectadas > 0 ? "Trabajador eliminado correctamente." : "No se encontró el trabajador." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }



        #endregion

        public ActionResult Listar_Trabajadores(string sexo=null!)
        {
            var listado = ListarTrabajadores(sexo);

            return View(listado);
        }

        
    }
}
