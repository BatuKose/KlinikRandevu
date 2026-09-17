using Entities.Data_Transfer_Objects.Nager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface INagerDateService
    {
        Task<List<NagerGetDataDTO>> GetHolidaysData();
    }
}
