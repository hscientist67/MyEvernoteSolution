using MyEvernote.Entities;
using MyEvernote.WebApp.Models;
using MyEvernoteCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.WebApp.Init
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUserName()
        {
            EvernoteUser user = SessionHelper.User;
            if (user != null)
                return user.Username;
            else
                return "system";
        }
    }
}