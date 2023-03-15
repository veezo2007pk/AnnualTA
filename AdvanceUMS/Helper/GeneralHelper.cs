using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using AdvanceUMS.Models;
using System.Text;
using System.IO;
using System.Globalization;

namespace AdvanceUMS.Helper
{
    
    public class GeneralHelper
    {
        public static string currentYear = "2023";

        Entities db = new Entities();
        public static Boolean IsDemo = true;

        public class UserControl
        {
            public Boolean IsAdd { get; set; }
            public Boolean IsEdit { get; set; }
            public Boolean IsDelete { get; set; }
            public Boolean IsView { get; set; }
            public Boolean IsLogin { get; set; }
            public Boolean IsAdmin { get; set; }
        }
        public string year()
        {
            return currentYear;
        }


        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }        

        public string CurrentLoginUser()
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
                return HttpContext.Current.Request.Cookies["User"].Values["UserID"];
            else
                return "";
        }
        public string CurrentContractorUser()
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
                return HttpContext.Current.Request.Cookies["User"].Values["ContractorID"];
            else
                return "";
        }
        public string CurrentSupervisorLoginUser()
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
                return HttpContext.Current.Request.Cookies["User"].Values["SupervisorID"];
            else
                return "";
        }
        public string CurrentEngineerLoginUser()
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
                return HttpContext.Current.Request.Cookies["User"].Values["EngineerID"];
            else
                return "";
        }

        public string CurrentHODLoginUser()
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
                return HttpContext.Current.Request.Cookies["User"].Values["uqHODCode"];
            else
                return "";
        }


        public void LogAction(String Message)
        {
            try
            {
                System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                Models.tblUserLog lg = new Models.tblUserLog();
                var UserID =  CurrentLoginUser();
                lg.ID = Guid.NewGuid();
                lg.Message = Message;
                lg.MoreInfo = browser.Browser + " " + browser.Version;
                lg.LogTime = dateTime;
                lg.UserID = new Guid(UserID);
                lg.IPAddress = GetVisitorIPAddress();
                db.tblUserLogs.Add(lg);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }


        // get visitor ip address
        public string GetVisitorIPAddress()
        {
            string IPAdd = string.Empty;
            IPAdd = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(IPAdd))
                IPAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return IPAdd;
        }

        // generate module url
        public string GetModuleSlug(String ModuleName, int? ParentModuleID)
        {
            string result = string.Empty;
            ModuleName = ModuleName.Replace(" ", "");
            if (String.IsNullOrEmpty(result))
                result = ModuleName;
            if (ParentModuleID > 0)
                result = GetModuleByID(ParentModuleID).ModuleName.Replace(" ", "") + "_" + result;            
            return RemoveIllegalCharacters(result);
        }

        //get role name
        public string GerRoleName(Guid? RoleID)
        {
            string str = "";
            var role = db.tblRoles.FirstOrDefault(s => s.ID == RoleID);
            if (role != null)
                str = role.Name;
            return str;
        }
        public string GerEngineerName(Guid? RoleID)
        {
            string str = "";
            var role = db.tblEngineers.FirstOrDefault(s => s.uqEngineerId == RoleID);
            if (role != null)
                str = role.strEngineerName;
            return str;
        }
        public string GerSupervisorName(Guid? RoleID)
        {
            string str = "";
            var role = db.tblSupervisors.FirstOrDefault(s => s.uqSupervisorId == RoleID);
            if (role != null)
                str = role.strSupervisorName;
            return str;
        }
        public string GerContractorName(Guid? RoleID)
        {
            string str = "";
            var role = db.tblContractors.FirstOrDefault(s => s.uqContractorCode == RoleID);
            if (role != null)
                str = role.strContractorName;
            return str;
        }
        public string GetArea(Guid? RoleID)
        {
            string str = "";
            var role = db.tblWorkers.Where(x => x.uqId == RoleID).FirstOrDefault();
            var accomodation = db.tblAccomodations.Where(x => x.uqAccommodationCode == role.uqAccommodationCode).FirstOrDefault();
            if (accomodation != null)
                str = accomodation.strAccommodationName;
            return str;
        }
        
        public string GetUsernameBySupervisor(Guid? RoleID)
        {
            string str = "";
            var role = db.tblUsers.Where(x=>x.uqSupervisorCode==RoleID).FirstOrDefault();
            if (role != null)
                str = role.UserName;
            return str;
        }
        public string GetUsernameByEngineer(Guid? RoleID)
        {
            string str = "";
            var role = db.tblUsers.Where(x => x.uqEngineerCode == RoleID).FirstOrDefault();
            if (role != null)
                str = role.UserName;
            return str;
        }
        public string GetFatherName(Guid? RoleID)
        {
            string str = "";
            var role = db.tblWorkers.Where(x => x.uqId == RoleID).FirstOrDefault();
            if (role != null)
                str = role.strFatherName;
            return str;
        }
        public string GetExperience(Guid? RoleID)
        {
            string str = "";
            var role = db.tblWorkers.Where(x => x.uqId == RoleID).FirstOrDefault();
            if (role != null)
                str = role.uqExperienceCode;
            return str;
        }
        public Models.tblModule GetModuleByID(int? CategoryID)
        {
            var module = (from c in db.tblModules
                          where c.ID == CategoryID
                          select c).FirstOrDefault();
            return module;
        }

        private string RemoveExtraHyphen(string text)
        {
            if (text.Contains("__"))
            {
                text = text.Replace("__", "_");
                return RemoveExtraHyphen(text);
            }
            return text;
        }

        private string RemoveDiacritics(string text)
        {
            string Normalize = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Normalize.Length - 1; i++)
            {
                char c = Normalize[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string RemoveIllegalCharacters(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text = text.Replace(":", string.Empty);
            text = text.Replace("/", string.Empty);
            text = text.Replace("?", string.Empty);
            text = text.Replace("#", string.Empty);
            text = text.Replace("[", string.Empty);
            text = text.Replace("]", string.Empty);
            text = text.Replace("@", string.Empty);
            text = text.Replace(",", string.Empty);
            text = text.Replace("\"", string.Empty);
            text = text.Replace("&", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace("'", string.Empty);
            //text = text.Replace("_", string.Empty);
            text = text.Replace(" ", "-");
            text = RemoveDiacritics(text);
            text = RemoveExtraHyphen(text);

            return HttpUtility.UrlEncode(text.ToLower()).Replace("%", string.Empty);
        }

        public UserControl GetUserPermission(string pageName)
        {
            string[] pageSplit = pageName.Split('/');
            UserControl control = new UserControl();
            control.IsAdd = false;
            control.IsDelete = false;
            control.IsEdit = false;
            control.IsView = false;
            control.IsLogin = false;
            control.IsAdmin = false;

            var userID = CurrentLoginUser();
            if (userID == "")
                return control;
            else
                control.IsLogin = true;

            try
            {
                Guid UserID = new Guid(userID);
                var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == UserID);
                if (userInfo != null)
                {
                    var role = db.tblRoles.FirstOrDefault(s => s.ID == userInfo.RoleID && s.Name.ToLower() == "admin");
                    if (role != null)
                    {
                        control.IsAdmin = true;
                        control.IsAdd = true;
                        control.IsDelete = true;
                        control.IsEdit = true;
                        control.IsView = true;
                        return control;
                    }
                    string moduleName = "";
                    if (pageSplit.Length > 1)
                        moduleName = pageSplit[2].ToLower();
                    var module = db.tblModules.FirstOrDefault(s => s.IsDeleted == false && s.PageSlug == moduleName);
                    if (module != null)
                    {
                        var roleModule = db.tblRolePermissions.FirstOrDefault(s => s.RoleID == userInfo.RoleID && s.ModuleID == module.ID);
                        if (roleModule != null)
                        {
                            if (roleModule.IsView == true)
                                control.IsView = true;
                            if (roleModule.IsAdd == true)
                                control.IsAdd = true;
                            if (roleModule.IsDeleted == true)
                                control.IsDelete = true;
                            if (roleModule.IsEdit == true)
                                control.IsEdit = true;
                            return control;
                        }
                    }
                }
                else
                {
                    control.IsAdd = true;
                    control.IsDelete = true;
                    control.IsEdit = true;
                    control.IsView = true;
                }
            }
            catch
            {
                control.IsLogin = false;
            }
            return control;
        }

        public List<MenuViewModel> GetMenuList(Guid? RoleID, Boolean IsAdmin)
        {
            var menuList = db.tblModules.AsEnumerable()
                .Where(s => s.IsActive == true && s.IsDeleted == false && s.ParentModuleID == 0).Select(s =>
                    new MenuViewModel
                    {
                        ID = s.ID,
                        ModuleName = s.ModuleName,
                        DisplayOrder = s.DisplayOrder,
                        ParentModuleID = s.ParentModuleID,
                        PageIcon = s.PageIcon,
                        PageURL = s.PageUrl,
                        PageSlug = s.PageSlug,
                    }).ToList();
            if (IsAdmin != true)
            {
                menuList = (from m in menuList
                    join m1 in db.tblRolePermissions on m.ID equals m1.ModuleID
                    where m1.IsView == true && m1.RoleID == RoleID
                    select m).ToList();
            }

            return menuList.OrderBy(s => s.DisplayOrder).ToList();
        }

        public List<SubMenuViewModel> GetSubMenuList(Guid? RoleID, int? ParentID,Boolean IsAdmin)
        {
            
            var menuList = db.tblModules.AsEnumerable().Where(s => s.IsActive == true && s.IsDeleted == false && s.ParentModuleID == ParentID).Select(s => new SubMenuViewModel
            {
                ID = s.ID,
                ModuleName = s.ModuleName,
                DisplayOrder = s.DisplayOrder,
                PageIcon = s.PageIcon,
                PageURL = s.PageUrl,
                PageSlug = s.PageSlug,
            }).ToList();
            if (IsAdmin != true)
            {
                menuList = (from m in menuList
                            join m1 in db.tblRolePermissions on m.ID equals m1.ModuleID
                            where m1.IsView == true && m1.RoleID == RoleID
                            select m).ToList();
            }
            return menuList.OrderBy(s => s.DisplayOrder).ToList();
        }

        public string UserStatus(string status)
        {
            status = status.ToLower();
            string lblStatus = "";
            if (status == "active")
                lblStatus = "<span class=\"label label-success\">Active</span>";
            else if (status == "inactive")
                lblStatus = "<span class=\"label label-danger\">Banned</span>";
            else if (status == "unconfirmed")
                lblStatus = "<span class=\"label label-warning\">Unconfirmed</span>";
            return lblStatus;
        }

        #region Role Permission
        public string CheckRolePermission(string type, int? ModuleID, Guid? RoleID)
        {
            string str = "";
            var role = db.tblRoles.FirstOrDefault(s => s.ID == RoleID && s.Name.ToLower() == "admin");
            if (role != null)
                return "checked";
            var permission = db.tblRolePermissions.Where(s => s.RoleID == RoleID && s.ModuleID == ModuleID).ToList();
            foreach (var item in permission)
            {
                if (type == "view")
                {
                    if (item.IsView == true)
                        str = "checked";
                }
                if (type == "create")
                {
                    if (item.IsAdd == true)
                        str = "checked";
                }
                if (type == "edit")
                {
                    if (item.IsEdit == true)
                        str = "checked";
                }
                if (type == "delete")
                {
                    if (item.IsDeleted == true)
                        str = "checked";
                }
                if (str != "")
                    return str;
            }
            return str;
        }
        #endregion

        #region ActivityLog List
        public List<ActivityViewModel> GetActivityLog(Guid UserID)
        {            
            var activity = (from a in db.tblUserLogs
                            join u in db.tblUsers on a.UserID equals u.ID
                            select new ActivityViewModel {UserID=u.ID, UserName = u.UserName, Message = a.Message, LogTime = a.LogTime, IPAddress = a.IPAddress });
            if (UserID != Guid.Empty)                       
                activity = activity;
            
            activity = activity.OrderBy(s => s.LogTime);
            return activity.ToList();
        }
        #endregion
    }
}