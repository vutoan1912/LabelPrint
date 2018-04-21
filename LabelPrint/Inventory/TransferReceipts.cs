using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
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
        //private Control FocusedControl;
        private bool IsTransferScan = false;
        private string _state;

        private string _project = "";
        private string _supplier = "";
        
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

            //set timer gridlookup
            timer_transfer.Elapsed += new ElapsedEventHandler(timer_transfer_Tick);
            timer_transfer.Interval = 500;
            timer_srcLocation.Elapsed += new ElapsedEventHandler(timer_srcLocation_Tick);
            timer_srcLocation.Interval = 400;
            timer_destLocation.Elapsed += new ElapsedEventHandler(timer_destLocation_Tick);
            timer_destLocation.Interval = 400;
            timer_UomPackage.Elapsed += new ElapsedEventHandler(timer_UomPackage_Tick);
            timer_UomPackage.Interval = 400;
            timer_UomLot.Elapsed += new ElapsedEventHandler(timer_UomLot_Tick);
            timer_UomLot.Interval = 400;

            //Form scan 
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;
        }

        #region Language
        private void SetLanguage(string cultureName)
        {
            culture = CultureInfo.CreateSpecificCulture(cultureName);
            ResourceManager rm = new ResourceManager("LabelPrint.Lang.Language", typeof(TransferReceipts).Assembly);

            lblRequest.Text = rm.GetString("request", culture);
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

        // ---------------------------------------------------------- TAB TRANSFER ------------------------------------------------------------------ //

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
            string url = "transfers/search?query=&size=10";
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
            string url_location = "locations/search?query=&size=10";
            var param_location = new { };
            HttpResponse res_location = HTTP.Instance.Get(url_location, param_location);

            if (res_location.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<ExpandoObject> list_location = new List<ExpandoObject>(res_location.DynamicBody);
                    DataTable dt_location = Common.ToDataTable(list_location);

                    this.gluSourceLocation.Properties.DataSource = dt_location;
                    this.gluSourceLocation.Properties.DisplayMember = "completeName";
                    this.gluSourceLocation.Properties.ValueMember = "id";

                    this.gluDestinationLocation.Properties.DataSource = dt_location;
                    this.gluDestinationLocation.Properties.DisplayMember = "completeName";
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
            string url_uom = "uoms/search?query=&size=10";
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

        private void cbxRequestPaper_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if(gluTransferNumber.EditValue != null) loadDataTransfer();
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
                        this._state = Convert.ToString(data["state"]);
                        this._state = this._state.ToLower();

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
                                        #region Get source location
                                        try
                                        {
                                            string url_srcLocation = "locations/search?query=id==" + Convert.ToString(data["srcLocationId"]);
                                            var param_srcLocation = new { };
                                            HttpResponse res_srcLocation = HTTP.Instance.Get(url_srcLocation, param_srcLocation);

                                            if (res_srcLocation.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                try
                                                {
                                                    List<ExpandoObject> list_srcLocation = new List<ExpandoObject>(res_srcLocation.DynamicBody);
                                                    DataTable dt_srcLocation = Common.ToDataTable(list_srcLocation);

                                                    if (dt_srcLocation.Rows.Count > 0)
                                                    {
                                                        lblSourceLocationValue.Text = Convert.ToString(dt_srcLocation.Rows[0]["completeName"]);

                                                        this.gluSourceLocation.Properties.DataSource = dt_srcLocation;
                                                        this.gluSourceLocation.Properties.DisplayMember = "completeName";
                                                        this.gluSourceLocation.Properties.ValueMember = "id";
                                                        gluSourceLocation.EditValue = dt_srcLocation.Rows[0]["id"];
                                                    }
                                                    
                                                }
                                                catch (Exception ex)
                                                {
                                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                                };
                                            }
                                        }
                                        catch { lblSourceLocationValue.Text = ""; };
                                        #endregion

                                        #region Get destination location
                                        try
                                        {
                                            string url_destLocation = "locations/search?query=id==" + Convert.ToString(data["destLocationId"]);
                                            var param_destLocation = new { };
                                            HttpResponse res_destLocation = HTTP.Instance.Get(url_destLocation, param_destLocation);

                                            if (res_destLocation.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                try
                                                {
                                                    List<ExpandoObject> list_destLocation = new List<ExpandoObject>(res_destLocation.DynamicBody);
                                                    DataTable dt_destLocation = Common.ToDataTable(list_destLocation);

                                                    if (dt_destLocation.Rows.Count > 0)
                                                    {
                                                        this.gluDestinationLocation.Properties.DataSource = dt_destLocation;
                                                        this.gluDestinationLocation.Properties.DisplayMember = "completeName";
                                                        this.gluDestinationLocation.Properties.ValueMember = "id";
                                                        gluDestinationLocation.EditValue = dt_destLocation.Rows[0]["id"];
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
                                    this._supplier = Convert.ToString(data_partner["name"]);
                                    lblPartnerValue.Text = data_partner["name"];
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                };
                            }

                        }
                        catch { lblPartnerValue.Text = ""; this._supplier = ""; };
                        #endregion

                        #region Get project information
                        try
                        {
                            string url_project = "product-version/" + Convert.ToString(data["productVersionId"]);
                            var param_project = new { };
                            HttpResponse res_project = HTTP.Instance.Get(url_project, param_project);

                            if (res_project.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                try
                                {
                                    var serializer_project = new JavaScriptSerializer();
                                    dynamic data_project = serializer_project.Deserialize(res_project.RawText, typeof(object));
                                    this._project = data_project["name"];
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Không tồn tại dữ liệu !");
                                };
                            }

                        }
                        catch { this._project = ""; };
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
                                    List<TransferItem> RootObject = JsonConvert.DeserializeObject<List<TransferItem>>(res_list_part.RawText, new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    });

                                    List<TransferItem> list_parts = RootObject as List<TransferItem>;
                                    dt_products = Common.ToDataTableClass<TransferItem>(list_parts);

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
                        //MessageBox.Show("Không tải được dữ liệu transfer !");
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
            this.vgListPackage.DataSource = null;
            this.dt_trans_details.Clear();

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
                            List<TransferDetail> RootObject = JsonConvert.DeserializeObject<List<TransferDetail>>(res_trans_details.RawText, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                            List<TransferDetail> list_trans_details = RootObject as List<TransferDetail>;
                            dt_trans_details = Common.ToDataTableClass<TransferDetail>(list_trans_details);
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
            if (this._state == "done")
            {
                MessageBox.Show("The selected transaction has completed, actions are not allowed !");
                return;
            }

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
                string url_package = "sequences/" + Config.PackReserveId + "/reserve/" + txtNumberPackage.Text;
                HttpResponse res_package = HTTP.Instance.Post(url_package, null);

                if (res_package.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer_package = new JavaScriptSerializer();
                        dynamic data_package = serializer_package.Deserialize(res_package.RawText, typeof(object));

                        List_allocate_package = Common.CreateSequential(Convert.ToInt32(data_package["nextNumber"]), Convert.ToInt32(data_package["step"]), Convert.ToInt32(data_package["length"]), Convert.ToInt32(txtNumberPackage.Text), data_package["prefix"]);
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
                    number_allocate_lot = Convert.ToInt32(txtNumberLot.Text) * Convert.ToInt32(txtNumberPackage.Text);
                }
                else
                {
                    number_allocate_lot = Convert.ToInt32(txtNumberLot.Text);
                }

                string url_lot = "sequences/" + Config.LotReserveId + "/reserve/" + Convert.ToString(number_allocate_lot);
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
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertDouble(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print package
                        if (count_lot_per_package == 0)
                        {
                            LabelPackage labelPackage = new LabelPackage(wtd.internal_reference, List_allocate_package[i], "", this.transfer_info["transferNumber"], this._supplier, this._project);
                            labelPackage.Template();
                        }

                        //Print
                        LabelPackage labelLot = new LabelPackage(wtd.internal_reference, lot, "", this.transfer_info["transferNumber"],this._supplier,this._project);
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
                        wtd.trace_number = null;
                        wtd.src_package_number = txtSourceNumber.Text;
                        wtd.internal_reference = Convert.ToString(data["internalReference"]);
                        wtd.reference = Convert.ToString(this.transfer_info["transferNumber"]);
                        list_transfer_details.Add(wtd);

                        //add to table list package
                        addPackageDataRow(wtd.internal_reference, wtd.reference, wtd.src_package_number, wtd.dest_package_number, wtd.trace_number, Common.ConvertInt(wtd.src_location_id),
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertDouble(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print package
                        LabelPackage labelPackage = new LabelPackage(wtd.internal_reference, pack,"", this.transfer_info["transferNumber"],this._supplier,this._project);
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
                        wtd.dest_package_number = null;
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
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertDouble(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print Lot
                        LabelPackage labelLot = new LabelPackage(wtd.internal_reference, lot, "", this.transfer_info["transferNumber"],this._supplier,this._project);
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

        private void addPackageDataRow(string _internalReference, string _reference, string _srcPackageNumber, string _destPackageNumber, string _traceNumber, int? _srcLocationId, int? _destLocationId,
            double _doneQuantity, int? _transferId, int? _transferItemId, int? _productId, int? _manId, string _manPn = null, int? _lotId = null, int _printed = 1)
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
                
            }
        }

        private DataTable checkStructureDatatable(DataTable dt)
        {
            DataColumnCollection columns = dt.Columns;
            if (!columns.Contains("id")) dt.Columns.Add("id");
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
                string _internal_reference = Convert.ToString(data["internalReference"]);
                foreach (var index in grvListPackage.GetSelectedRows())
                {
                    dynamic row = this.grvListPackage.GetRow(index);
                    string _id_package = Convert.ToString(row["destPackageNumber"]);
                    string _id_lot = Convert.ToString(row["traceNumber"]);
                    string _id = _id_lot.Length > 0 ? _id_lot : _id_package;
                    LabelPackage labelPackage = new LabelPackage(_internal_reference, _id, "", this.transfer_info["transferNumber"], this._supplier, this._project);
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
            //frmTransferReceiptsDetails.ProductName = Convert.ToString(data["productName"]);
            frmTransferReceiptsDetails.ManId = Common.ConvertInt(data["manId"]);
            try { frmTransferReceiptsDetails.ManPn = Convert.ToString(data["manPn"]); }
            catch { frmTransferReceiptsDetails.ManPn = null; }
            frmTransferReceiptsDetails.ProductId = Common.ConvertInt(data["productId"]);
            frmTransferReceiptsDetails.TransferId = Common.ConvertInt(row["transferId"]);
            frmTransferReceiptsDetails.TransferItemId = Common.ConvertInt(row["transferItemId"]);
            frmTransferReceiptsDetails.SourceLocationId = Common.ConvertInt(row["srcLocationId"]);
            frmTransferReceiptsDetails.DestLocationId = Common.ConvertInt(row["destLocationId"]);
            frmTransferReceiptsDetails.DoneQuantityPackage = Common.ConvertInt(row["doneQuantity"]);
            frmTransferReceiptsDetails.InternalReference = Convert.ToString(row["internalReference"]);
            frmTransferReceiptsDetails.Reference = Convert.ToString(this.transfer_info["transferNumber"]);
            frmTransferReceiptsDetails.State = this._state;

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
            if (this._state == "done")
            {
                MessageBox.Show("The selected transaction has completed, not allowed to delete !");
                return;
            }

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
                try { transfer_info_delete.Remove("manOrderTransfer"); }catch { };
                try { transfer_info_delete.Remove("returnedTransfer"); }catch { };
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
            if (this._state == "done")
            {
                MessageBox.Show("The selected transaction has completed, not allowed to delete !");
                return;
            }

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
                try { transfer_info_delete.Remove("manOrderTransfer"); }catch { };
                try { transfer_info_delete.Remove("returnedTransfer"); }catch { };
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            loadDataListPackage();
        }

        #region TextBox KeyPress

        private void txtSourceNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Clear();
        }

        private void txtNumberPerPackage_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Clear();
        }

        private void txtNumberPackage_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Clear();
        }

        private void txtNumberPerLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Clear();
        }

        private void txtNumberLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Clear();
        }

        #endregion

        #region GridLookUpEdit

        #region TransferNumber
        
        System.Timers.Timer timer_transfer = new System.Timers.Timer();
        void timer_transfer_Tick(object sender, EventArgs e)
        {
            timer_transfer.Stop();

            if (this.gluTransferNumber.Text.Length > 0)
            {
                string key_search = this.gluTransferNumber.Text;
                string url = "transfers/search?query=transferNumber==\"*" + key_search + "*\"&size=15";

                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list_request_paper = new List<ExpandoObject>(res.DynamicBody);
                        dt_request_paper = Common.ToDataTable(list_request_paper);
                        this.gluTransferNumber.Invoke(new MethodInvoker(delegate { 
                            gluTransferNumber.Properties.DataSource = dt_request_paper;

                            if (dt_request_paper.Rows.Count == 1)
                            {
                                gluTransferNumber.EditValue = dt_request_paper.Rows[0]["id"];
                            }
                            else
                            {
                                gluTransferNumber.ShowPopup();
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                    };
                }
            }
        }

        private void gluTransferNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar != (Char)Keys.Back && e.KeyChar != (Char)Keys.Delete)
            //{
            //    key_search_transfer.Append(e.KeyChar);
            //    clear_key_transfer_number = false;
            //}
            //else
            //{
            //    clear_key_transfer_number = true;
            //}
        }

        private void gluTransferNumber_TextChanged(object sender, EventArgs e)
        {
            code_read.Clear();
            timer_transfer.Start();
        }

        #endregion

        #region SourceLocation
        System.Timers.Timer timer_srcLocation = new System.Timers.Timer();
        void timer_srcLocation_Tick(object sender, EventArgs e)
        {
            timer_srcLocation.Stop();

            if (this.gluSourceLocation.Text.Length > 0)
            {
                string url = "locations/search?query=completeName==\"*" + this.gluSourceLocation.Text + "*\"&size=15";

                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list = new List<ExpandoObject>(res.DynamicBody);
                        DataTable dt = Common.ToDataTable(list);

                        this.gluSourceLocation.Invoke(new MethodInvoker(delegate
                        {
                            this.gluSourceLocation.Properties.DataSource = dt;
                            this.gluSourceLocation.Properties.DisplayMember = "completeName";
                            this.gluSourceLocation.Properties.ValueMember = "id";

                            if (dt.Rows.Count == 1)
                            {
                                gluSourceLocation.EditValue = dt.Rows[0]["id"];
                            }
                            else
                            {
                                gluSourceLocation.ShowPopup();
                            }
                        }));

                        //MessageBox.Show(this.gluSourceLocation.Text);
                    }
                    catch (Exception ex)
                    {
                    };
                }
            }
        }

        private void gluSourceLocation_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void gluSourceLocation_TextChanged(object sender, EventArgs e)
        {
            code_read.Clear();
            timer_srcLocation.Start();
        }
        #endregion

        #region DestinationLocation
        System.Timers.Timer timer_destLocation = new System.Timers.Timer();
        void timer_destLocation_Tick(object sender, EventArgs e)
        {
            timer_destLocation.Stop();

            if (this.gluDestinationLocation.Text.Length > 0)
            {
                string url = "locations/search?query=completeName==\"*" + this.gluDestinationLocation.Text + "*\"&size=15";

                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list = new List<ExpandoObject>(res.DynamicBody);
                        DataTable dt = Common.ToDataTable(list);

                        this.gluDestinationLocation.Invoke(new MethodInvoker(delegate
                        {
                            this.gluDestinationLocation.Properties.DataSource = dt;
                            this.gluDestinationLocation.Properties.DisplayMember = "completeName";
                            this.gluDestinationLocation.Properties.ValueMember = "id";

                            if (dt.Rows.Count == 1)
                            {
                                gluDestinationLocation.EditValue = dt.Rows[0]["id"];
                            }
                            else
                            {
                                gluDestinationLocation.ShowPopup();
                            }
                        }));

                        //MessageBox.Show(this.gluDestinationLocation.Text);
                    }
                    catch (Exception ex)
                    {
                    };
                }
            }
        }
        private void gluDestinationLocation_TextChanged(object sender, EventArgs e)
        {
            code_read.Clear();
            timer_destLocation.Start();
        }
        #endregion

        #region UomPackage
        System.Timers.Timer timer_UomPackage = new System.Timers.Timer();
        void timer_UomPackage_Tick(object sender, EventArgs e)
        {
            timer_UomPackage.Stop();

            if (this.gluUomPackage.Text.Length > 0)
            {
                string url = "uoms/search?query=name==\"*" + this.gluUomPackage.Text + "*\"&size=10";

                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list = new List<ExpandoObject>(res.DynamicBody);
                        DataTable dt = Common.ToDataTable(list);

                        this.gluUomPackage.Invoke(new MethodInvoker(delegate
                        {
                            this.gluUomPackage.Properties.DataSource = dt;
                            this.gluUomPackage.Properties.DisplayMember = "name";
                            this.gluUomPackage.Properties.ValueMember = "id";

                            if (dt.Rows.Count == 1)
                            {
                                gluUomPackage.EditValue = dt.Rows[0]["id"];
                            }
                            else
                            {
                                gluUomPackage.ShowPopup();
                            }
                        }));

                        //MessageBox.Show(this.gluUomPackage.Text);
                    }
                    catch (Exception ex)
                    {
                    };
                }
            }
        }

        private void gluUomPackage_TextChanged(object sender, EventArgs e)
        {
            code_read.Clear();
            timer_UomPackage.Start();
        }
        #endregion

        #region UomLot
        System.Timers.Timer timer_UomLot = new System.Timers.Timer();
        void timer_UomLot_Tick(object sender, EventArgs e)
        {
            timer_UomLot.Stop();

            if (this.gluUomLot.Text.Length > 0)
            {
                string url = "uoms/search?query=name==\"*" + this.gluUomLot.Text + "*\"&size=10";

                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list = new List<ExpandoObject>(res.DynamicBody);
                        DataTable dt = Common.ToDataTable(list);

                        this.gluUomLot.Invoke(new MethodInvoker(delegate
                        {
                            this.gluUomLot.Properties.DataSource = dt;
                            this.gluUomLot.Properties.DisplayMember = "name";
                            this.gluUomLot.Properties.ValueMember = "id";

                            if (dt.Rows.Count == 1)
                            {
                                gluUomLot.EditValue = dt.Rows[0]["id"];
                            }
                            else
                            {
                                gluUomLot.ShowPopup();
                            }
                        }));

                        //MessageBox.Show(this.gluUomLot.Text);
                    }
                    catch (Exception ex)
                    {
                    };
                }
            }
        }
        private void gluUomLot_TextChanged(object sender, EventArgs e)
        {
            code_read.Clear();
            timer_UomLot.Start();
        }
        #endregion

        #endregion

        // ---------------------------------------------------------- TAB ID ------------------------------------------------------------------ //

        private int packageID;

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (ThreadScan == null || !ThreadScan.IsAlive)
            {
                ThreadScan = new Thread(new ThreadStart(PopQueueScan));
                ThreadScan.Start();
                //Console.WriteLine("Pop Queue Scan!!!");
            }
        }

        private void TransferReceipts_KeyPress(object sender, KeyPressEventArgs e)
        {
            code_read.Append(e.KeyChar);
            if (e.KeyChar == (char)13)
            {
                //Push Queue
                QueueScan.Enqueue(code_read.ToString().Trim());
                code_read.Clear();
            }
        }

        private void TransferReceipts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ThreadScan != null && ThreadScan.IsAlive) ThreadScan.Abort();
        }

        private void PopQueueScan()
        {
            if (QueueScan.Count > 0)
            {
                string ScanString = QueueScan.Dequeue();
                //Console.WriteLine("Pop Queue : " + ScanString);

                if (ScanString.Length > 3)
                {
                    if (ScanString.StartsWith("["))
                    {
                        LabelPackage label = new LabelPackage(ScanString);
                        this.txtSearchID.Invoke(new MethodInvoker(delegate { txtSearchID.Text = label.PackageID; }));
                        LoadDataSearch();
                        return;
                    }

                    TemThungCuon temthung = new TemThungCuon(ScanString);
                    if (temthung.IdThung != null && temthung.IdThung.Length > 0 && temthung.Type != null && temthung.Type == "1")
                    {
                        this.txtSearchID.Invoke(new MethodInvoker(delegate { txtSearchID.Text = temthung.IdThung; }));
                        LoadDataSearch();
                        return;
                    }

                    TemCuon temcuon = new TemCuon(ScanString);
                    if (temcuon.IdCuon != null && temcuon.IdCuon.Length > 0 && temcuon.Type != null && temcuon.Type == "0")
                    {
                        this.txtSearchID.Invoke(new MethodInvoker(delegate { txtSearchID.Text = temcuon.IdCuon; }));
                        LoadDataSearch();
                        return;
                    }
                    
                    ttError.ShowHint("Định dạng ID sai"); 
                    return;

                    //double n;
                    //bool isNumeric = double.TryParse(ScanString, out n);

                    //if (!isNumeric)
                    //{
                    //    try
                    //    {
                    //        string[] split = ScanString.Split('-');
                    //        this.gluTransferNumber.Invoke(new MethodInvoker(delegate { try { gluTransferNumber.EditValue = split[1]; } catch { } }));
                    //    }
                    //    catch (Exception ex) { }
                    //}
                }
            }
        }

        private DataTable buildTableSearchID()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("productId", typeof(int));
            dt.Columns.Add("productName", typeof(string));
            dt.Columns.Add("srcPackageNumber", typeof(string));
            dt.Columns.Add("destPackageNumber", typeof(string));
            dt.Columns.Add("transferId", typeof(int));
            dt.Columns.Add("transferNumber", typeof(string));
            dt.Columns.Add("locationId", typeof(string));
            dt.Columns.Add("locationName", typeof(string));
            dt.Columns.Add("quantity", typeof(double));
            dt.Columns.Add("supplier", typeof(string));
            dt.Columns.Add("project", typeof(string));
            dt.Columns.Add("manId", typeof(int));
            dt.Columns.Add("companyCode", typeof(int));
            dt.Columns.Add("internalReference", typeof(string));
            return dt;
        }

        private void txtSearchID_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        private void LoadDataSearch()
        {
            DataTable dt = buildTableSearchID();
            DataRow dr = dt.NewRow();
            string packNumber = this.txtSearchID.Text.Trim();
            try
            {
                string url = "";
                if (packNumber.StartsWith(Config.PackPrefix))
                    url = "packages/search?query=packageNumber==\"" + packNumber + "\"";
                else if (packNumber.StartsWith(Config.LotPrefix))
                    url = "lots/search?query=lotNumber==\"" + packNumber + "\"";
                else return;
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));
                        if (packNumber.StartsWith(Config.PackPrefix))
                            dr["srcPackageNumber"] = data[0]["packageNumber"];
                        else if (packNumber.StartsWith(Config.LotPrefix))
                            dr["srcPackageNumber"] = data[0]["lotNumber"];

                        this.packageID = data[0]["id"];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error during load information package/lot !");
                        return;
                    };
                }
            }
            catch
            {
                dr["packageNumber"] = "";
                MessageBox.Show("Error during load information package/lot !");
                return;
            }

            #region Find Stock
            try
            {
                string url = "quants/search?query=packageId==" + this.packageID.ToString() + ";onHand>0";
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<StockQuantity> ListStockQuant = JsonConvert.DeserializeObject<List<StockQuantity>>(res.RawText, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        if (ListStockQuant.Count == 0)
                        {
                            MessageBox.Show("Package/Lot does not exist in stock!");
                            return;
                        }
                        else if (ListStockQuant.Count > 1)
                        {
                            MessageBox.Show("You can not split packages including roll!");
                            return;
                        }
                        else
                        {
                            dr["productId"] = ListStockQuant[0].productId;
                            dr["quantity"] = ListStockQuant[0].onHand;
                            dr["locationId"] = ListStockQuant[0].locationId;
                            dr["manId"] = ListStockQuant[0].manId;
                        }
                    }
                    catch (Exception ex)
                    { };
                }
            }
            catch
            {
                dr["packageNumber"] = "";
                MessageBox.Show("Error during load information package/lot !");
                return;
            };
            #endregion

            #region Find location
            //try
            //{
            //    string url = "transfer-details/find-last-done-by-package?Id=" + Id;
            //    var param = new { };
            //    HttpResponse res = HTTP.Instance.Get(url, param);

            //    if (res.StatusCode == System.Net.HttpStatusCode.OK)
            //    {
            //        try
            //        {
            //            var serializer = new JavaScriptSerializer();
            //            dynamic data = serializer.Deserialize(res.RawText, typeof(object));
            //            dr["destLocationName"] = data["destLocationName"];
            //        }
            //        catch (Exception ex)
            //        { };
            //    }
            //}
            //catch { dr["destLocationName"] = null; };
            #endregion

            #region Find location stock
            try
            {
                string url = "locations/" + Common.ConvertInt(dr["locationId"]);
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));
                        dr["locationName"] = data["completeName"];
                    }
                    catch (Exception ex)
                    { };
                }
            }
            catch { dr["locationName"] = null; };
            #endregion

            #region Find product name
            try
            {
                string url = "products/" + Common.ConvertInt(dr["productId"]);
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));
                        dr["productName"] = data["name"];
                    }
                    catch (Exception ex)
                    { };
                }
            }
            catch { dr["productName"] = null; };
            try
            {
                string url = "companies/" + Common.ConvertInt(dr["manId"]);
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));
                        dr["companyCode"] = data["name"];
                    }
                    catch (Exception ex)
                    { };
                }
            }
            catch { dr["companyCode"] = null; };
            if (dr["productName"] != null && dr["companyCode"] != null)
                dr["internalReference"] = Convert.ToString(dr["productName"]) + Convert.ToString(dr["companyCode"]);
            else
                dr["internalReference"] = null;
            #endregion

            #region Find transfer number
            try
            {
                string url = "transfers/get-first-by-package?Id=" + packNumber;
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        dynamic data = serializer.Deserialize(res.RawText, typeof(object));
                        dr["transferNumber"] = data["transferNumber"];

                        #region Get partner information
                        try
                        {
                            string url_partner = "companies/" + Convert.ToString(data["partnerId"]);
                            var param_partner = new { };
                            HttpResponse res_partner = HTTP.Instance.Get(url_partner, param_partner);

                            if (res_partner.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var serializer_partner = new JavaScriptSerializer();
                                dynamic data_partner = serializer_partner.Deserialize(res_partner.RawText, typeof(object));
                                dr["supplier"] = data_partner["name"];
                            }

                        }
                        catch { dr["supplier"] = null; };
                        #endregion

                        #region Get project information
                        try
                        {
                            string url_project = "product-version/" + Convert.ToString(data["productVersionId"]);
                            var param_project = new { };
                            HttpResponse res_project = HTTP.Instance.Get(url_project, param_project);

                            if (res_project.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var serializer_project = new JavaScriptSerializer();
                                dynamic data_project = serializer_project.Deserialize(res_project.RawText, typeof(object));
                                dr["project"] = data_project["name"];
                            }

                        }
                        catch { dr["project"] = null; };
                        #endregion
                    }
                    catch (Exception ex)
                    { };
                }
            }
            catch { dr["transferNumber"] = null; };
            #endregion

            dt.Rows.Add(dr);
            this.vgSearchID.DataSource = dt;
        }

        private void btnPrintAgain_Click(object sender, EventArgs e)
        {
            //LabelPackage labelPackage = new LabelPackage(wtd.internal_reference, List_allocate_package[i], "", this.transfer_info["transferNumber"], this._supplier, this._project);
            //labelPackage.Template();
        }

        private void btnRepackage_Click(object sender, EventArgs e)
        {

        }

        private void txtSearchID_KeyDown(object sender, KeyEventArgs e)
        {
            code_read.Clear();
            if (e.KeyCode == Keys.Enter)
            {
                LoadDataSearch();
            }
        }

    }
}