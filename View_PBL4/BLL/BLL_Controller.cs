using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using View_PBL4.Models;

namespace View_PBL4.BLL
{
    public class BLL_Controller
    {
        PBL4Entities db = new PBL4Entities();
        private static BLL_Controller _Instance;
        public static BLL_Controller Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new BLL_Controller();
                }
                return _Instance;
            }
            private set { }
        }
        private BLL_Controller()
        {

        }
        public void UpdateStatus(string UserID, string Detail, string Status)
        {
            var result = db.Users.Where(p => p.UserID == UserID).FirstOrDefault();
            result.UserID = UserID;
            result.Detail = Detail;
            result.Status = Status;
            db.SaveChanges();
        }
        public List<User> LoadUser ()
        {
            List<User> user = new List<User>();
            user = db.Users.ToList();
            return user;
        }
        public List<Folder> LoadFolder()
        {
            List<Folder> folder = new List<Folder>();
            folder = db.Folders.ToList();
            return folder;
        }
        public List<User> LoadUserByStatus()
        {
            List<User> user = new List<User>();
            user = db.Users.Where(p=> p.Status == "Online").ToList();
            return user;
        }
        public List<FileDetail> Load_Img_Txt(string id)
        {
            List<FileDetail> result = new List<FileDetail>();
            result = db.FileDetails.Where(p => p.FolderID == id).ToList();
            return result;
        }
        public List<Folder> Cheange_Img(string id)
        {
            List<Folder> result = new List<Folder>();
            result = db.Folders.Where(p => p.Type == "Image" && p.UserID == id).ToList();
            return result;
        }
        public List<Folder> Cheange_txt(string id)
        {
            List<Folder> result = new List<Folder>();
            result = db.Folders.Where(p => p.Type == "Text" && p.UserID == id).ToList();
            return result;
        }
        public void AddUserSocket(User Client)
        {
            db.Users.Add(Client);
            db.SaveChanges();
        }
        public void AddFolder(Folder Folder)
        {
            db.Folders.Add(Folder);
            db.SaveChanges();

        }
        public void AddFileDetail(FileDetail FileDetail)
        {
            db.FileDetails.Add(FileDetail);
            db.SaveChanges();
        }
        public List<string> GetIDFolder()
        {
            List<string> result;
            result = db.Folders.Select(p => p.FolderID).ToList();
            return result;
        }
        public List<string> GetIDFile()
        {
            List<string> result;
            result = db.FileDetails.Select(p => p.FileID).ToList();
            return result;
        }
        public List<string> GetUserID()
        {
            List<string> result;
            result = db.Users.OrderBy(p => p.UserID).Select(p => p.UserID).ToList();
            return result;
        }
        

        public bool CheckUserID(string Macaddress)
        {

            List<string> UserID = GetUserID();
            if (UserID.Contains(Macaddress))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CheckFolderID(string FolderID)
        {
            List<string> Folder = GetIDFolder();
            if (Folder.Contains(FolderID))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CheckFileDetail(string FileDetail)
        {
            List<string> File = GetIDFile();
            if (File.Contains(FileDetail))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}