using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using apiRESTCine.Data;    // CineDbContext, SucursalRepository
using apiRESTCine.Models;  // clsApiStatus, clsSucursal

namespace apiRESTCine.Controllers
{
    [ApiController]
    [Route("cine/sucursal")]
    [Produces("application/json")]
    public class SucursalController : ControllerBase
    {
        private readonly SucursalRepository _repo;

        public SucursalController(SucursalRepository repo)
        {
            _repo = repo;
        }

        // ---------------------------
        // Helper: DataTable -> List<>
        // ---------------------------
        private static List<Dictionary<string, object?>> TableToList(DataTable table)
        {
            
            var list = new List<Dictionary<string, object?>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn col in table.Columns)
                    dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                list.Add(dict);
            }
            return list;
        }

        // GET cine/sucursal/vwrptsucursales
        [HttpGet("vwrptsucursales")]
        public IActionResult vwRptSucursales()
        {
            try
            {
                // Requisito: ejecutar constructor sin parámetros
                var _ = new clsSucursal();

                var ds = _repo.vwRptSucursales();
                var rows = (ds.Tables.Count > 0) ? TableToList(ds.Tables[0]) : new List<Dictionary<string, object?>>();

                return Ok(new clsApiStatus
                {
                    Code = 0,
                    Message = "OK",
                    Data = new { rows } // JSON limpio, sin ciclos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new clsApiStatus
                {
                    Code = -1,
                    Message = $"Error: {ex.Message}",
                    Data = null
                });
            }
        }

        // POST cine/sucursal/spinssucursales
        [HttpPost("spinssucursales")]
        public IActionResult spInsSucursales([FromBody] clsSucursal entrada)
        {
            try
            {
                // Requisito: ejecutar constructor con parámetros
                var modelo = new clsSucursal(
                    entrada.Clave ?? string.Empty,
                    entrada.Nombre ?? string.Empty,
                    entrada.Direccion ?? string.Empty,
                    entrada.Url ?? string.Empty,
                    entrada.Logo ?? string.Empty
                );

                var ds = _repo.spInsSucursales(modelo.Nombre, modelo.Direccion, modelo.Url, modelo.Logo);

                int bandera = -99;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Contains("bandera"))
                {
                    bandera = Convert.ToInt32(ds.Tables[0].Rows[0]["bandera"]);
                }

                return Ok(new clsApiStatus
                {
                    Code = 0,
                    Message = "Ejecutado",
                    Data = new { bandera } // Solo el dato útil
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new clsApiStatus
                {
                    Code = -1,
                    Message = $"Error: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
