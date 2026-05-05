using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IMuayeneRepository
    {
        public void CalismaPlaniOlustur(DoktorCalismaPlani plan);
        public Task<bool> doktorVarMI(int number);
        public Task<bool> polVarMI(int number);
        public Task<bool> PolRandevuMüsaitMi(int number);
        Task<int?> PolMaxSüre(int number);
        Task<int?> PolMaxRanevu(int number);
    }
}
