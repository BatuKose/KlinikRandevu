using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ISistemParametresiRepository 
    {
        Task<SistemParametresi?> GetirAsync(string parametreAdi);
        Task<List<SistemParametresi>> HepsiniGetirAsync();
        void Ekle(SistemParametresi entity);
        Task<SistemParametresi> Mevcut(string name);
        Task<SistemParametresi> MevcutById(int id);
    }
}
