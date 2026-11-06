using Entities.Data_Transfer_Objects.Email;
using Entities.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class SistemParametresiRepository : ISistemParametresiRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public SistemParametresiRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }
        public async Task<SistemParametresi?> GetirAsync(string parametreAdi)
        {
            return await _repositoryContext.parametreler
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ParametreAdi == parametreAdi && p.Aktif);
        }

        public async Task<List<SistemParametresi>> HepsiniGetirAsync()
        {
            return await _repositoryContext.parametreler
                .AsNoTracking()
                .ToListAsync();
        }

        public void Ekle(SistemParametresi entity)
        {
            _repositoryContext.parametreler.Add(entity);
        }
        public async Task<SistemParametresi?>Mevcut(string name)
        {
            var param= await _repositoryContext.parametreler.FirstOrDefaultAsync(p=>p.ParametreAdi==name);
            return param;
        }
        public async Task<SistemParametresi?> MevcutById(int id)
        {
            var param = await _repositoryContext.parametreler.FirstOrDefaultAsync(p => p.Id==id);
            return param;
        }

        public void userekle(User user)
        {
           _repositoryContext.Add(user);
        }
        public bool aynıUsernameVarmi(string username)
        {
            bool userVarmi = _repositoryContext.Users.Any(u=>u.UserName==username);
            return userVarmi;
        }

        public bool ayniEpostaVarmi(string eposta)
        {
            bool epostaVarmi = _repositoryContext.Users.Any(e => e.Email==eposta);
            return epostaVarmi;
        }
        public async Task<GetEmailConfigDTO?> GetEmail()
        {
            var email = await _repositoryContext.EmailConfig
                .AsNoTracking()
                .Where(e => e.Active)
                .Select(e => new GetEmailConfigDTO
                {
                   SenderEmail = e.SenderEmail,
                   Password= e.Password,
                   Port= e.Port,
                   SenderName= e.SenderName,
                   SmtpServer=e.SmtpServer
                })
                .FirstOrDefaultAsync();

            return email;
        }

        public async Task<List<EmailConfig>> EmailConfigListele()
        {
            return await _repositoryContext.EmailConfig
                .AsNoTracking()
                .OrderByDescending(e => e.Active)
                .ThenByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<EmailConfig?> EmailConfigGetirById(int id)
        {
            return await _repositoryContext.EmailConfig.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> BaskaAktifEmailConfigVarMi(int haricId)
        {
            return await _repositoryContext.EmailConfig.AnyAsync(e => e.Active && e.Id != haricId);
        }

        public void EmailConfigEkle(EmailConfig entity)
        {
            _repositoryContext.EmailConfig.Add(entity);
        }
        public async Task<User> GetUserByUsername(string  username)
        {
            var result = await _repositoryContext.Users.FirstOrDefaultAsync(u => u.UserName==username);
            return result;
        }
    }
}
