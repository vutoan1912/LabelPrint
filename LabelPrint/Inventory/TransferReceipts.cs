﻿using DevExpress.XtraEditors.Repository;
using EasyHttp.Http;
using LabelPrint.App_Data;
using LabelPrint.Business;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace LabelPrint.Inventory
{
    public partial class TransferReceipts : Form
    {
        private CultureInfo culture;
        public DataTable dt_request_paper;
        public DataTable dt_products;
        public DataTable dt_trans_details = new DataTable();
        public List<wh_transfer_details> list_transfer_details;
        public Dictionary<string, dynamic> transfer_info;
        private StringBuilder code_read = new StringBuilder();
        //private string code_data = null;
        private Thread ThreadScan;
        private Queue<string> QueueScan = new Queue<string>();
        private Control FocusedControl;
        private bool IsTransferScan = false;

        //search
        private StringBuilder key_search_transfer = new StringBuilder();

        public TransferReceipts()
        {
            InitializeComponent();
            culture = CultureInfo.CurrentCulture;

            this.Focus();
            this.KeyPreview = true;
        }

        private void TransferReceipts_Load(object sender, EventArgs e)
        {
            imgCbxLanguage.SelectedIndex = 1;
            SetLanguage("vi-VN");
            
            loadFormData();

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (ThreadScan == null || !ThreadScan.IsAlive)
            {
                ThreadScan = new Thread(new ThreadStart(PopQueueScan));
                ThreadScan.Start();
                Console.WriteLine("Pop Queue Scan!!!");
            }
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
            string url = "transfers/search?query=&size=15";
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
            string url_location = "locations/search?query=&size=15";
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
            string url_uom = "uoms/search?query=&size=15";
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
            loadDataTransfer();
        }

        private void loadDataTransfer()
        {
            if (this.gluTransferNumber.EditValue != null && IsTransferScan == false)
            {
                string url = "transfers/" + this.gluTransferNumber.EditValue.ToString();
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));

                        transfer_info = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res.RawText);

                        lblRequestName.Text = data["transferNumber"];

                        #region Get operations type
                        try
                        {
                            string url_op = "operation-types/" + Convert.ToString(data["operationTypeId"]);
                            var param_op = new { };
                            HttpResponse res_op = HTTP.Instance.Get(url_op, param_op);

                            if (res_op.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                try
                                {
                                    var serializer_op = new JavaScriptSerializer();
                                    dynamic data_op = serializer_op.Deserialize(res_op.RawText, typeof(object));

                                    if (data_op["type"] != "manufacturing")
                                    {
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

                                        this.gluSourceLocation.EditValue = data["srcLocationId"];
                                        this.gluDestinationLocation.EditValue = data["destLocationId"];

                                        panelControl2.Enabled = true;
                                    }
                                    else
                                    {
                                        panelControl2.Enabled = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                };
                            }

                        }
                        catch { };
                        #endregion

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

                        try { lblCreatedValue.Text = Convert.ToString(data["created"]); } catch { lblCreatedValue.Text = ""; };
                        try { lblDocumentValue.Text = (string)data["sourceDocument"]; } catch { lblDocumentValue.Text = ""; };

                        #region Get list part in request paper
                        try
                        {
                            string url_list_part = "transfer-items/search?query=transferId==" + this.gluTransferNumber.EditValue.ToString() + "&size=2000";
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

                                    try
                                    {
                                        this.vgListProduct.DataSource = dt_products;
                                        loadDataListPackage();
                                    }
                                    catch (Exception ex)
                                    {
                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.vgListProduct.DataSource = null;
                                    MessageBox.Show("List product is empty !");
                                };
                            }
                        }
                        catch { this.vgListProduct.DataSource = null; };
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không tải được dữ liệu transfer !");
                    };
                }
                else
                {
                    MessageBox.Show("Lỗi tải dữ liệu transfer !");
                    this.vgListProduct.DataSource = null;
                    this.vgListPackage.DataSource = null;
                }
            }

        }

        private void loadDataTransferScan()
        {
            if (this.gluTransferNumber.EditValue != null && IsTransferScan == true)
            {
                string url = "transfers/" + this.gluTransferNumber.EditValue.ToString();
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));

                        transfer_info = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res.RawText);

                        this.lblRequestName.Invoke(new MethodInvoker(delegate { lblRequestName.Text = data["transferNumber"]; }));

                        #region Get operations type
                        try
                        {
                            string url_op = "operation-types/" + Convert.ToString(data["operationTypeId"]);
                            var param_op = new { };
                            HttpResponse res_op = HTTP.Instance.Get(url_op, param_op);

                            if (res_op.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                try
                                {
                                    var serializer_op = new JavaScriptSerializer();
                                    dynamic data_op = serializer_op.Deserialize(res_op.RawText, typeof(object));

                                    if (data_op["type"] != "manufacturing")
                                    {
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

                                                    this.lblSourceLocationValue.Invoke(new MethodInvoker(delegate { lblSourceLocationValue.Text = data_srcLocation["completeName"]; }));
                                                }
                                                catch (Exception ex)
                                                {
                                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                                };
                                            }
                                        }
                                        catch { lblSourceLocationValue.Text = ""; };
                                        #endregion

                                        this.gluSourceLocation.Invoke(new MethodInvoker(delegate { this.gluSourceLocation.EditValue = data["srcLocationId"]; }));
                                        this.gluDestinationLocation.Invoke(new MethodInvoker(delegate { this.gluDestinationLocation.EditValue = data["destLocationId"]; }));

                                        panelControl2.Enabled = true;
                                    }
                                    else
                                    {
                                        panelControl2.Enabled = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                };
                            }

                        }
                        catch { };
                        #endregion

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

                                    this.lblPartnerValue.Invoke(new MethodInvoker(delegate { lblPartnerValue.Text = data_partner["name"]; }));
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                };
                            }

                        }
                        catch { lblPartnerValue.Text = ""; };
                        #endregion

                        this.lblCreatedValue.Invoke(new MethodInvoker(delegate { lblCreatedValue.Text = Convert.ToString(data["created"]); }));
                        this.lblDocumentValue.Invoke(new MethodInvoker(delegate { lblDocumentValue.Text = (string)data["sourceDocument"]; }));

                        #region Get list part in request paper
                        try
                        {
                            string url_list_part = "transfer-items/search?query=transferId==" + this.gluTransferNumber.EditValue.ToString() + "&size=2000";
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

                                    this.vgListProduct.Invoke(new MethodInvoker(delegate { vgListProduct.DataSource = dt_products; }));
                                }
                                catch (Exception ex)
                                {
                                    this.vgListProduct.DataSource = null;
                                };
                            }
                        }
                        catch { this.vgListProduct.DataSource = null; };
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không tải được dữ liệu transfer !");
                    };
                }
                else
                {
                    MessageBox.Show("Lỗi tải dữ liệu transfer !");
                    this.vgListProduct.DataSource = null;
                    this.vgListPackage.DataSource = null;
                }
                IsTransferScan = false;
            }
            
        }

        private void grvListPart_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                //MessageBox.Show("Focus state: focused row - " + grvListPart.FocusedRowHandle.ToString());
                //MessageBox.Show("Selection state: selected row - " + grvListPart.GetSelectedRows().FirstOrDefault());

                loadDataListPackage();
            }
            ));
        }

        private void loadDataListPackage()
        {
            if (grvListPart.DataRowCount > 0)
            {
                var data = grvListPart.GetDataRow(grvListPart.GetSelectedRows().FirstOrDefault());
                try { lblProductName.Text = Convert.ToString(data["productName"]); }
                catch { };

                try
                {
                    #region Load list Transfer Details
                    string url_trans_details = "transfer-details/search?query=transferItemId==" + data["id"] + "&size=3000";
                    var param_trans_details = new { };
                    HttpResponse res_trans_details = HTTP.Instance.Get(url_trans_details, param_trans_details);

                    if (res_trans_details.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        try
                        {
                            List<ExpandoObject> list_trans_details = new List<ExpandoObject>(res_trans_details.DynamicBody);
                            int index = list_trans_details.Count > 1 ? list_trans_details.Count - 1 : 0;
                            dt_trans_details = Common.ToDataTable(list_trans_details, _length: index);
                            vgListPackage.DataSource = dt_trans_details;
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Không load được dữ liệu !");
                            vgListPackage.DataSource = null;
                        };
                    }
                    else
                    {
                        //messageShow.ShowHint("không có dữ liệu");
                        vgListPackage.DataSource = null;
                    }
                    #endregion
                }
                catch (Exception ex) { }
            }
            else
            {
                vgListPackage.DataSource = null;
            }
            
                
        }

        private void gluUomPackage_EditValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(gluUomPackage.EditValue.ToString());

        }

        private void btnAllocate_Click(object sender, EventArgs e)
        {
            if (txtNumberPerPackage.Text.Trim().Length == 0 && txtNumberPackage.Text.Trim().Length == 0
                && txtNumberPerLot.Text.Trim().Length == 0 && txtNumberLot.Text.Trim().Length == 0)
            {
                MessageBox.Show("The request to enter required information is missing");
                return;
            }

            //if (Common.ConvertDouble(txtNumberPerPackage.Text) < (Common.ConvertDouble(txtNumberPerLot.Text) * Common.ConvertInt(txtNumberLot.Text)))
            //{
            //    MessageBox.Show("The total amount of lot is greater than the number of the package");
            //    return;
            //}

            bool status_reserve = true;
            List<string> List_allocate_package = new List<string>();
            List<string> List_allocate_lot = new List<string>();

            if (txtNumberPerPackage.Text.Trim().Length > 0 && txtNumberPackage.Text.Trim().Length > 0 && Common.ConvertInt(txtNumberPackage.Text.Trim()) > 0)
            {
                //allocate package
                string url_package = "sequences/4/reserve/" + txtNumberPackage.Text;
                HttpResponse res_package = HTTP.Instance.Post(url_package, null);

                if (res_package.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer_package = new JavaScriptSerializer();
                        dynamic data_package = serializer_package.Deserialize(res_package.RawText, typeof(object));

                        List_allocate_package = Common.CreateSequential(Common.ConvertInt(data_package["nextNumber"]), Common.ConvertInt(data_package["step"]), Common.ConvertInt(data_package["length"]), Common.ConvertInt(txtNumberPackage.Text), data_package["prefix"]);
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
            }

            if (txtNumberPerLot.Text.Trim().Length > 0 && txtNumberLot.Text.Trim().Length > 0 && Common.ConvertInt(txtNumberLot.Text.Trim()) > 0)
            {
                //allocate lot
                int number_allocate_lot = 0;
                if (Common.ConvertInt(txtNumberPackage.Text) > 0)
                {
                    number_allocate_lot = Common.ConvertInt(txtNumberLot.Text) * Common.ConvertInt(txtNumberPackage.Text);
                }
                else
                {
                    number_allocate_lot = Common.ConvertInt(txtNumberLot.Text);
                }
                
                string url_lot = "sequences/2/reserve/" + Convert.ToString(number_allocate_lot);
                HttpResponse res_lot = HTTP.Instance.Post(url_lot, null);

                if (res_lot.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer_lot = new JavaScriptSerializer();
                        dynamic data_lot = serializer_lot.Deserialize(res_lot.RawText, typeof(object));

                        List_allocate_lot = Common.CreateSequential(Common.ConvertInt(data_lot["nextNumber"]), Common.ConvertInt(data_lot["step"]), Common.ConvertInt(data_lot["length"]), number_allocate_lot, data_lot["prefix"]);
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
            }

            if (status_reserve)
            {
                DataRow data = grvListPart.GetDataRow(grvListPart.GetSelectedRows().FirstOrDefault());
                long maxID = 0;
                using (erpEntities dbContext = new erpEntities())
                {
                    wh_transfer_details wtd = dbContext.wh_transfer_details.OrderByDescending(u => u.id).FirstOrDefault();
                    if (wtd != null) maxID = wtd.id + 1;
                }
                this.dt_trans_details = checkStructureDatatable(dt_trans_details);

                TransferDetailsRepository TransferDetails = new TransferDetailsRepository();
                list_transfer_details = new List<wh_transfer_details>();

                if (List_allocate_package.Count > 0 && List_allocate_lot.Count > 0)
                {
                    #region allocate package & lot

                    #region allocate package
                    //foreach (string pack in List_allocate_package)
                    //{
                    //    wh_transfer_details wtd = new wh_transfer_details();
                    //    wtd.src_package_number = txtSourceNumber.Text;
                    //    wtd.created = DateTime.Now;
                    //    wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                    //    wtd.dest_package_number = pack;
                    //    wtd.done_quantity = Common.ConvertDouble(txtNumberPerPackage.Text) - (Common.ConvertDouble(txtNumberPerLot.Text) * Common.ConvertInt(txtNumberLot.Text));
                    //    wtd.id = maxID;
                    //    wtd.man_id = Common.ConvertInt(data["manId"]);
                    //    //wtd.man_pn = Convert.ToString(data["manPn"]);
                    //    try { wtd.man_pn = Convert.ToString(data["manPn"]); }
                    //    catch (Exception ex) { wtd.man_pn = null; };
                    //    wtd.product_id = Common.ConvertInt(data["productId"]);
                    //    wtd.product_name = Convert.ToString(data["productName"]);
                    //    wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                    //    wtd.status = 0;
                    //    wtd.transfer_id = Common.ConvertInt(gluTransferNumber.EditValue);
                    //    wtd.transfer_item_id = Common.ConvertInt(data["id"]);
                    //    wtd.trace_number = null;
                    //    wtd.src_package_number = txtSourceNumber.Text;
                    //    list_transfer_details.Add(wtd);

                    //    //add to table list package
                    //    addPackageDataRow(wtd.src_package_number, wtd.dest_package_number, wtd.trace_number, Common.ConvertInt(wtd.src_location_id),
                    //        Common.ConvertInt(wtd.dest_location_id), Common.ConvertInt(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                    //        Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                    //    maxID++;
                    //}
                    #endregion

                    int count_lot_per_package = 0; int i = 0;
                    foreach (string lot in List_allocate_lot)
                    {
                        wh_transfer_details wtd = new wh_transfer_details();
                        wtd.created = DateTime.Now;
                        wtd.src_package_number = txtSourceNumber.Text;
                        wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                        wtd.dest_package_number = List_allocate_package[i];
                        wtd.done_quantity = Common.ConvertDouble(txtNumberPerLot.Text);
                        wtd.id = maxID;
                        wtd.man_id = Common.ConvertInt(data["manId"]);
                        //wtd.man_pn = Convert.ToString(data["manPn"]);
                        try { wtd.man_pn = Convert.ToString(data["manPn"]); }
                        catch (Exception ex) { wtd.man_pn = null; }
                        wtd.product_id = Common.ConvertInt(data["productId"]);
                        wtd.product_name = Convert.ToString(data["productName"]);
                        wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                        wtd.status = 0;
                        wtd.transfer_id = Common.ConvertInt(gluTransferNumber.EditValue);
                        wtd.transfer_item_id = Common.ConvertInt(data["id"]);
                        wtd.trace_number = lot;
                        wtd.internal_reference = Convert.ToString(data["internalReference"]);
                        wtd.reference = Convert.ToString(this.transfer_info["transferNumber"]);
                        list_transfer_details.Add(wtd);

                        //add to table list package
                        addPackageDataRow(wtd.internal_reference, wtd.reference, wtd.src_package_number, wtd.dest_package_number, wtd.trace_number, Common.ConvertInt(wtd.src_location_id),
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertInt(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print package
                        if (count_lot_per_package == 0)
                        {
                            LabelPackage labelPackage = new LabelPackage(wtd.product_name, List_allocate_package[i], "");
                            labelPackage.Template();
                        }

                        //Print
                        LabelPackage labelLot = new LabelPackage(wtd.product_name, lot, "");
                        labelLot.Template();

                        maxID++;
                        count_lot_per_package++;
                        if (count_lot_per_package >= Common.ConvertInt(txtNumberLot.Text)) { i++; count_lot_per_package = 0; }
                    }
                    #endregion
                }
                else if (List_allocate_package.Count > 0)
                {
                    #region allocate Package
                    foreach (string pack in List_allocate_package)
                    {
                        wh_transfer_details wtd = new wh_transfer_details();
                        wtd.src_package_number = txtSourceNumber.Text;
                        wtd.created = DateTime.Now;
                        wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                        wtd.dest_package_number = pack;
                        wtd.done_quantity = Common.ConvertDouble(txtNumberPerPackage.Text);
                        wtd.id = maxID;
                        wtd.man_id = Common.ConvertInt(data["manId"]);
                        //wtd.man_pn = Convert.ToString(data["manPn"]);
                        try { wtd.man_pn = Convert.ToString(data["manPn"]); }
                        catch (Exception ex) { wtd.man_pn = null; };
                        wtd.product_id = Common.ConvertInt(data["productId"]);
                        wtd.product_name = Convert.ToString(data["productName"]);
                        wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                        wtd.status = 0;
                        wtd.transfer_id = Common.ConvertInt(gluTransferNumber.EditValue);
                        wtd.transfer_item_id = Common.ConvertInt(data["id"]);
                        wtd.trace_number = "";
                        wtd.src_package_number = txtSourceNumber.Text;
                        wtd.internal_reference = Convert.ToString(data["internalReference"]);
                        wtd.reference = Convert.ToString(this.transfer_info["transferNumber"]);
                        list_transfer_details.Add(wtd);

                        //add to table list package
                        addPackageDataRow(wtd.internal_reference, wtd.reference, wtd.src_package_number, wtd.dest_package_number, wtd.trace_number, Common.ConvertInt(wtd.src_location_id),
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertInt(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print package
                        LabelPackage labelPackage = new LabelPackage(wtd.product_name, pack, "");
                        labelPackage.Template();

                        maxID++;
                    }
                    #endregion
                }
                else if (List_allocate_lot.Count > 0)
                {
                    #region allocate Lot
                    foreach (string lot in List_allocate_lot)
                    {
                        wh_transfer_details wtd = new wh_transfer_details();
                        wtd.created = DateTime.Now;
                        wtd.src_package_number = txtSourceNumber.Text;
                        wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                        wtd.dest_package_number = "";
                        wtd.done_quantity = Common.ConvertDouble(txtNumberPerLot.Text);
                        wtd.id = maxID;
                        wtd.man_id = Common.ConvertInt(data["manId"]);
                        //wtd.man_pn = Convert.ToString(data["manPn"]);
                        try { wtd.man_pn = Convert.ToString(data["manPn"]); }
                        catch (Exception ex) { wtd.man_pn = null; }
                        wtd.product_id = Common.ConvertInt(data["productId"]);
                        wtd.product_name = Convert.ToString(data["productName"]);
                        wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                        wtd.status = 0;
                        wtd.transfer_id = Common.ConvertInt(gluTransferNumber.EditValue);
                        wtd.transfer_item_id = Common.ConvertInt(data["id"]);
                        wtd.trace_number = lot;
                        wtd.internal_reference = Convert.ToString(data["internalReference"]);
                        wtd.reference = Convert.ToString(this.transfer_info["transferNumber"]);
                        list_transfer_details.Add(wtd);

                        //add to table list package
                        addPackageDataRow(wtd.internal_reference, wtd.reference, wtd.src_package_number, wtd.dest_package_number, wtd.trace_number, Common.ConvertInt(wtd.src_location_id),
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertInt(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print Lot
                        LabelPackage labelLot = new LabelPackage(wtd.product_name, lot, "");
                        labelLot.Template();

                        maxID++;
                    }
                    #endregion
                }

                vgListPackage.DataSource = dt_trans_details;

                TransferDetails.Add(list_transfer_details.ToArray());
            }
            else
            {
                MessageBox.Show("There is an error in the allocation process");
            }
        }

        private void addPackageDataRow(string _internalReference, string _reference, string _srcPackageNumber, string _destPackageNumber, string _traceNumber, int _srcLocationId, int _destLocationId,
            double _doneQuantity, int _transferId, int _transferItemId, int _productId, int _manId, string _manPn = null, int? _lotId = null, int _printed = 1)
        {
            DataRow myR = this.dt_trans_details.NewRow();
            try
            {
                myR["id"] = -1;//this.dt_trans_details.Rows.Count + 1;
                myR["srcPackageNumber"] = _srcPackageNumber;
                myR["destPackageNumber"] = _destPackageNumber;
                myR["traceNumber"] = _traceNumber;
                myR["srcLocationId"] = _srcLocationId;
                myR["destLocationId"] = _destLocationId;
                myR["doneQuantity"] = _doneQuantity;
                myR["transferId"] = _transferId;
                myR["transferItemId"] = _transferItemId;
                myR["productId"] = _productId;
                myR["manId"] = _manId;
                myR["manPn"] = _manPn;
                myR["internalReference"] = _internalReference;
                myR["reference"] = _reference;
                //myR["lotId"] = _lotId;
                myR["countPrint"] = _printed;

                this.dt_trans_details.Rows.Add(myR);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private DataTable checkStructureDatatable(DataTable dt)
        {
            DataColumnCollection columns = dt.Columns;
            if (!columns.Contains("srcPackageNumber")) dt.Columns.Add("srcPackageNumber");
            if (!columns.Contains("destPackageNumber")) dt.Columns.Add("destPackageNumber");
            if (!columns.Contains("traceNumber")) dt.Columns.Add("traceNumber");
            if (!columns.Contains("srcLocationId")) dt.Columns.Add("srcLocationId");
            if (!columns.Contains("destLocationId")) dt.Columns.Add("destLocationId");
            if (!columns.Contains("doneQuantity")) dt.Columns.Add("doneQuantity");
            if (!columns.Contains("transferId")) dt.Columns.Add("transferId");
            if (!columns.Contains("transferItemId")) dt.Columns.Add("transferItemId");
            if (!columns.Contains("productId")) dt.Columns.Add("productId");
            if (!columns.Contains("manId")) dt.Columns.Add("manId");
            if (!columns.Contains("manPn")) dt.Columns.Add("manPn");
            if (!columns.Contains("lotId")) dt.Columns.Add("lotId");
            if (!columns.Contains("countPrint")) dt.Columns.Add("countPrint");
            if (!columns.Contains("internalReference")) dt.Columns.Add("internalReference");
            if (!columns.Contains("reference")) dt.Columns.Add("reference");
            return dt;
        }

        private DataTable buildDataTable()
        {
            DataTable myDtTable = new DataTable();
            myDtTable.Columns.Add("id", typeof(int));
            myDtTable.Columns.Add("srcPackageNumber", typeof(string));
            myDtTable.Columns.Add("destPackageNumber", typeof(string));
            myDtTable.Columns.Add("traceNumber", typeof(double));
            myDtTable.Columns.Add("srcLocationId", typeof(int));
            myDtTable.Columns.Add("destLocationId", typeof(int));
            myDtTable.Columns.Add("doneQuantity", typeof(double));
            myDtTable.Columns.Add("transferId", typeof(int));
            myDtTable.Columns.Add("transferItemId", typeof(int));
            myDtTable.Columns.Add("productId", typeof(int));
            myDtTable.Columns.Add("manId", typeof(int));
            myDtTable.Columns.Add("manPn", typeof(string));
            myDtTable.Columns.Add("lotId", typeof(int));
            myDtTable.Columns.Add("countPrint", typeof(int));
            myDtTable.Columns.Add("internalReference", typeof(string));
            myDtTable.Columns.Add("reference", typeof(string));

            return myDtTable;
        }

        private void txtNumberPerPackage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (txtNumberPerLot.Text != null && txtNumberPerLot.Text.Length > 0)
                    {
                        if (Common.ConvertDouble(txtNumberPerPackage.Text) % Common.ConvertDouble(txtNumberPerLot.Text) == 0)
                            txtNumberLot.Text = Convert.ToString(Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text));
                        else
                            txtNumberLot.Text = Convert.ToString(Math.Floor((Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text))) + 1);
                    }
                    else if (txtNumberLot.Text != null && txtNumberLot.Text.Length > 0)
                    {
                        txtNumberPerLot.Text = Convert.ToString(Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberLot.Text));
                    }
                }
                catch { }
            }
        }

        private void txtNumberPerLot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (Common.ConvertDouble(txtNumberPerPackage.Text) % Common.ConvertDouble(txtNumberPerLot.Text) == 0)
                        txtNumberLot.Text = Convert.ToString(Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text));
                    else
                        txtNumberLot.Text = Convert.ToString(Math.Floor((Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberPerLot.Text))) + 1);
                }
                catch { }
            }
        }

        private void txtNumberLot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (Common.ConvertDouble(txtNumberPerPackage.Text) % Common.ConvertDouble(txtNumberLot.Text) == 0)
                        txtNumberPerLot.Text = Convert.ToString(Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberLot.Text));
                    else
                        txtNumberPerLot.Text = Convert.ToString(Math.Floor(Common.ConvertDouble(txtNumberPerPackage.Text) / Common.ConvertDouble(txtNumberLot.Text)));
                }
                catch { }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Bạn có chắc chắn muốn in lại tem?.", "In lại tem", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                var data = grvListPart.GetDataRow(grvListPart.GetSelectedRows().FirstOrDefault());
                string _product_name = Convert.ToString(data["internalReference"]);
                foreach (var index in grvListPackage.GetSelectedRows())
                {
                    dynamic row = this.grvListPackage.GetRow(index);
                    string _id_package = Convert.ToString(row["destPackageNumber"]);
                    string _id_lot = Convert.ToString(row["traceNumber"]);
                    string _id = _id_lot.Length > 0 ? _id_lot : _id_package;
                    LabelPackage labelPackage = new LabelPackage(_product_name, _id, "");
                    labelPackage.Template();
                }
            }
        }

        private void cbxViewPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadDataListPackage();
            
            if (cbxViewPackage.EditValue.ToString() == "Package")
            {
                vgListPackage.DataSource = this.dt_trans_details;
                for (int i = 0; i < grvListPackage.DataRowCount; i++)
                {
                    DataRow row = grvListPackage.GetDataRow(i);
                    if (row["traceNumber"] != null && Convert.ToString(row["traceNumber"]).Length > 0) { grvListPackage.DeleteRow(i); i--; }
                }
            }
            else if (cbxViewPackage.EditValue.ToString() == "Lot")
            {
                vgListPackage.DataSource = this.dt_trans_details;
                for (int i = 0; i < grvListPackage.DataRowCount; i++)
                {
                    DataRow row = grvListPackage.GetDataRow(i);
                    if (row["traceNumber"] == null || Convert.ToString(row["traceNumber"]).Length == 0) { grvListPackage.DeleteRow(i); i--; }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var data = grvListPart.GetDataRow(grvListPart.GetSelectedRows().FirstOrDefault());

            TransferReceiptsDetails frmTransferReceiptsDetails = new TransferReceiptsDetails();
            dynamic row = grvListPackage.GetRow(grvListPackage.FocusedRowHandle);
            frmTransferReceiptsDetails.ID = Common.ConvertInt(row["id"]);
            frmTransferReceiptsDetails.transfer_info = this.transfer_info;
            frmTransferReceiptsDetails.PackageID = Convert.ToString(row["destPackageNumber"]);
            frmTransferReceiptsDetails.ProductName = Convert.ToString(data["productName"]);
            frmTransferReceiptsDetails.ManId = Common.ConvertInt(data["manId"]);
            try { frmTransferReceiptsDetails.ManPn = Convert.ToString(data["manPn"]); }
            catch { frmTransferReceiptsDetails.ManPn = null; }
            frmTransferReceiptsDetails.ProductId = Common.ConvertInt(data["productId"]);
            frmTransferReceiptsDetails.TransferId = Common.ConvertInt(row["transferId"]);
            frmTransferReceiptsDetails.TransferItemId = Common.ConvertInt(row["transferItemId"]);
            frmTransferReceiptsDetails.SourceLocationId = Common.ConvertInt(row["srcLocationId"]);
            frmTransferReceiptsDetails.DestLocationId = Common.ConvertInt(row["destLocationId"]);
            frmTransferReceiptsDetails.DoneQuantityPackage = Common.ConvertInt(row["doneQuantity"]);
            frmTransferReceiptsDetails.InternalReference = Common.ConvertInt(row["internalReference"]);
            frmTransferReceiptsDetails.Reference = Convert.ToString(this.transfer_info["transferNumber"]);

            frmTransferReceiptsDetails.ShowDialog();

            //data back
            if (frmTransferReceiptsDetails.CheckAllocate)
            {
                grvListPackage.DeleteRow(grvListPackage.FocusedRowHandle);
            }
            
            //merge lots allocate
            this.dt_trans_details = checkStructureDatatable(dt_trans_details);
            foreach (DataRow row_lot in frmTransferReceiptsDetails.dt_lots.Rows)
            {
                if (Common.ConvertInt(row_lot["id"]) == -1)
                {
                    addPackageDataRow(Convert.ToString(row_lot["internalReference"]), Convert.ToString(this.transfer_info["transferNumber"]), Convert.ToString(row_lot["srcPackageNumber"]), Convert.ToString(row_lot["destPackageNumber"]), Convert.ToString(row_lot["traceNumber"]),
                    Common.ConvertInt(row_lot["srcLocationId"]), Common.ConvertInt(row_lot["destLocationId"]), Common.ConvertDouble(row_lot["doneQuantity"]),
                    Common.ConvertInt(row_lot["transferId"]), Common.ConvertInt(row_lot["transferItemId"]), Common.ConvertInt(row_lot["productId"]), Common.ConvertInt(row_lot["manId"]));
                }
            }

            vgListPackage.DataSource = dt_trans_details;
        }

        private void btnDeletePack_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Bạn có chắc chắn muốn xóa tem?.", "Xóa tem", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                Dictionary<string, dynamic> transfer_info_delete = transfer_info;
                //transfer_info
                transfer_info_delete.Remove("transferItems");
                transfer_info_delete.Remove("removedTransferItems");
                transfer_info_delete.Remove("transferDetails");
                transfer_info_delete.Remove("active");
                DataRow row = grvListPackage.GetDataRow(grvListPackage.FocusedRowHandle);
                string _id_delete = "[" + Convert.ToString(row["id"]) + "]";
                transfer_info_delete["removedTransferDetails"] = _id_delete;
                string param_put = Common.DictionaryObjectToJson(transfer_info_delete);

                //PUT DATA
                string url = "transfers/" + Convert.ToString(transfer_info_delete["id"]);

                HttpResponse res = HTTP.Instance.Put(url, param_put);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    grvListPackage.DeleteRow(grvListPackage.FocusedRowHandle);
                }
                else
                {
                    MessageBox.Show("Error delete !!");
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Bạn có chắc chắn muốn xóa tem?.", "Xóa tem", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                Dictionary<string, dynamic> transfer_info_delete = transfer_info;
                //transfer_info
                transfer_info_delete.Remove("transferItems");
                transfer_info_delete.Remove("removedTransferItems");
                transfer_info_delete.Remove("transferDetails");
                transfer_info_delete.Remove("active");
                string _id_delete = "[";
                foreach (var index in grvListPackage.GetSelectedRows())
                {
                    dynamic row = this.grvListPackage.GetRow(index);
                    if (_id_delete.Length >= 2) _id_delete += "," + Convert.ToString(row["id"]);
                    else _id_delete += Convert.ToString(row["id"]);
                }
                _id_delete += "]";

                transfer_info_delete["removedTransferDetails"] = _id_delete;
                string param_put = Common.DictionaryObjectToJson(transfer_info_delete);

                //PUT DATA
                string url = "transfers/" + Convert.ToString(transfer_info_delete["id"]);

                HttpResponse res = HTTP.Instance.Put(url, param_put);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    grvListPackage.DeleteSelectedRows();
                }
                else
                {
                    MessageBox.Show("Error delete !!");
                }
            }
        }

        private void grvListPackage_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.Caption == "edit")
            {
                //var _id = grvListPackage.GetRowCellValue(e.RowHandle, "id");
                var _traceNumber = grvListPackage.GetRowCellValue(e.RowHandle, "traceNumber");
                //if (Common.ConvertInt(_id) == -1 || Convert.ToString(_traceNumber).Length > 0)
                if (Convert.ToString(_traceNumber).Length > 0)
                {
                    RepositoryItemButtonEdit ritem = new RepositoryItemButtonEdit();
                    ritem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                    ritem.ReadOnly = true;
                    ritem.Buttons[0].Visible = false;
                    e.RepositoryItem = ritem;
                }
                else
                {
                    btnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                    btnEdit.ReadOnly = true;
                    btnEdit.Buttons[0].Visible = true;
                    e.RepositoryItem = btnEdit;
                }
            }
            if (e.Column.Caption == "delete")
            {
                var _id = grvListPackage.GetRowCellValue(e.RowHandle, "id");
                if (Common.ConvertInt(_id) == -1)
                {
                    RepositoryItemButtonEdit ritem = new RepositoryItemButtonEdit();
                    ritem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                    ritem.ReadOnly = true;
                    ritem.Buttons[0].Visible = false;
                    e.RepositoryItem = ritem;
                }
                else
                {
                    btnDeletePack.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                    btnDeletePack.ReadOnly = true;
                    btnDeletePack.Buttons[0].Visible = true;
                    e.RepositoryItem = btnDeletePack;
                }
            }
        }

        private void TransferReceipts_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Append(e.KeyChar);
            //timeScan.Enabled = true;
            //timeScan.Interval = 2500;

            if (e.KeyChar == (char)13)
            {
                //Push Queue
                QueueScan.Enqueue(code_read.ToString().Trim());
                code_read.Clear();
            }
        }

        private void PopQueueScan()
        {
            if (QueueScan.Count > 0)
            {

                string ScanText = QueueScan.Dequeue();

                //Pop Queue -> process data
                double n;
                bool isNumeric = double.TryParse(ScanText, out n);

                //var a = this.FocusedControl;

                if (!isNumeric)
                {
                    //process input transfer
                    try
                    {
                        string[] split = ScanText.Split('-');

                        //var element = gluTransferNumber.Properties.GetRowByKeyValue(split[1]);
                        IsTransferScan = true;
                        this.gluTransferNumber.Invoke(new MethodInvoker(delegate { try { gluTransferNumber.EditValue = split[1]; } catch { } }));

                        loadDataTransferScan();
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private void TransferReceipts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ThreadScan != null && ThreadScan.IsAlive) ThreadScan.Abort();
        }

        #region search
        private void gluTransferNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (Char)Keys.Back && e.KeyChar != (Char)Keys.Delete)
            {
                key_search_transfer.Append(e.KeyChar);
                //MessageBox.Show(key_search_transfer.ToString());

                string url = "transfers/search?query=transferNumber==\"*" + key_search_transfer.ToString() + "*\"&size=15";
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list_request_paper = new List<ExpandoObject>(res.DynamicBody);
                        dt_request_paper = Common.ToDataTable(list_request_paper);

                        gluTransferNumber.Properties.DataSource = dt_request_paper;
                        gluTransferNumber.Properties.DisplayMember = "transferNumber";
                        gluTransferNumber.Properties.ValueMember = "id";
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Không load được dữ liệu !");
                    };
                }
                else
                {
                    //messageShow.ShowHint("không có dữ liệu");
                }
            }
            
            
        }
        #endregion

        private void gluTransferNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (gluTransferNumber.SelectionLength == gluTransferNumber.Text.Length && (e.KeyData == Keys.Back || e.KeyData == Keys.Delete))
            {
                gluTransferNumber.EditValue = null;
                e.Handled = true;
                key_search_transfer = new StringBuilder();
            }
        }

        private void gluTransferNumber_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {

        }

    }
}