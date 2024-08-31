using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public class ConnectionManager
    {
        public string ConnectionString { get; set; }  
  
        public string BasisConnectionString { get; set; }
        public string _basisConnectionString { get; set; }
        public string BasisQuery { get; set; }
        public string BasisQuery2 { get; set; }
     
        public string RunTime { get; set; }
        public string AllowedIP { get; set; }
        public string CreatedBy { get; set; }
        public string DelayTime { get; set; }

    }
}
