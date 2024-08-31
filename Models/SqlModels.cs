using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public class SqlModels
    {

        public string ResponseCode { get; set; }
        public int ResultInt { get; set; }
        public long ResultLong { get; set; }
        public DataTable ResultDataTable { get; set; }
        public DataSet ResultDataSet { get; set; }
        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }
    }
}
