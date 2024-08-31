using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public class InsertRecords
    {
        private readonly ILogger<InsertRecords> _logger;

        public InsertRecords(ILogger<InsertRecords> logger)
        {
            _logger = logger;
        }
        public SqlModels InsertWithParam(string ConnString, string CommandName, CommandType commandType, SqlParameter[] param)
        {
            string classMeth = "InsertWithParam";
            //string result = string.Empty;
            long result = 0;
            var res = new SqlModels();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = commandType;
                        cmd.CommandText = CommandName;
                        cmd.Parameters.AddRange(param);

                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        result = Convert.ToInt64(cmd.ExecuteScalar());
                        res.ResponseCode = "00";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(classMeth, "Error occured while executing " + CommandName + " " + ex.Message + " " + ex.StackTrace, "");
                        result = 0;
                        res.ResponseCode = "99";
                        res.ErrorMessage = ex.ToString();
                    }
                }
            }

            res.ResultLong = result;
            return res;
        }
    }
}
