namespace apiRESTCine.Data;   // <-- EXACTO

using System.Data;
using MySql.Data.MySqlClient;

public class SucursalRepository
{
    private readonly string _connStr;

    public SucursalRepository(CineDbContext ctx)
    {
        _connStr = ctx.ConnectionString;
    }

    public DataSet vwRptSucursales()
    {
        var ds = new DataSet();
        using var conn = new MySqlConnection(_connStr);
        using var cmd  = new MySqlCommand("SELECT * FROM vwRptSucursales;", conn);
        using var da   = new MySqlDataAdapter(cmd);
        da.Fill(ds, "Sucursales");
        return ds;
    }

    public DataSet spInsSucursales(string nombre, string direccion, string url, string logo)
    {
        var ds = new DataSet();
        int flag = -99;

        using var conn = new MySqlConnection(_connStr);
        using var cmd  = new MySqlCommand("spInsSucursales", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.Add(new MySqlParameter("p_nombre",    MySqlDbType.VarChar, 50)  { Value = nombre });
        cmd.Parameters.Add(new MySqlParameter("p_direccion", MySqlDbType.VarChar, 100) { Value = direccion });
        cmd.Parameters.Add(new MySqlParameter("p_homeweb",   MySqlDbType.VarChar, 100) { Value = url });
        cmd.Parameters.Add(new MySqlParameter("p_logo",      MySqlDbType.VarChar, 100) { Value = logo });

        var pFlag = new MySqlParameter("p_flag", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(pFlag);

        conn.Open();
        cmd.ExecuteNonQuery();

        if (pFlag.Value != DBNull.Value) flag = Convert.ToInt32(pFlag.Value);

        var t = new DataTable("Estatus");
        t.Columns.Add("bandera", typeof(int));
        var r = t.NewRow(); r["bandera"] = flag; t.Rows.Add(r);
        ds.Tables.Add(t);

        return ds;
    }
}
