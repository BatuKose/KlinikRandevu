using Entities.Data_Transfer_Objects.Parametre;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface ISistemParametreService
    {
        Task<SistemParametresi?> GetirAsync(string parametreAdi);
        Task<bool> AktifMi(string parametreAdi);
        Task<ParametreEkleDTO> ParametreEkleAsync(ParametreEkleDTO parametre);
        Task<ParametreEkleDTO> ParametreGuncelle(ParametreEkleDTO parametre, int id);
    }
}
