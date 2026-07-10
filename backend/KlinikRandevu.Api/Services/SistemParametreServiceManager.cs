using Entities.Data_Transfer_Objects.Parametre;
using Entities.Data_Transfer_Objects.User;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Repositories.Contracts;
using Services.Contracts;
using System.Net.Mail;
using System.Reflection.Emit;

namespace Services
{
    public class SistemParametreServiceManager : ISistemParametreService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMemoryCache _cache;

        public SistemParametreServiceManager(IRepositoryManager repositoryManager, IMemoryCache cache)
        {
            _repositoryManager = repositoryManager;
            _cache = cache;
        }


        public async Task<SistemParametresi?> GetirAsync(string parametreAdi)
        {
            var cacheKey = $"sysparam_{parametreAdi}";

            if (_cache.TryGetValue(cacheKey, out SistemParametresi? cached))
                return cached;
            var param =await _repositoryManager.SistemParametresi.GetirAsync(parametreAdi);
            if(param != null)
            {
                _cache.Set(cacheKey, param,TimeSpan.FromMinutes(30));
            }
            return param;
                
        }

        public async Task<bool> AktifMi(string parametreAdi)
        {
           var param= await GetirAsync(parametreAdi);
            return param != null &&param.Deger1?.ToUpper()=="EVET";
        }

       

        public async Task<ParametreEkleDTO> ParametreEkleAsync(ParametreEkleDTO parametre)
        {
            if (parametre == null)
                throw new BadRequestException("Parametre bilgileri boş olamaz");

            if (string.IsNullOrWhiteSpace(parametre.ParametreAdi))
                throw new BadRequestException("Parametre adı boş olamaz");

            var mevcut = await _repositoryManager.SistemParametresi.GetirAsync(parametre.ParametreAdi);

            if (mevcut != null)
                throw new BadRequestException("Bu parametre adı zaten mevcut");

            var param = new SistemParametresi
            {
                ParametreAdi = parametre.ParametreAdi,
                Deger1 = parametre.Deger1,
                Deger2 = parametre.Deger2,
                Deger3 = parametre.Deger3,
                Deger4 = parametre.Deger4,
                Deger5 = parametre.Deger5,
                Aciklama=parametre.Aciklama
            };

            _repositoryManager.SistemParametresi.Ekle(param);
            await _repositoryManager.saveAsyc();

            return new ParametreEkleDTO
            {
                ParametreAdi = param.ParametreAdi,
                Deger1 = param.Deger1,
                Deger2 = param.Deger2,
                Deger3 = param.Deger3,
                Deger4 = param.Deger4,
                Deger5 = param.Deger5,
                Aciklama= param.Aciklama
            };
        }
        public async Task<ParametreEkleDTO>ParametreGuncelle(ParametreEkleDTO parametre,int id)
        {
            if (parametre == null)
                throw new BadRequestException("Parametre bilgileri boş olamaz");

            var mevcutParametre = await _repositoryManager.SistemParametresi.MevcutById(id);
            if(mevcutParametre == null) throw new NotFoundException("Parametre bilgileri bulunamadı");
            mevcutParametre.ParametreAdi=parametre.ParametreAdi;
            mevcutParametre.Deger1= parametre.Deger1;
            mevcutParametre.Deger2 = parametre.Deger2;
            mevcutParametre.Deger3 = parametre.Deger3;
            mevcutParametre.Deger4 = parametre.Deger4;
            mevcutParametre.Deger5 = parametre.Deger5;
            mevcutParametre.Aciklama=parametre.Aciklama;
            mevcutParametre.GuncellemeTarihi=DateTime.Now;
            await _repositoryManager.saveAsyc();
            return new ParametreEkleDTO
            {
                ParametreAdi=mevcutParametre.ParametreAdi,
                Deger1 = mevcutParametre.Deger1,
                Deger2 = mevcutParametre.Deger2,
                Deger3 = mevcutParametre.Deger3,
                Deger4 = mevcutParametre.Deger4,
                Deger5 = mevcutParametre.Deger5,
                Aciklama=parametre.Aciklama
            };
        }
        public void RedisAll()
        {
            if(_cache is MemoryCache memory)
            {
                memory.Compact(1.0);
            }
        }

        public void userEkle(userEkleDTO user)
        {
            if (user == null) throw new BadRequestException("Kullanıcı bilgileri boş olamaz");
            if(string.IsNullOrWhiteSpace(user.username) || string.IsNullOrWhiteSpace(user.password))
                throw new BadRequestException("Kullanıcı adı veya şifrenin doldurulması gerekiyor.");
            string? gelenEmail = null;
            if (!string.IsNullOrWhiteSpace(user.email))
            {
                if (!MailAddress.TryCreate(user.email, out _))
                    throw new BadRequestException("e-posta girişi hatalı");
                var mailExi = _repositoryManager.SistemParametresi.ayniEpostaVarmi(user.email);
                if (mailExi)
                    throw new BadRequestException("Bu eposta sistemde mevcut farklı e posta yazınız");
                gelenEmail = user.email;
            }
            if (user.username.Length<3 || user.password.Length<=3) throw new BadRequestException("kullanıcı adı veya şifresi 3 haneden büyük olmalıdır");
            var userExixts = _repositoryManager.SistemParametresi.aynıUsernameVarmi(user.username);
            if(userExixts) throw new BadRequestException("Aynı kullanıcı adı mevcut farklı kullanıcı adı alınız");
          
            var userAdd = new User
            {
                UserName=user.username,
                Email=gelenEmail,
                Name=user.name,
                Surname=user.surname,
                Password= BCrypt.Net.BCrypt.HashPassword(user.password)
            };
            _repositoryManager.SistemParametresi.userekle(userAdd);
            _repositoryManager.Save();
        }
    }
}