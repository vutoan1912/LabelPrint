using EasyHttp.Http;
using LabelPrint.App_Data;
using LabelPrint.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace LabelPrint.Inventory
{
    public partial class TransferReceiptsDetails : Form
    {
        //param form
        public int ID;
        public string PackageID;
        public string ProductName;
        public string SourcePackageNumber;
        public int SourceLocationId;
        public int DestLocationId;
        public int UomPackageId;
        public int UomLotId;
        public int DoneQuantityPackage;
        public int ManId;
        public string ManPn;
        public int ProductId;
        public int TransferId;
        public int TransferItemId;
        public string InternalReference;
        public string Reference;
        //init param
        
        public Dictionary<string, dynamic> transfer_info;
        public dynamic package_info;
        public bool CheckAllocate = false;

        private DataTable _dt_lots = new DataTable();

        public DataTable dt_lots { set { _dt_lots = value; } get { return _dt_lots; } }

        public TransferReceiptsDetails()
        {
            InitializeComponent();
        }

        private void TransferReceiptsDetails_Load(object sender, EventArgs e)
        {
            loadData();
            loadFormData();
        }

        private void loadData()
        {
            txtPackageID.Text = PackageID;

            #region Load list Lots
            if (ID > 0)
            {
                string url = "transfer-details/search?query=destPackageNumber==" + PackageID + "&size=20";
                var param = new { };
                HttpResponse res = HTTP.Instance.Get(url, param);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<ExpandoObject> list_request_paper = new List<ExpandoObject>(res.DynamicBody);
                        _dt_lots = Common.ToDataTable(list_request_paper);

                        //delete package
                        for (int i = _dt_lots.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = _dt_lots.Rows[i];
                            if (Convert.ToString(dr["traceNumber"]).Length == 0) dr.Delete();
                        }

                        this.vgListLot.DataSource = _dt_lots;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không load được dữ liệu !");
                    };
                }
                else
                {

                }
            }
            
            #endregion
        }

        private void loadFormData()
        {
            
            #region Load Package Info
            //string url_package = "transfer-details/" + ID.ToString();
            //var param_package = new { };
            //HttpResponse res_package = HTTP.Instance.Get(url_package, param_package);

            //if (res_package.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    try
            //    {
            //        var serializer = new JavaScriptSerializer();
            //        package_info = serializer.Deserialize(res_package.RawText, typeof(object));
            //        //txtSourceNumber.Text = Convert.ToString(package_info["srcPackageNumber"]);
            //        gluSourceLocation.EditValue = package_info["srcLocationId"];
            //        gluDestinationLocation.EditValue = package_info["destLocationId"];
            //        txtNumberPerPackage.Text = Convert.ToString(package_info["doneQuantity"]);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Không load được dữ liệu !");
            //    };
            //}
            //else
            //{
            //    MessageBox.Show("Không load được dữ liệu !");
            //}
            #endregion

            #region Load list Location
            string url_location = "locations/search?query=&size=20";
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

            #region Init data form
            gluSourceLocation.EditValue = this.SourceLocationId;
            gluDestinationLocation.EditValue = this.DestLocationId;
            txtNumberPerPackage.Text = Convert.ToString(DoneQuantityPackage);
            #endregion

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Bạn có chắc chắn muốn xóa tem?", "Xóa tem", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                Dictionary<string, dynamic> transfer_info_delete = transfer_info;
                //transfer_info
                transfer_info_delete.Remove("transferItems");
                transfer_info_delete.Remove("removedTransferItems");
                transfer_info_delete.Remove("transferDetails");
                transfer_info_delete.Remove("active");
                string _id_delete = "[";
                foreach (var index in grvListLot.GetSelectedRows())
                {
                    dynamic row = this.grvListLot.GetRow(index);
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
                    grvListLot.DeleteSelectedRows();
                }
                else
                {
                    MessageBox.Show("Error delete !!");
                }
            }
        }

        private void btnGrvDelete_Click(object sender, EventArgs e)
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
                DataRow row = grvListLot.GetDataRow(grvListLot.FocusedRowHandle);
                string _id_delete = "[" + Convert.ToString(row["id"]) + "]";
                transfer_info_delete["removedTransferDetails"] = _id_delete;
                string param_put = Common.DictionaryObjectToJson(transfer_info_delete);

                //PUT DATA
                string url = "transfers/" + Convert.ToString(transfer_info_delete["id"]);

                HttpResponse res = HTTP.Instance.Put(url, param_put);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    grvListLot.DeleteRow(grvListLot.FocusedRowHandle);
                }
                else
                {
                    MessageBox.Show("Error delete !!");
                }
            }
        }

        private void btnGrvEdit_Click(object sender, EventArgs e)
        {
            Dictionary<string, dynamic> transfer_info_put = new Dictionary<string, dynamic>();
            transfer_info_put = transfer_info;

            transfer_info_put.Remove("transferItems");
            transfer_info_put.Remove("removedTransferItems");
            transfer_info_put.Remove("removedTransferDetails");
            transfer_info_put.Remove("active");

            DataRow row = grvListLot.GetDataRow(grvListLot.FocusedRowHandle);
            string _id_delete = "[" + Convert.ToString(row["id"]) + "]";

            string para_transfer_detail = " [ ";
            para_transfer_detail += " { \"id\": " + row["id"] + ", ";
            para_transfer_detail += " { \"destLocationId\": " + row["destLocationId"] + ", ";
            para_transfer_detail += "\"destPackageNumber\": \"" + row["destPackageNumber"] + "\", ";
            para_transfer_detail += "\"doneQuantity\": " + row["doneQuantity"] + ", ";
            para_transfer_detail += "\"manId\": " + row["manId"] + ", ";
            if (row["manPn"] != null) para_transfer_detail += "\"manPn\": \"" + row["manPn"] + "\", ";
            para_transfer_detail += "\"productId\": " + row["productId"] + ", ";
            para_transfer_detail += "\"reserved\": " + row["reserved"] + ", ";
            para_transfer_detail += "\"srcLocationId\": " + row["srcLocationId"] + ", ";
            para_transfer_detail += "\"traceNumber\": \"" + row["traceNumber"] + "\", ";
            para_transfer_detail += "\"transferId\": " + row["transferId"] + ", ";
            para_transfer_detail += "\"transferItemId\": " + row["transferItemId"] + " }";
            para_transfer_detail += " ] ";

            transfer_info_put["transferDetails"] = (object)para_transfer_detail;

            string param_put = Common.DictionaryObjectToJson(transfer_info_put);

            //PUT DATA
            string url = "transfers/" + Convert.ToString(transfer_info_put["id"]);
            HttpResponse res = HTTP.Instance.Put(url, param_put);

            if (res.StatusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Cập nhật dữ liệu thành công !");
            }
            else
            {
                MessageBox.Show("Lỗi trong quá trình cập nhật dữ liệu !");
            }
        }

        private void btnAllocate_Click(object sender, EventArgs e)
        {
            bool status_reserve = true;
            List<string> List_allocate_lot = new List<string>();

            if (txtNumberPerLot.Text.Trim().Length > 0 && txtNumberLot.Text.Trim().Length > 0 && Common.ConvertInt(txtNumberLot.Text.Trim()) > 0)
            {
                //allocate lot
                string url_lot = "sequences/2/reserve/" + txtNumberLot.Text.Trim();
                HttpResponse res_lot = HTTP.Instance.Post(url_lot, null);

                if (res_lot.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var serializer_lot = new JavaScriptSerializer();
                        dynamic data_lot = serializer_lot.Deserialize(res_lot.RawText, typeof(object));

                        List_allocate_lot = Common.CreateSequential(Common.ConvertInt(data_lot["nextNumber"]), Common.ConvertInt(data_lot["step"]), Common.ConvertInt(data_lot["length"]), Common.ConvertInt(txtNumberLot.Text.Trim()), data_lot["prefix"]);
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
                long maxID = 0;
                using (erpEntities dbContext = new erpEntities())
                {
                    wh_transfer_details wtd = dbContext.wh_transfer_details.OrderByDescending(u => u.id).FirstOrDefault();
                    if (wtd != null) maxID = wtd.id + 1;
                }
                this._dt_lots = checkStructureDatatable(_dt_lots);

                TransferDetailsRepository TransferDetails = new TransferDetailsRepository();
                List<wh_transfer_details> list_transfer_details = new List<wh_transfer_details>();

                if (List_allocate_lot.Count > 0)
                {
                    foreach (string lot in List_allocate_lot)
                    {
                        wh_transfer_details wtd = new wh_transfer_details();
                        wtd.created = DateTime.Now;
                        wtd.src_package_number = txtSourceNumber.Text;
                        wtd.dest_location_id = Common.ConvertInt(gluDestinationLocation.EditValue);
                        wtd.dest_package_number = PackageID;
                        wtd.done_quantity = Common.ConvertDouble(txtNumberPerLot.Text);
                        wtd.id = maxID;
                        wtd.man_id = this.ManId;
                        try { wtd.man_pn = this.ManPn; }
                        catch (Exception ex) { wtd.man_pn = null; }
                        wtd.product_id = this.ProductId;
                        wtd.product_name = ProductName;
                        wtd.src_location_id = Common.ConvertInt(gluSourceLocation.EditValue);
                        wtd.status = 0;
                        wtd.transfer_id = this.TransferId;
                        wtd.transfer_item_id = this.TransferItemId;
                        wtd.trace_number = lot;
                        wtd.internal_reference = InternalReference;
                        wtd.reference = Reference;
                        list_transfer_details.Add(wtd);

                        //add to table list package
                        addPackageDataRow(InternalReference, Reference, wtd.src_package_number, wtd.dest_package_number, wtd.trace_number, Common.ConvertInt(wtd.src_location_id),
                            Common.ConvertInt(wtd.dest_location_id), Common.ConvertInt(wtd.done_quantity), Common.ConvertInt(wtd.transfer_id),
                            Common.ConvertInt(wtd.transfer_item_id), Common.ConvertInt(wtd.product_id), Common.ConvertInt(wtd.man_id));

                        //Print Lot
                        LabelPackage labelLot = new LabelPackage(wtd.product_name, lot, "");
                        labelLot.Template();

                        maxID++;
                    }
                }

                vgListLot.DataSource = _dt_lots;
                TransferDetails.Add(list_transfer_details.ToArray());

                //delete package parent
                CheckAllocate = true;
            }
        }

        private void addPackageDataRow(string _internalReference, string _reference, string _srcPackageNumber, string _destPackageNumber, string _traceNumber, int _srcLocationId, int _destLocationId,
            double _doneQuantity, int _transferId, int _transferItemId, int _productId, int _manId, string _manPn = null, int? _lotId = null, int _printed = 1)
        {
            DataRow myR = this._dt_lots.NewRow();
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
                //myR["lotId"] = _lotId;
                myR["internalReference"] = _internalReference;
                myR["reference"] = _reference;
                myR["countPrint"] = _printed;

                this._dt_lots.Rows.Add(myR);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
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
    }
}
