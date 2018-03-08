using EasyHttp.Http;
using LabelPrint.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrint.Inventory
{
    public partial class TransferReceiptsDetails : Form
    {
        public int ID;
        public string PackageID;
        public DataTable dt_lots;

        public TransferReceiptsDetails()
        {
            InitializeComponent();
        }

        private void loadData()
        {
            txtPackageID.Text = PackageID;

            #region Load list Lots
            string url = "transfer-details/search?query=destPackageNumber==" + PackageID;
            var param = new { };
            HttpResponse res = HTTP.Instance.Get(url, param);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<ExpandoObject> list_request_paper = new List<ExpandoObject>(res.DynamicBody);
                    dt_lots = Common.ToDataTable(list_request_paper);
                    this.vgListLot.DataSource = dt_lots;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không load được dữ liệu !");
                };
            }
            else
            {
                //messageShow.ShowHint("không có dữ liệu");
            }
            #endregion
        }

        private void TransferReceiptsDetails_Load(object sender, EventArgs e)
        {
            loadData();
        }
    }
}
