using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{ 
    public class BasisCheck
    {
        private readonly ILogger<BasisCheck> _logger;
        private readonly ConnectionManager _connection;


        public BasisCheck(ILogger<BasisCheck> logger, ConnectionManager connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public DataTable CheckBasisforNewAccounts()
        {
            DataTable dt = null;
            try
            {
                string strQuery = _connection.BasisQuery;

                DateTime openDate = DateTime.Now.AddDays(-1);
                _logger.LogInformation("Basis setting connection !!! ", "CheckBasisforTransaction");
               
                    using (OracleConnection conn = new OracleConnection(_connection.BasisConnectionString))
                    {
                        using (OracleCommand cmd = new OracleCommand(strQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;


                            cmd.Parameters.Add("openDate", openDate.ToString("ddMMMyyyy"));
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            cmd.CommandTimeout = 3600;

                            OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd);
                            dt = new DataTable();
                            dataAdapter.Fill(dt);
                            _logger.LogInformation("Account Opened Previous Day returned records " + dt.Rows.Count.ToString(), "NewAccounts");


                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                                conn.Dispose();

                            }
                        }
                       

                    }
              
             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckBasisforNewAccounts");
            }
       
            return dt;
        }

        public string GetNuban(string braCode, string cusNum, string curCode, string ledCode, string subCode)
        {
            DataTable dt = null;
            string nuban = string.Empty;
            try
            {
                string strQuery = _connection.BasisQuery2;

                _logger.LogInformation("Basis setting connection !!! ", "CheckBasisforTransaction");

                using (OracleConnection conn = new OracleConnection(_connection.BasisConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(strQuery, conn))
                    {
                        cmd.CommandType = CommandType.Text;


                        cmd.Parameters.Add("braCode",braCode);
                        cmd.Parameters.Add("cusNum", cusNum);
                        cmd.Parameters.Add("curCode", curCode);
                        cmd.Parameters.Add("ledCode", ledCode);
                        cmd.Parameters.Add("subCode", subCode);

                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                        cmd.CommandTimeout = 3600;

                        OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd);
                        dt = new DataTable();
                        dataAdapter.Fill(dt);

                        if(dt.Rows.Count > 0)
                        {
                            foreach (DataRow drr in dt.Rows)
                            {
                                nuban = drr["map_acc_no"].ToString();
                                _logger.LogInformation("NUBAN has been retrieved: " + dt.Rows.Count.ToString(), "FetchNuban");
                            }
                        }
                        else
                        {
                            nuban = "";
                            _logger.LogInformation("No Record was retrieved from table for: " + braCode + "/" + cusNum);
                        }

                        

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                            conn.Dispose();

                        }
                    }


                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FetchNuban");
            }

            return nuban;
        }
    }
}
