using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        Task saveAsyc();
        void Save();
        IPatientRepository Patient { get; }
        IPoliklinikRepository Poliklinik { get; }

    }
}
