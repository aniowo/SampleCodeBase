using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AdditionalAccountCreation.WorkerService
{
    public class ProcessProfiling
    {
        private readonly ILogger<ProcessProfiling> _logger;
        private readonly IBasisRepo _basisRepo;
        private readonly ConnectionManager _connection;
        private readonly SelectRecords _select;
        private readonly InsertRecords _insert;
        private string braCode;
        private string cusNum;
        private string curCode;
        private string ledCode;
        private string subCode;
        private string custId;
        private string nuban;
        private string status;
        private string createdBy;
        private string userId;
        private string acctId;
        private string fullAcctKey;
        private string updatedD;



        public ProcessProfiling(ILogger<ProcessProfiling> logger, IBasisRepo basisRepo, ConnectionManager connectionManager, SelectRecords select, InsertRecords insert)
        {
            _basisRepo = basisRepo;
            _logger = logger;
            _connection = connectionManager;
            _select = select;
            _insert = insert;
        }

        public string ProcessNewAccount()
        {
            string ret_str = string.Empty;

            DataTable newAccounts = _basisRepo.CheckBasis();

            if (newAccounts.Rows.Count > 0)
            {
                foreach (DataRow dr in newAccounts.Rows)
                {
                    braCode = dr["bra_code"].ToString();
                    cusNum = dr["cus_num"].ToString();
                    curCode = dr["cur_code"].ToString();
                    ledCode = dr["led_code"].ToString();
                    subCode = dr["sub_acct_code"].ToString();
                    fullAcctKey = braCode + "/" + cusNum + "/" + curCode + "/" + ledCode + "/" + subCode;
                    nuban = _basisRepo.GetNuban(braCode, cusNum, curCode, ledCode, subCode);
                    userId = braCode + cusNum  + "01";

                    SqlParameter[] sqlParameter = new SqlParameter[]
                   {
                        new SqlParameter("@userId",userId),
                   };

                    SqlModels users = new SqlModels();

                    SqlModels customers = new SqlModels();

                    users = _select.SelectWithParam(_connection.ConnectionString, "SelectUsersByUserID", CommandType.StoredProcedure, sqlParameter);

                    if (users.ResultDataTable.Rows.Count > 0)
                    {
                        foreach (DataRow drr in users.ResultDataTable.Rows)
                        {
                            custId = drr["customer_id"].ToString();
                            createdBy = drr["Registered_By"].ToString();
                        }

                        SqlParameter[] sqlParameter2 = new SqlParameter[]
                        {
                                        new SqlParameter("@customerId",custId),
                        };

                          customers = _select.SelectWithParam(_connection.ConnectionString, "SelectCustomersByCustomerID", CommandType.StoredProcedure, sqlParameter2);

                        if (customers.ResultDataTable.Rows.Count > 0)
                        {
                            foreach (DataRow drr2 in customers.ResultDataTable.Rows)
                            {
                                status = drr2["status"].ToString();
                            }


                        }

                        else
                        {
                            _logger.LogInformation(custId, ": Customer was not Found on CUSTOMERS Table");
                        }
                    }

                    else
                    {
                        _logger.LogInformation(userId, ": User was not Found on USERS Table");
                    }

                    if (users.ResultDataTable.Rows.Count == 1 && customers.ResultDataTable.Rows.Count == 1)
                    {
                        _logger.LogInformation("About to Check if Record  Already exist in ACCOUNTS Table");
                        SqlParameter[] sqlParameter3 = new SqlParameter[]
                        {
                                        new SqlParameter("@Customer_id",custId),
                                        new SqlParameter("@Ledger_code",ledCode),
                                        new SqlParameter("@sub_acct",subCode),
                        };

                        SqlModels account = _select.SelectWithParam(_connection.ConnectionString, "SelectAccountsByCustomerID", CommandType.StoredProcedure, sqlParameter3);

                        if (account.ResultDataTable.Rows.Count == 0)
                        {
                            
                            _logger.LogInformation("About to Insert record into ACCOUNTS Table");
                            SqlParameter[] sqlParameter4 = new SqlParameter[]
                            {
                                new SqlParameter("@Customer_id", custId),
                                new SqlParameter("@Customer_no", cusNum),
                                new SqlParameter("@Branch_code", braCode),
                                new SqlParameter("@Currency_code", curCode),
                                new SqlParameter("@Ledger_code", ledCode),
                                new SqlParameter("@Sub_acct_code", subCode),
                                new SqlParameter("@Accountfullname", fullAcctKey),
                                new SqlParameter("@nuban", nuban),
                                new SqlParameter("@Status", status),
                                new SqlParameter("@created_by", _connection.CreatedBy),
                                new SqlParameter("@UpdateD", updatedD),
                            };
                            var insertRes = _insert.InsertWithParam(_connection.ConnectionString, "InsertNewAccount2", CommandType.StoredProcedure, sqlParameter4);

                            if (insertRes.ResponseCode == "00")
                            {
                                acctId = insertRes.ResultLong.ToString();
                                _logger.LogInformation(custId + ": Insert Record into ACCOUNTS Table Successful");

                                _logger.LogInformation("About to Insert record into USERS_ACCOUNTS Table");
                                SqlParameter[] sqlParameter5 = new SqlParameter[]
                                 {
                                new SqlParameter("@user_id", userId),
                                new SqlParameter("@Account_Id", acctId),
                                new SqlParameter("@short_name", "DOM1"),
                                 };

                                var insertRes2 = _insert.InsertWithParam(_connection.ConnectionString, "InsertUser_Account", CommandType.StoredProcedure, sqlParameter5);

                                if (insertRes2.ResponseCode == "00")
                                {
                                    ret_str = "SUCCESS";
                                    _logger.LogInformation(userId + ": Insert Record into USERS_ACCOUNTS Table Successful");
                                }
                                else
                                {
                                    _logger.LogInformation(userId + ": Insert Record into USERS_ACCOUNTS Table Failed with error Code: " + insertRes2.ResponseCode);
                                }
                            }
                            else
                            {
                                _logger.LogError(custId + ": Insert Record into ACCOUNTS Table Failed with error Code: " + insertRes.ResponseCode);
                            }
                        }
                        else
                        {
                            _logger.LogError(custId + ": Record Already exist on ACCOUNTS table for this User");
                        }

                    }
                }
            }
            else
            {
                _logger.LogInformation("No New Account was Opened the Previous Day");
            }

            return ret_str;
        }
    }
}
