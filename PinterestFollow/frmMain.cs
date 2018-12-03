using PinterestFollow.Classes.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace PinterestFollow
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
         
            //MessageBox.Show(HttpUtility.HtmlEncode(@"http://pinterest.com/resource/UserFollowingResource/get/?data=%7B%22options%22%3A%7B%22username%22%3A%22omidam81%22%2C%22bookmarks%22%3A%5B%22101290240.19_-1%3A0%7C497b54c3e8cb3b3c4f13e9e1ed0b1ec55b2ef5ad591bb623db028de8ff05e11f%22%5D%7D%2C%22module%22%3A%7B%22name%22%3A%22Grid%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22layout%22%3A%22fixed_height%22%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Afalse%7D%2C%22append%22%3Atrue%2C%22errorStrategy%22%3A0%7D%2C%22context%22%3A%7B%22app_version%22%3A%226b0d%22%7D%7D&source_url=%2Fomidam81%2Ffollowing%2F&module_path=App()%3EUserProfilePage(resource%3DUserResource(username%3Domidam81))%3EUserProfileHeader(resource%3DUserResource(username%3Domidam81))%3EShowModalButton(submodule%3D%5Bobject+Object%5D%2C+class_name%3DeditProfile%2C+text%3DEdit+Profile%2C+has_icon%3Dtrue%2C+tagName%3Dbutton%2C+borderless%3Dtrue%2C+show_text%3Dfalse%2C+ga_category%3Duser_edit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))&_=1364397480760"));


            InitializeComponent();
        }
        public int UnFollowCount { get; set; }
        public int FollowCount { get; set; }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure About closing this program", "Closing", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtProxy_TextChanged(object sender, EventArgs e)
        {
            btnAddUser.
                Enabled =

                !string.IsNullOrWhiteSpace(txtpassword.Text) &&
                !string.IsNullOrWhiteSpace(txtUsername.Text);
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            User U = new User()
            {
                Password = txtpassword.Text,
                UserName = txtUsername.Text,
                Proxies = txtProxy.Text,
                Enabled = true
            };
            var v = Program.Database.Users.FirstOrDefault(aa => aa.UserName == U.UserName);
            if (v != null)
                Program.Database.Users.Remove(v);
            Program.Database.Users.Add(U);

            Database_DatabaseChanged(Program.Database.Users, new EventArgs());
            if (Program.Database.Users.Count == 1)
            {
                Program.Database.Save();
                Application.Restart();
                
            }
            txtProxy.Text = txtpassword.Text = txtUsername.Text = "";
        }


        int xRoundCoutn = 0;
        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadSetting();
            Program.Database.DatabaseChanged += Database_DatabaseChanged;
            Database_DatabaseChanged(Program.Database.Users, new EventArgs());

            if (Classes.AppSetting.AutoSatrt)
            {
                btnPlay_Click(btnPlay, new EventArgs());
            }
        }

        private void LoadSetting()
        {
            txtMin.Text = Classes.AppSetting.MinDelay.ToString();
            txtMax.Text = Classes.AppSetting.MaxDelay.ToString();
            chkAutoStart.Checked = Classes.AppSetting.AutoSatrt;
            cmbThreadCount.Text = Classes.AppSetting.ThreadCount.ToString();
            nmHours.Value = Classes.AppSetting.TotalHours;

            MinF.Value = Classes.AppSetting.MinF;

            MaxF.Value = Classes.AppSetting.MaxF;

            MinU.Value = Classes.AppSetting.MinU;

            MaxU.Value = Classes.AppSetting.MaxU;



            FollowCount = new Random().Next(Classes.AppSetting.MinF, Classes.AppSetting.MaxF);

            UnFollowCount = new Random().Next(Classes.AppSetting.MinU, Classes.AppSetting.MaxU);

        }

        void Database_DatabaseChanged(object sender, EventArgs e)
        {
            Program.Database.Save();

            this.Invoke(new MethodInvoker(delegate()
            {
                dgUserEdit.DataSource = dgAccountInfo.DataSource = null;
                dgUserEdit.DataSource = Program.Database.Users;
                dgAccountInfo.DataSource = Program.Database.Users;
            }));
            
            Application.DoEvents();
            

        }

        private void dgUserEdit_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgUserEdit_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == 1)
            {
                var user = dgUserEdit.Rows[e.RowIndex].DataBoundItem as User;

                var mUser = Program.Database.Users.FirstOrDefault(aa => aa.UserName == user.UserName);
                mUser.Enabled = !mUser.Enabled;

            }
        }

        private void dgUserEdit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == 2)
            {
                var user = dgUserEdit.Rows[e.RowIndex].DataBoundItem as User;
                var mUser = Program.Database.Users.FirstOrDefault(aa => aa.UserName == user.UserName);
                Program.Database.Users.Remove(Program.Database.Users.FirstOrDefault(aa => aa.UserName == user.UserName));
                Database_DatabaseChanged(Program.Database.Users, new EventArgs());
            }

            if (e.ColumnIndex == 3)
            {
                var user = dgUserEdit.Rows[e.RowIndex].DataBoundItem as User;
                MessageBox.Show(string.IsNullOrWhiteSpace(user.LastMessage) ? "No Error" : user.LastMessage);
            }
        }

        private void dgUserEdit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dgUserEdit.Rows[e.RowIndex].Cells[2].Value = "Delete";
            dgUserEdit.Rows[e.RowIndex].Cells[3].Value = "View";
            dgUserEdit.Rows[e.RowIndex].Cells[3].ToolTipText = (dgUserEdit.Rows[e.RowIndex].DataBoundItem as User).LastMessage;
            if ((dgUserEdit.Rows[e.RowIndex].DataBoundItem as User).HaveError)
            {
                dgUserEdit.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            }
        }

        private void dgAccountInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgAccountInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Classes.AppSetting.MinDelay = int.Parse(txtMin.Text);
            Classes.AppSetting.MaxDelay = int.Parse(txtMax.Text);
            Classes.AppSetting.AutoSatrt = chkAutoStart.Checked;
            Classes.AppSetting.ThreadCount = cmbThreadCount.SelectedIndex + 1;
            btnApply.Enabled = false;
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            CheckForEnableApplyButton();
        }

        private void CheckForEnableApplyButton()
        {
            btnApply.Enabled = false;
            var b1 = !string.IsNullOrWhiteSpace(txtMax.Text) ||
                !string.IsNullOrWhiteSpace(txtMin.Text);
            if (!b1) return;
            int x1 = 0;
            int x2 = 1;
            if (!int.TryParse(txtMin.Text, out x1)) return;
            if (!int.TryParse(txtMax.Text, out x2)) return;
            if (x1 > x2) return;

            if (x1 == Classes.AppSetting.MinDelay
                && x2 == Classes.AppSetting.MaxDelay
                && chkAutoStart.Checked == Classes.AppSetting.AutoSatrt
                && cmbThreadCount.SelectedIndex + 1 == Classes.AppSetting.ThreadCount
                ) return;

            btnApply.Enabled = true;
        }

        private void txtMax_TextChanged(object sender, EventArgs e)
        {
            CheckForEnableApplyButton();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            ProgressTimer.Enabled = true;
            this.btnStop.Enabled = !(this.btnPlay.Enabled = false);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ProgressTimer.Enabled = false;
            this.Invoke(new MethodInvoker(delegate()
            {
                this.btnStop.Enabled = !(this.btnPlay.Enabled = true);
            }));
        }

        private void cmbThreadCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForEnableApplyButton();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            var vTime = new Random().Next(Classes.AppSetting.MinDelay, Classes.AppSetting.MaxDelay);
            vTime = vTime * 1000;
            ProgressTimer.Interval = vTime;
            ProgressTimer.Enabled = false;

            new System.Threading.Thread(new System.Threading.ThreadStart(DoOneRound)) { IsBackground = true }.Start();
        }

        private void DoOneRound()
        {
            List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
            bool b1 = false;

            foreach (User user in Program.Database.Users.Where(aa => aa.Enabled).OrderBy(aa => Guid.NewGuid()).Take(Classes.AppSetting.ThreadCount))
            {
                System.Threading.Thread T = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DoOneRoundForUser));
                threads.Add(T);
                b1 = true;
                T.IsBackground = true;
                T.Start(user);

            }
            if (!b1) btnStop_Click(btnStop, new EventArgs());
            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            if (btnStop.Enabled)
            {
                System.Threading.Thread.Sleep(ProgressTimer.Interval);
                ProgressTimer_Tick(ProgressTimer, new EventArgs());
            }
        }

        private void DoOneRoundForUser(object UObj)
        {
            var User = UObj as User;
            var tmpuser = Program.Database.Users.FirstOrDefault(aa => aa.UserName == User.UserName);

            if (!tmpuser.IsLogin)
            {
                try
                {
                    if (!Classes.PinterestHelper.Login(ref tmpuser)) return;
                }
                catch (Exception ex)
                {
                    User.LastMessage = ex.Message;
                    tmpuser.Enabled = false;
                    return;
                }
            }

            for (int i = 0; i < 1; i++)
            {
                try
                {
                    PinterestFollow.Classes.PinterestHelper.SetUserInfo(User);
                    Database_DatabaseChanged(Program.Database.Users, new EventArgs());

                }
                catch (Exception)
                {
                    return;
                }

                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DoStepSurffing));
                thread.IsBackground = true;
                thread.Start(tmpuser);


                System.Threading.Thread thread2 = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DoOnStepUnFollow));
                thread2.IsBackground = true;
                thread2.Start(tmpuser);

                thread.Join(); thread2.Join();
            }
        }
        private void DoOnStepUnFollow(object tmpuser)
        {
            DoOnStepUnFollow(tmpuser as User);
        }
        /// <summary>
        /// Do One Step Un Followe For User
        /// </summary>
        /// <param name="tmpuser">User</param>
        private void DoOnStepUnFollow(User UserToUnfollow)
        {

            if (UserToUnfollow.TodayUnfollowd >= UnFollowCount) return;
            
            if (xRoundCoutn % 20 == 0)
            {
                string strUrl = string.Format("http://pinterest.com/{0}/following/?page={1}", UserToUnfollow.pUsername, new Random().Next(0, int.MaxValue));
                List<string> Users = PinterestFollow.Classes.PinterestHelper.GetUserFromPage(strUrl, UserToUnfollow);
                var newUsers = Users.Where(aa => !UserToUnfollow.UsersFollowed.Select(bb => bb.UserName).Contains(aa));

                foreach (var item in newUsers)
                {
                    UserToUnfollow.UsersFollowed.Add(new FollowedUser()
                    {
                        UserName = item,
                        DateStartFollowing = DateTime.Now
                    });
                }

                xRoundCoutn++;
            }
            else
            {
                var vCurrent = UserToUnfollow.UsersFollowed.Where(aa => DateTime.Now.Subtract(aa.DateStartFollowing).TotalHours >= Classes.AppSetting.TotalHours).OrderBy(aa => Guid.NewGuid()).FirstOrDefault();

                if (vCurrent != null)
                    if (PinterestFollow.Classes.PinterestHelper.UnFollowUser(vCurrent.UserName, UserToUnfollow))
                    {
                        UserToUnfollow.UsersFollowed.Remove(UserToUnfollow.UsersFollowed.FirstOrDefault(aa => aa.UserName == vCurrent.UserName));
                        UserToUnfollow.TodayUnfollowd++;
                        Database_DatabaseChanged(Program.Database.Users, new EventArgs());
                    }

            }

        }
        private void DoStepSurffing(object User)
        {
            DoStepSurffing(User as User);
        }
        private void DoStepSurffing(User User)
        {


            if (User.TodayFollow >= FollowCount) return;

            if (User.LastVistingLink == "HP")
            {
                User.LastVistingLink = string.Format("http://pinterest.com/{0}/following/", User.pUsername, new Random().Next(300));
            }
            try
            {
                var str = User.LastVistingLink;
                if (User.LastVistingLink.IndexOf("?page") < 0)
                {
                    str = User.LastVistingLink + string.Format("?page={0}", new Random().Next(0, int.MaxValue));
                }
                List<string> users = PinterestFollow.Classes.PinterestHelper.GetUserFromPage(str, User);
                users = new List<string>(users.Where(aa => aa != User.pUsername).ToArray());

                if (users == null) { SetInvalidUser(User); return; }

                var LastUser = "";

                foreach (var item in users)
                {
                    if (User.following == null) User.following = new List<string>();
                    User.following.Add(item);
                }
                if (User.following != null && User.following.Count >= 4000)
                {
                    int x = User.following.Count;
                    for (int i = 0; i < x - 400; i++)
                    {
                        User.following.RemoveAt(0);
                    }
                }
                foreach (var item in users.Take(1))
                {
                    try
                    {
                        if (PinterestFollow.Classes.PinterestHelper.FollowUser(item, User))
                        {
                            User.UsersFollowed.Add(new FollowedUser() { DateStartFollowing = DateTime.Now, UserName = item });
                            User.TodayFollow++;
                        }
                    }
                    catch (Exception ex)
                    {
                        User.LastMessage = ex.Message;
                        User.HaveError = true;
                        return;
                    }
                    LastUser = item;
                }

                SetLastUrl(User, LastUser);
            }
            catch (Exception ex)
            {
                User.LastMessage = ex.Message;
                User.HaveError = true;
            }


            Database_DatabaseChanged(Program.Database.Users, new EventArgs());
        }

        private void SetLastUrl(User User, string LastUser)
        {

            if (User.following == null) User.following = new List<string>();
            var las = (User.following.OrderBy(aa => Guid.NewGuid()) == null) ? null : User.following.OrderBy(aa => Guid.NewGuid()).FirstOrDefault();

            if (las == null)
            {
                if (string.IsNullOrWhiteSpace(LastUser)) LastUser = User.pUsername;
                var url = string.Format("http://pinterest.com/{0}/following/", LastUser);
                var user = Program.Database.Users.FirstOrDefault(aa => aa.UserName == User.UserName);
                user.LastVistingLink = url;
            }
            else
            {
                var url = string.Format("http://pinterest.com/{0}/following/", las);
                var user = Program.Database.Users.FirstOrDefault(aa => aa.UserName == User.UserName);
                if (user != null)
                    user.LastVistingLink = url;
            }
        }

        private void SetInvalidUser(User User)
        {
            var user = Program.Database.Users.FirstOrDefault(aa => aa.UserName == User.UserName);
            user.HaveError = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Classes.AppSetting.TotalHours = (int)nmHours.Value;

            Classes.AppSetting.MinF = (int)MinF.Value;

            Classes.AppSetting.MaxF = (int)MaxF.Value;

            Classes.AppSetting.MinU = (int)MinU.Value;

            Classes.AppSetting.MaxU = (int)MaxU.Value;
        }

    }
}
