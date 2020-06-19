﻿using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Okurdostu.Data.Model;
using Okurdostu.Web.Base;
using Okurdostu.Web.Extensions;
using Okurdostu.Web.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Okurdostu.Web.Controllers
{
    [Authorize]
    public class AccountController : OkurdostuContextController
    {
        private User AuthUser;

#pragma warning disable CS0618 // Type or member is obsolete
        private IHostingEnvironment Environment;
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
        public AccountController(IHostingEnvironment env) => Environment = env;
#pragma warning restore CS0618 // Type or member is obsolete

        #region account
        [NonAction]
        public async Task<bool> ConfirmIdentityWithPassword(string ConfirmPassword)
        {
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();
            return ConfirmPassword == AuthUser.Password ? true : false;
        }

        //  email editleme:

        //  1.  Guid ile bir key oluşturulacak ve kullanıcı ile bu key eşleştirilip veritabanına alınacak
        //  2.  Keye ulaşabileceği bir link o an kullandığı e-mail adresine yollanacak
        //  3.  O keyli link kullanılarak yeni bir e-mail adresi girişi yapabilecek
        //  4.  Yeni girişin yapıldığı e-mail ilk başta veritabanında eşleştirdiğimiz(key ve kullanıcı) kullanıcıya atanıp: yeni e-mail için onay istenecek

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/password")]
        public async Task EditPassword(ProfileModel Model)
        {
            if (await ConfirmIdentityWithPassword(Model.ConfirmPassword.SHA512()))
            {
                if (Model.RePassword == Model.Password)
                {
                    AuthUser.Password = Model.Password.SHA512();
                    var result = await Context.SaveChangesAsync();
                    if (result! > 0)
                        TempData["ProfileMessage"] = "Başaramadık, neler olduğunu bilmiyoruz";
                }
                else
                    TempData["ProfileMessage"] = "Yeni parolalarınız birbiri ile eşleşmedi";
            }
            else
                TempData["ProfileMessage"] = "Kimliğinizi doğrulayamadık";

            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost,ValidateAntiForgeryToken]
        [Route("~/username")]
        public async Task EditUsername(ProfileModel Model)
        {
            if (await ConfirmIdentityWithPassword(Model.ConfirmPassword.SHA512()))
            {
                if (Model.Username.ToLower() != AuthUser.Username)
                {
                    if (AuthUser.Username != Model.Username)
                    {
                        string NowUsername = AuthUser.Username;
                        AuthUser.Username = Model.Username;
                        try
                        {
                            await Context.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            TempData["ProfileMessage"] = e.InnerException.Message.Contains("Unique_Key_Username")
                                ? "Bu kullanıcı adını kullanamazsınız"
                                : "Başaramadık, neler olduğunu bilmiyoruz";

                            AuthUser.Username = NowUsername;
                            //log ex message
                        }
                    }
                }
            }
            else
                TempData["ProfileMessage"] = "Kimliğinizi doğrulayamadık";

            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/contact")]
        public async Task Contact(ProfileModel Model) //editing, adding contacts
        {
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();
            AuthUser.ContactEmail = Model.ContactEmail;
            AuthUser.Twitter = Model.Twitter;
            AuthUser.Github = Model.Github;
            var result = await Context.SaveChangesAsync();
            if (result! > 0)
                TempData["ProfileMessage"] = "Başaramadık neler olduğunu bilmiyoruz";

            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/basic")]
        public async Task ProfileBasic(ProfileModel Model) //editing, adding bio and fullname
        {
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();
            AuthUser.Biography = Model.Biography;
            AuthUser.FullName = Model.FullName;
            var result = await Context.SaveChangesAsync();
            if (result! > 0)
                TempData["ProfileMessage"] = "Başaramadık neler olduğunu bilmiyoruz";
            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/photo")]
        public async Task AddPhoto()
        {
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();
            var File = Request.Form.Files.First();

            if (File.Length < 1048576)
            {
                if (File.ContentType == "image/png" || File.ContentType == "image/jpg" || File.ContentType == "image/jpeg")
                {
                    string NewName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);
                    string FilePathWithName = Environment.WebRootPath + "/image/profil-fotograf/" + NewName;
                    using var image = Image.Load(File.OpenReadStream());
                    if (image.Width > 200)
                        image.Mutate(x => x.Resize(200, 200));
                    image.Save(FilePathWithName);
                    AuthUser.PictureUrl = "/image/profil-fotograf/" + NewName;
                    var result = await Context.SaveChangesAsync();
                    if (result! > 0)
                        TempData["ProfileMessage"] = "Başaramadık, neler olduğunu bilmiyoruz";
                }
                else
                    TempData["ProfileMessage"] = "PNG, JPG ve JPEG türünde fotoğraf yükleyiniz";
            }
            else
                TempData["ProfileMessage"] = "Seçtiğiniz dosya 1 megabyte'dan fazla";

            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/remove-photo")]
        public async Task RemovePhoto()
        {
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();
            AuthUser.PictureUrl = null;
            //file server'dan silenecek, kullanılmayan hiç bir kullanıcı verisi sunucuda tutulmayacak
            await Context.SaveChangesAsync();
            Response.Redirect("/" + AuthUser.Username);
        }
        #endregion


        #region Education
        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/education")]
        public async Task AddEducation(EducationModel Model)
        {
            var University = await Context.University.FirstOrDefaultAsync(x => x.Id == Model.UniversityId);
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();

            if (University != null)
            {
                if (Model.Startyear < Model.Finishyear)
                {
                    if (User != null)
                    {
                        var Education = new UserEducation
                        {
                            UserId = AuthUser.Id,
                            UniversityId = University.Id,
                            Department = Model.Department,
                            StartYear = Model.Startyear.ToString(),
                            EndYear = Model.Finishyear.ToString(),
                            ActivitiesSocieties = Model.ActivitiesSocieties
                        };
                        await Context.UserEducation.AddAsync(Education);
                        var result = await Context.SaveChangesAsync();

                        TempData["ProfileMessage"] = result > 0
                            ? "Eğitim bilginiz eklendi<br />Onaylanması için belge yollamayı unutmayın."
                            : "Başaramadık, neler olduğunu bilmiyoruz";
                    }
                }
                else
                    TempData["ProfileMessage"] = "Başlangıç yılınız, bitiriş yılınızdan büyük olmamalı";
            }
            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/edit-education")]
        public async Task EditEducation(EducationModel Model)
        {
            var Education = await Context.UserEducation.FirstOrDefaultAsync(x => x.Id == Model.EducationId);
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();

            if (Education != null)
            {
                if (Education.UserId == AuthUser.Id)
                {
                    Education.ActivitiesSocieties = Model.ActivitiesSocieties;
                    if (Education.IsUniversityInformationsCanEditable())
                    {
                        Education.UniversityId = (short)Model.UniversityId;
                        Education.Department = Model.Department;
                    }
                    if (Model.Startyear < Model.Finishyear)
                    {
                        Education.StartYear = Model.Startyear.ToString();
                        Education.EndYear = Model.Finishyear.ToString();
                        TempData["ProfileMessage"] = "Eğitim bilgileriniz düzenlendi";
                    }
                    else
                        TempData["ProfileMessage"] = "Başlangıç yılınız, bitiriş yılınızdan büyük olmamalı" +
                            "<br />" + "Bunlar dışında ki eğitim bilgileriniz düzenlendi";

                    var result = await Context.SaveChangesAsync();
                    if (result! > 0)
                        TempData["ProfileMessage"] = "Başaramadık, neler olduğunu bilmiyoruz";
                }
                else
                    TempData["ProfileMessage"] = "MC Hammer: You can't touch this";
            }
            Response.Redirect("/" + AuthUser.Username);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/remove-education")]
        public async Task RemoveEducation(long Id, string Username)
        {
            var Education = await Context.UserEducation.FirstOrDefaultAsync(x => x.Id == Id && x.User.Username == Username && !x.IsRemoved);
            AuthUser = await GetAuthenticatedUserFromDatabaseAsync();

            if (Education != null)
            {
                if (AuthUser.Id == Education.UserId)
                {
                    var AuthenticatedUserNeedCount = Context.Need.Where(x => !x.IsRemoved && x.UserId == AuthUser.Id).Count();
                    if (Education.IsActiveEducation && AuthenticatedUserNeedCount > 0)
                        TempData["ProfileMessage"] = "İhtiyaç kampanyanız olduğu için" +
                            "<br />" +
                            "Aktif olan eğitim bilginizi silemezsiniz." +
                            "Aktif olan eğitim bilgisi, hala burada okuduğunuzu iddia ettiğiniz bir eğitim bilgisidir.";
                    else
                    {
                        Education.IsRemoved = true;
                        var result = await Context.SaveChangesAsync();
                        if (result! > 0)
                            TempData["ProfileMessage"] = "Başaramadık, neler olduğunu bilmiyoruz";
                    }
                }
                else
                    TempData["ProfileMessage"] = "MC Hammer: You can't touch this";
            }

            Response.Redirect("/" + AuthUser.Username);
        }
        #endregion
    }
}
