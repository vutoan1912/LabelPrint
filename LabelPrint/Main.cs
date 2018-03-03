using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyHttp.Http;
using LabelPrint.Business;
using System.Web.Script.Serialization;
using System.Net;
using LabelPrint.App_Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using LabelPrint.Dashboard;

namespace LabelPrint
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            ApiResponse api_res = new ApiResponse();
            string url = "auth/token";

            //var param = new
            //{
            //    email = txtUser.Text.Trim(),
            //    password = txtPass.Text.Trim(),
            //    rememberMe = checkRemember.Checked
            //};

            var param = "{\"email\": \"" + txtUser.Text.Trim() + "\",\"password\": \"" + txtPass.Text.Trim() + "\",\"rememberMe\": \"" + checkRemember.Checked + "\" }";

            HttpResponse res = HTTP.Instance.Post(url, param);

            var serializer = new JavaScriptSerializer();
            dynamic data = serializer.Deserialize(res.RawText, typeof(object));

            if (res.StatusCode != HttpStatusCode.OK)
            {
                lblError.Text = "Failed to sign in! Please check your credentials and try again";
                lblError.Visible = true;
            }
            else
            {
                Config.API_KEY = "Bearer " + (string)data["id_token"];
                
                var param_current_user = new { };
                HttpResponse res_current_user = HTTP.Instance.Get("users/current", param_current_user);

                if (res_current_user.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var serializer_current_user = new JavaScriptSerializer();
                    dynamic _data = serializer_current_user.Deserialize(res_current_user.RawText, typeof(object));

                    // parse data
                    List<base_user> arr_user = new List<base_user>();
                    int i = 1;
                    foreach (var auth in _data["authorities"])
                    {
                        base_user item = new base_user();
                        item.id = i;
                        item.user = txtUser.Text.Trim();
                        item.authorities = auth;
                        arr_user.Add(item);
                        i++;
                    }

                    UserRepository user = new UserRepository();
                    //IList<base_user> arr_delete = user.GetAll();
                    //user.Remove(arr_delete.ToArray());
                    user.Add(arr_user.ToArray());

                    this.Hide();
                    var dashboard = new DashboardInventory();
                    dashboard.Closed += (s, args) => this.Close();
                    dashboard.Show();

                    //Dashboard.DashboardInventory dashboard = new Dashboard.DashboardInventory();
                    //dashboard.ShowDialog();
                }
                else
                {
                    lblError.Text = "Failed to sign in! Please check your credentials and try again";
                    lblError.Visible = true;
                }
            }
        }
    }
}
