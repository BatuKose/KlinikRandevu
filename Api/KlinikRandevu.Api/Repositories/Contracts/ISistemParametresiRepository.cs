using Entities.Data_Transfer_Objects.Email;
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
        Task<SistemParametresi?> Mevcut(string name);
        Task<SistemParametresi?> MevcutById(int id);
        void userekle(User user);
        bool aynıUsernameVarmi(string username);
        bool ayniEpostaVarmi(string eposta);
        Task<GetEmailConfigDTO?> GetEmail();
        Task<List<EmailConfig>> EmailConfigListele();
        Task<EmailConfig?> EmailConfigGetirById(int id);
        Task<bool> BaskaAktifEmailConfigVarMi(int haricId);
        void EmailConfigEkle(EmailConfig entity);
        Task<User> GetUserByUsername(string username);
    }
}
