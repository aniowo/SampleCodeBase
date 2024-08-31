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
    public class BasisRepo : IBasisRepo
    {
        private readonly ILogger<BasisRepo> _logger;
        private readonly BasisCheck _basisCheck;


        public BasisRepo(ILogger<BasisRepo> logger, BasisCheck basisCheck)
        {
            _logger = logger;
            _basisCheck = basisCheck;
        }
        public DataTable CheckBasis()
        {

            _logger.LogInformation($"About to Check Basis for New Accounts that was Created the Previous Day");
            DataTable dataTable = _basisCheck.CheckBasisforNewAccounts();

            return dataTable;

        }

        public string GetNuban(string braCode, string cusNum, string curCode, string ledCode, string subCode)
        {
            _logger.LogInformation($"About to get NUBAN from Basis");

            string nuban = _basisCheck.GetNuban(braCode,cusNum,curCode,ledCode,subCode);

           return nuban;
        }
    }
}
