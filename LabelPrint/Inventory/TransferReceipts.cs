using EasyHttp.Http;
using LabelPrint.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace LabelPrint.Inventory
{
    public partial class TransferReceipts : Form
    {
        private CultureInfo culture;
        public DataTable dt_request_paper;

        public TransferReceipts()
        {
            InitializeComponent();
            culture = CultureInfo.CurrentCulture;           
        }

        private void TransferReceipts_Load(object sender, EventArgs e)
        {
            imgCbxLanguage.EditValue = 1;
            SetLanguage("vi-VN");
            
            loadFormData();
        }

        #region Language
        private void SetLanguage(string cultureName)
        {
            culture = CultureInfo.CreateSpecificCulture(cultureName);
            ResourceManager rm = new ResourceManager("LabelPrint.Lang.Language", typeof(TransferReceipts).Assembly);

            lblRequest.Text = rm.GetString("request", culture);
            lblName.Text = rm.GetString("transfer_receipt", culture);
            tabID.Text = rm.GetString("tab_id", culture);
            tabTransfer.Text = rm.GetString("tab_transfer", culture);
        }

        private void imgCbxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(imgCbxLanguage.EditValue.ToString());
            if (imgCbxLanguage.EditValue.ToString() == "0")
                SetLanguage("en-US");
            else
                SetLanguage("vi-VN");
        }
        #endregion

        private void loadFormData()
        {
            #region Init data form
            lblRequestName.Text = "";
            lblDocumentValue.Text = "";
            lblPartnerValue.Text = "";
            lblSourceLocationValue.Text = "";
            lblCreatedValue.Text = "";
            #endregion

            #region Load list transfer
            string url = "transfers";
            var param = new {};
            HttpResponse res = HTTP.Instance.Get(url, param);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<ExpandoObject>  list_request_paper = new List<ExpandoObject>(res.DynamicBody);
                    dt_request_paper = Common.ToDataTable(list_request_paper);

                    if (dt_request_paper.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_request_paper.Rows.Count; i++)
                        {
                            cbxRequestPaper.Properties.Items.Add(dt_request_paper.Rows[i]["transferNumber"].ToString());
                            //var item = new
                            //{
                            //    id = Common.ConvertInt(dt_request_paper.Rows[i]["id"]),
                            //    name = dt_request_paper.Rows[i]["transferNumber"].ToString()
                            //};
                            //cbxRequestPaper.Properties.Items.Add(item);
                        }

                        //cbxRequestPaper.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("Các thao tác trong Lệnh sản xuất chưa được cấp tài nguyên số !");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Không load được dữ liệu !");
                };
            }
            else
            {
                //messageShow.ShowHint("không có dữ liệu");
            }
            #endregion

            cbxViewPackage.Properties.Items.Add("All");
            cbxViewPackage.Properties.Items.Add("Package");
            cbxViewPackage.Properties.Items.Add("Lot");
        }

        private void cbxRequestPaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(cbxRequestPaper.EditValue.ToString());
            //DataRow row = DataTableExt.FindInMultiPKey(dt_request_paper, cbxRequestPaper.EditValue.ToString());
            //DataRow[] row = DataTableExt.SearchExpression(dt_request_paper, "id = 4");

            DataRow row = DataTableExt.FindByKey(dt_request_paper, "transferNumber", cbxRequestPaper.EditValue.ToString());

            string url = "transfers/" + row["id"].ToString();
            var param = new { };
            HttpResponse res = HTTP.Instance.Get(url, param);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    dynamic data = serializer.Deserialize(res.RawText, typeof(object));

                    lblRequestName.Text = data["transferNumber"];

                    #region Get partner information
                    try { 
                        string url_partner = "companies/" + Convert.ToString(data["partnerId"]);
                        var param_partner = new { };
                        HttpResponse res_partner = HTTP.Instance.Get(url_partner, param_partner);

                        if (res_partner.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                var serializer_partner = new JavaScriptSerializer();
                                dynamic data_partner = serializer_partner.Deserialize(res_partner.RawText, typeof(object));
                                lblPartnerValue.Text = data_partner["name"];
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Không tồn tại dữ liệu !");
                            };
                        }

                    } catch { lblPartnerValue.Text = ""; };
                    #endregion

                    #region Get source location information
                    try {
                        string url_srcLocation = "locations/" + Convert.ToString(data["srcLocationId"]);
                        var param_srcLocation = new { };
                        HttpResponse res_srcLocation = HTTP.Instance.Get(url_srcLocation, param);

                        if (res_srcLocation.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                var serializer_srcLocation = new JavaScriptSerializer();
                                dynamic data_srcLocation = serializer_srcLocation.Deserialize(res_srcLocation.RawText, typeof(object));
                                lblSourceLocationValue.Text = data_srcLocation["completeName"];
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Không tồn tại dữ liệu !");
                            };
                        }

                    } catch { lblSourceLocationValue.Text = ""; };
                    #endregion

                    try { lblCreatedValue.Text = Convert.ToString(data["created"]); } catch { lblCreatedValue.Text = ""; };
                    try { lblDocumentValue.Text = (string)data["sourceDocument"]; } catch { lblDocumentValue.Text = ""; };

                    #region Get list part in request paper
                    try
                    {
                        string url_list_part = "transfer-items/search?query=transferId==" + Convert.ToString(row["id"]);
                        var param_list_part = new { };
                        HttpResponse res_list_part = HTTP.Instance.Get(url_list_part, param_list_part);

                        if (res_list_part.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                var serializer_list_part = new JavaScriptSerializer();
                                dynamic data_list_part = serializer_list_part.Deserialize(res_list_part.RawText, typeof(object));
                                
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Không tồn tại dữ liệu !");
                            };
                        }

                    }
                    catch { lblPartnerValue.Text = ""; };
                    #endregion
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Không tồn tại dữ liệu !");
                };
            }
            else
            {
                //messageShow.ShowHint("không có dữ liệu");
            }
        }

        private void tabTransfer_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
