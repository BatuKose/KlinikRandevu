using Entities.Data_Transfer_Objects.IcdApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IIcdApiManager
    {
        Task<string> IcdApiTokenAl();
        Task<List<TaniDto>> TaniAraAsync(string aranan);
    }
}
