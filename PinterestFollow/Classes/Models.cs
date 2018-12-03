using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PinterestFollow.Classes.Models
{
    public class User
    {
        public User()
        {
            LastVistingLink = "HP";
            UsersFollowed = new List<FollowedUser>();
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime LastSignIn { get; set; }
        public List<Cookie> Cookies { get; set; }
        public string Proxies { get; set; }
        public bool Enabled { get; set; }
        public int Followers { get; set; }
        private int _Following;
        public int Following
        {
            get
            {
                return _Following;
            }
            set
            {
               
                if (_Following == 0)
                {
                    _Following = value;
                    LastCheck = DateTime.Now;
                    LastAdded = Following;
                }
                else {
                    _Following = Following;
                }
                _Following = value;
            }
        }
        public Image StausImage
        {
            get
            {
                if (this.Enabled)
                {
                    return global::PinterestFollow.PinTerestGResource.Enabled;
                }
                else
                {
                    return global::PinterestFollow.PinTerestGResource.Disabled;
                }
            }
        }
        public bool IsLogin { get; set; }
        public string pUsername { get; set; }
        public bool HaveError { get; set; }
        public string LastVistingLink { get; set; }
        public List<string> following { get; set; }
       
        
        public string LastMessage { get; set; }



        public List<FollowedUser> UsersFollowed
        {
            get;
            set;
        }
        
        
        public int LastAdded { get; set; }
        public DateTime LastCheck { get; set; }

        private int _TodayFollow = 0;
        public int TodayFollow
        {
            get
            {
                //return _TodayFollow; 

                if ((DateTime.Now.Subtract(LastCheck).TotalHours >= 24))
                {
                    _TodayFollow = 0;
                    _TodayUnfollowd = 0;
                    LastCheck = DateTime.Now;
                    return _TodayFollow;

                }
                else
                {
                    return _TodayFollow;
                    //_TodayFollow = value;
                }

            }

            set
            {
                if ((DateTime.Now.Subtract(LastCheck).TotalHours >= 24))
                {
                    _TodayFollow = 0;
                    _TodayUnfollowd = 0;
                    LastCheck = DateTime.Now;
                }
                else
                {
                    _TodayFollow = value;
                    //_TodayFollow = value;
                }
            }
        }
        private int _TodayUnfollowd = 0;
        public int TodayUnfollowd
        {
            get
            {
                //return _TodayFollow; 

                if ((DateTime.Now.Subtract(LastCheck).TotalHours >= 24))
                {
                    _TodayUnfollowd = 0;
                    _TodayFollow = 0;
                    LastCheck = DateTime.Now;

                    return _TodayUnfollowd;
                }

                else
                {
                    return _TodayUnfollowd;
                }
            }

            set
            {
                if ((DateTime.Now.Subtract(LastCheck).TotalHours >= 24))
                {
                    _TodayFollow = 0;
                    _TodayUnfollowd = 0;
                    LastCheck = DateTime.Now;
                }
                else
                {
                    _TodayUnfollowd = value;
                    //_TodayFollow = value;
                }
            }
        }
    }


    public class FollowedUser
    {
        public string UserName { get; set; }
        public DateTime DateStartFollowing { get; set; }
    }

    public class Database
    {
        private List<User> _Users;
        public List<User> Users
        {
            get { return _Users; }
            set
            {
                _Users = value;
                if (DatabaseChanged != null)
                {
                    DatabaseChanged(this.Users, new EventArgs());
                }

            }
        }

        public event EventHandler DatabaseChanged;
        public Database()
        {
            Load();
        }
        public void Save()
        {
            StorageHelper.SaveUsers(this.Users);
        }
        public void Load()
        {
            this.Users = StorageHelper.LoadUsers();
        }
    }

    
}
