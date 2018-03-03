using EasyHttp.Http;
using LabelPrint.App_Data;
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
        public DataTable dt_products;
        public List<wh_transfer_details> list_transfer_details;

        public TransferReceipts()
        {
            InitializeComponent();
            culture = CultureInfo.CurrentCulture;
        }

        private void TransferReceipts_Load(object sender, EventArgs e)
        {
            imgCbxLanguage.SelectedIndex = 1;
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

                    gluTransferNumber.Properties.DataSource = dt_request_paper;
                    gluTransferNumber.Properties.DisplayMember = "transferNumber";
                    gluTransferNumber.Properties.ValueMember = "id";
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

            #region Load list Location
            string url_location = "locations";
            var param_location = new { };
            HttpResponse res_location = HTTP.Instance.Get(url_location, param_location);

            if (res_location.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<ExpandoObject> list_location = new List<ExpandoObject>(res_location.DynamicBody);
                    DataTable dt_location = Common.ToDataTable(list_location);

                    this.gluSourceLocation.Properties.DataSource = dt_location;
                    this.gluSourceLocation.Properties.DisplayMember = "name";
                    this.gluSourceLocation.Properties.ValueMember = "id";

                    this.gluDestinationLocation.Properties.DataSource = dt_location;
                    this.gluDestinationLocation.Properties.DisplayMember = "name";
                    this.gluDestinationLocation.Properties.ValueMember = "id";
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

            #region Load list uom
            string url_uom = "uoms";
            var param_uom = new { };
            HttpResponse res_uom = HTTP.Instance.Get(url_uom, param_uom);

            if (res_uom.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<ExpandoObject> list_uom = new List<ExpandoObject>(res_uom.DynamicBody);
                    DataTable dt_uom = Common.ToDataTable(list_uom);

                    this.gluUomPackage.Properties.DataSource = dt_uom;
                    this.gluUomPackage.Properties.DisplayMember = "name";
                    this.gluUomPackage.Properties.ValueMember = "id";

                    this.gluUomLot.Properties.DataSource = dt_uom;
                    this.gluUomLot.Properties.DisplayMember = "name";
                    this.gluUomLot.Properties.ValueMember = "id";
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

            cbxViewPackage.Properties.Items.Add("All");
            cbxViewPackage.Properties.Items.Add("Package");
            cbxViewPackage.Properties.Items.Add("Lot");
            cbxViewPackage.SelectedIndex = 0;
        }

        private void cbxRequestPaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void tabTransfer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbxRequestPaper_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(cbxRequestPaper.EditValue.ToString());
            //DataRow row = DataTableExt.FindInMultiPKey(dt_request_paper, cbxRequestPaper.EditValue.ToString());
            //DataRow[] row = DataTableExt.SearchExpression(dt_request_paper, "id = 4");

            //DataRow row = DataTableExt.FindByKey(dt_request_paper, "transferNumber", this.gluTransferNumber.EditValue.ToString());

            string url = "transfers/" + this.gluTransferNumber.EditValue.ToString();
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
                    try
                    {
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

                    }
                    catch { lblPartnerValue.Text = ""; };
                    #endregion

                    #region Get source location information
                    try
                    {
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

                    }
                    catch { lblSourceLocationValue.Text = ""; };
                    #endregion

                    try { lblCreatedValue.Text = Convert.ToString(data["created"]); } catch { lblCreatedValue.Text = ""; };
                    try { lblDocumentValue.Text = (string)data["sourceDocument"]; } catch { lblDocumentValue.Text = ""; };

                    #region Get list part in request paper
                    try
                    {
                        string url_list_part = "transfer-items/search?query=transferId==" + this.gluTransferNumber.EditValue.ToString();
                        var param_list_part = new { };
                        HttpResponse res_list_part = HTTP.Instance.Get(url_list_part, param_list_part);

                        if (res_list_part.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                //var serializer_list_part = new JavaScriptSerializer();
                                //dynamic data_list_part = serializer_list_part.Deserialize(res_list_part.RawText, typeof(object));

                                List<ExpandoObject> list_parts = new List<ExpandoObject>(res_list_part.DynamicBody);
                                dt_products = Common.ToDataTable(list_parts);

                                //foreach (var item in data_list_part)
                                //{
                                //    var row_part = this.dt_products.NewRow();
                                //    row_part["productName"] = item["productName"];
                                //    row_part["productId"] = item["productId"];
                                //    row_part["manPn"] = item["manPn"];
                                //    row_part["initialQuantity"] = item["initialQuantity"];
                                //    row_part["doneQuantity"] = item["doneQuantity"];
                                //    row_part["srcLocationId"] = item["srcLocationId"];
                                //    row_part["destLocationId"] = item["destLocationId"];
                                //    row_part["spq"] = item["spq"];
                                //    this.dt_products.Rows.Add(row_part);
                                //}

                                this.vgListProduct.DataSource = dt_products;
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Không tồn tại dữ liệu !");
                            };
                        }
                    }
                    catch { lblPartnerValue.Text = ""; };
                    #endregion

                    this.gluSourceLocation.EditValue = data["srcLocationId"];
                    this.gluDestinationLocation.EditValue = data["destLocationId"];
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

        private void grvListPart_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                //MessageBox.Show("Focus state: focused row - " + grvListPart.FocusedRowHandle.ToString());
                //MessageBox.Show("Selection state: selected row - " + grvListPart.GetSelectedRows().FirstOrDefault());

                //var data = grvListPart.GetDataRow(grvListPart.GetSelectedRows().FirstOrDefault());

                //MessageBox.Show("Selection state: selected row - " + grvListPart.GetSelectedRows());
            }
            ));
        }

        private void gluUomPackage_EditValueChanged(object sender, EventArgs e)
        {
            MessageBox.Show(gluUomPackage.EditValue.ToString());
        }

        private void btnAllocate_Click(object sender, EventArgs e)
        {
            bool status_reserve = true;
            List<string> List_allocate_package = new List<string>();
            List<string> List_allocate_lot = new List<string>();

            //allocate package
            string url_package = "sequences/3/reserve";
            var param_package = new {
                id = 3,
                quantity = txtNumberPackage.Text
            };
            HttpResponse res_package = HTTP.Instance.Get(url_package, param_package);

            if (res_package.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var serializer_package = new JavaScriptSerializer();
                    dynamic data_package = serializer_package.Deserialize(res_package.RawText, typeof(object));

                    List_allocate_package = Common.CreateSequential(Common.ConvertInt(data_package["nextNumber"]), Common.ConvertInt(data_package["step"]), Common.ConvertInt(data_package["length"]), Common.ConvertInt(txtNumberPackage), data_package["prefix"]);
                }
                catch (Exception ex)
                {
                    status_reserve = false;
                };
            }
            else
            {
                status_reserve = false;
            }

            //allocate lot
            string url_lot = "sequences/10/reserve";
            var param_lot = new
            {
                id = 3,
                quantity = txtNumberLot.Text
            };
            HttpResponse res_lot = HTTP.Instance.Get(url_lot, param_lot);

            if (res_lot.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var serializer_lot = new JavaScriptSerializer();
                    dynamic data_lot = serializer_lot.Deserialize(res_lot.RawText, typeof(object));

                    List_allocate_lot = Common.CreateSequential(Common.ConvertInt(data_lot["nextNumber"]), Common.ConvertInt(data_lot["step"]), Common.ConvertInt(data_lot["length"]), Common.ConvertInt(txtNumberPackage), data_lot["prefix"]);
                }
                catch (Exception ex)
                {
                    status_reserve = false;
                };
            }
            else
            {
                status_reserve = false;
            }

            DataRow data = grvListPart.GetDataRow(grvListPart.GetSelectedRows().FirstOrDefault());
            long maxID = 0;
            using (erpEntities dbContext = new erpEntities())
            {
                wh_transfer_details wtd = dbContext.wh_transfer_details.OrderByDescending(u => u.id).FirstOrDefault();
                if (wtd != null) maxID = wtd.id + 1;
            }

            if (status_reserve)
            {
                TransferDetailsRepository TransferDetails = new TransferDetailsRepository();
                list_transfer_details = new List<wh_transfer_details>();
                foreach (string pack in List_allocate_package)
                {
                    wh_transfer_details wtd = new wh_transfer_details();
                    wtd.created = DateTime.Now;
                    wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                    wtd.dest_package_number = pack;
                    wtd.done_quantity = Common.ConvertDouble(txtNumberPerPackage.Text);
                    wtd.id = maxID;
                    wtd.man_id = Common.ConvertInt(data["manId"]);
                    wtd.man_pn = Convert.ToString(data["manPn"]);
                    wtd.product_id = Common.ConvertInt(data["productId"]);
                    wtd.product_name = Convert.ToString(data["productName"]);
                    wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                    wtd.status = 0;
                    wtd.transfer_id = Common.ConvertInt(gluTransferNumber.EditValue);
                    wtd.transfer_item_id = Common.ConvertInt(data["id"]);
                    wtd.trace_number = null;
                    wtd.src_package_number = txtSourceNumber.Text;
                    list_transfer_details.Add(wtd);

                    maxID++;
                }

                int count_lot_per_package = 0; int i = 0;
                foreach (string lot in List_allocate_lot)
                {
                    wh_transfer_details wtd = new wh_transfer_details();
                    wtd.created = DateTime.Now;
                    wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                    wtd.dest_package_number = List_allocate_package[i];
                    wtd.done_quantity = Common.ConvertDouble(txtNumberPerPackage.Text);
                    wtd.id = maxID;
                    wtd.man_id = Common.ConvertInt(data["manId"]);
                    wtd.man_pn = Convert.ToString(data["manPn"]);
                    wtd.product_id = Common.ConvertInt(data["productId"]);
                    wtd.product_name = Convert.ToString(data["productName"]);
                    wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                    wtd.status = 0;
                    wtd.transfer_id = Common.ConvertInt(gluTransferNumber.EditValue);
                    wtd.transfer_item_id = Common.ConvertInt(data["id"]);
                    wtd.trace_number = lot;
                    list_transfer_details.Add(wtd);

                    maxID++;
                    count_lot_per_package++;
                    if (count_lot_per_package >= 5) i++;
                }

                TransferDetails.Add(list_transfer_details.ToArray());
            }
            else
            {
                MessageBox.Show("There is an error in the allocation process");
            }
        }

        private void txtNumberPerPackage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtNumberPerLot.Text != null && txtNumberPerLot.Text.Length > 0)
                {
                    if (Common.ConvertDouble(txtNumberPerPackage.Text) % Common.ConvertDouble(txtNumberPerLot.Text) == 0)
                        txtNumberLot.Text = Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text);
                    else
                        txtNumberLot.Text = (Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text)) + 1;
                }
                else if (txtNumberLot.Text != null && txtNumberLot.Text.Length > 0)
                {
                    txtNumberPerLot.Text = Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberLot.Text);
                }
            }
        }

        private void txtNumberPerLot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Common.ConvertDouble(txtNumberPerPackage.Text) % Common.ConvertDouble(txtNumberPerLot.Text) == 0)
                    txtNumberLot.Text = Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text);
                else
                    txtNumberLot.Text = (Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text)) + 1;
            }
        }

        private void txtNumberLot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Common.ConvertDouble(txtNumberPerPackage.Text) % Common.ConvertDouble(txtNumberLot.Text) == 0)
                    txtNumberPerLot.Text = Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberLot.Text);
                else
                    txtNumberPerLot.Text = (Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberLot.Text)) + 1;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            LabelPackage labelPackage = new LabelPackage("HY5ND35N000001NA", "PKG0000000072", "");
            labelPackage.Template();
        }
    }
}