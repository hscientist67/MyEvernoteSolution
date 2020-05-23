using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernoteCommon.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class UserManager : ManagerBase<EvernoteUser>
    {
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            //Aktivasyon epostası gönderilecek.
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Email adresi kayıtlı");
                }
            }
            else
            {
                //2 tane insert olduğu için bunu kullanırken "base.insert" dedik.
                int dbresult = base.Insert(new EvernoteUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    Password = data.Password,
                    ProfileImageFileName = "user.png",
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false
                }); ;

                if (dbresult > 0)
                {
                    res.Result = Find(x => x.Username == data.Username && x.Email == data.Email);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br>Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                    MailHelper.SendMail(body, res.Result.Email, "MyEvernote Hesap Aktifleştirme");
                }

            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Id == id);

            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            //Giriş kontrolü
            //Hesap aktive edilmiş mi?
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen eposta adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameorPassWrong, "Kullanıcı adı ya da şifre hatalıdır");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.ActivateGuid == id);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Kullanıcı hesabı daha önce aktif edilmiştir.");
                    return res;
                }
                else
                {
                    res.Result.IsActive = true;
                    Update(res.Result);
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.CheckYourEmail, "Aktive edilecek kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {
            EvernoteUser user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (user != null && user.Id != data.Id)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı daha önce kullanılmış.");

                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi sistemde zaten kayıtlıdır.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if (string.IsNullOrEmpty(data.ProfileImageFileName) == false)
            {
                res.Result.ProfileImageFileName = data.ProfileImageFileName;

            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {
            EvernoteUser user = Find(x => x.Id == id);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
                else
                {
                    res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı bulunamadı.");
                }
            }

            return res;

            //İlişkili tablolarda kaydı varsa da hata verecek. Onunla ilgili de işlem yapılmalıdır?
        }
        
        //Burada metot oluştururken "new" keywordunü kullandım. bu demektir ki "Gizli metot" yani burada eğer "insert" isminde bir tane daha metot var
        //o yüzden bunu gizle. Bu şekilde bir metodun geri dönüş tipini bu şekilde değiştirmiş olurum.
        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data)
        {
            //Aktivasyon epostası gönderilecek.
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            res.Result = data;

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "Email adresi kayıtlı");
                }
            }
            else
            {
                res.Result.ProfileImageFileName = "user.png";
                res.Result.ActivateGuid = Guid.NewGuid();

                int dbresult = base.Insert(res.Result);
                if (dbresult==0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı kayıt edilemedi.");
                }
            }
            return res;
        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            EvernoteUser user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (user != null && user.Id != data.Id)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı daha önce kullanılmış.");

                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi sistemde zaten kayıtlıdır.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;
          
            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }
            return res;
        }
    }

}

