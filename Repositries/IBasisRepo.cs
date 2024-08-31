using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public interface IBasisRepo
    {
        public DataTable CheckBasis();

        public string GetNuban(string braCode, string cusNum, string curCode, string ledCode, string subCode);
    }
}
