using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public class SelectRecords
    {
        private readonly ILogger<SelectRecords> _logger;

        public SelectRecords(ILogger<SelectRecords> logger)
        {
            _logger = logger;
        }
        public SqlModels SelectWithParam(string ConnString, string CommandQuery, CommandType cmdType, SqlParameter[] param)
        {
            string classMeth = "SelectWithQuery";
            DataTable ds = new DataTable();
            var res = new SqlModels();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = cmdType;
                        cmd.CommandText = CommandQuery;
                        cmd.Parameters.AddRange(param);

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                            res.ResponseCode = "00";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, classMeth + " :Error Occured while Excecuting Method SelectWithParam");
                        res.ResponseCode = "99";
                        res.ErrorMessage = ex.Message;
                        res.StackTrace = ex.StackTrace;
                    }
                }
            }

            res.ResultDataTable = ds;
            return res;
        }

    }
}
