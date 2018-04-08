using EasyHttp.Http;
using LabelPrint.App_Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;

namespace LabelPrint.Business
{
    class syncInventoryLocalToErp
    {
        private Thread LTES;
        private int RecordsPerSync = 200;
        public syncInventoryLocalToErp()
        {
            //Console.WriteLine("Sync Local to ERP ... ");
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = Int32.Parse(Config.DbSyncInterval) * 60 * 60 * 1000; // by hours
            aTimer.Interval = 10000;
            aTimer.Enabled = true;
        }// Main sync function

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (LTES == null || !LTES.IsAlive)
            {
                LTES = new Thread(new ThreadStart(Todo));
                LTES.Start();
                //Console.WriteLine("Start it!!!");
            }
            else if (!LTES.IsAlive)
            {
                LTES.Start();
            }
        }

        public void Todo()
        {
            //@TODO thực hiện sync toàn bộ DB ở đây
            Console.WriteLine("Start sync Local to ERP ...");
            DoSyncTransferDetails();

            Config.LatestDbSyncTime = DateTime.Now.ToString();

            //@TODO: xóa các DL đã sync, dữ lại DL phát sinh trong vòng 2 tuần.
            //* DS các bảng cần xóa
            // ittp_tem_thung_resource
            // tem_thung_thanh_pham
            // tem_thung_thanh_pham_detail
            // fac_digital_resources_used
            // fac_digital_resources_sync
            // it_tem_thanh_pham
        }

        private void DoSyncTransferDetails()
        {
            List<wh_transfer_details> listTransferDetails;

            //get all db that haven't synced
            //Do queue sync till the end
            try
            {
                using (var dbContext = new erpEntities())
                {
                    wh_transfer_details wtd = dbContext.wh_transfer_details.Where(r => r.status == 0).OrderByDescending(u => u.transfer_id).FirstOrDefault();
                    if (wtd != null)
                    {
                        listTransferDetails = dbContext.wh_transfer_details.Where(r => r.status == 0).Where(r => r.transfer_id == wtd.transfer_id).Take(RecordsPerSync).ToList();
                        if (listTransferDetails.Count > 0)
                        {
                            string url_transfer = "transfers/" + Convert.ToString(wtd.transfer_id);
                            var param_transfer = new { };
                            HttpResponse res_transfer = HTTP.Instance.Get(url_transfer, param_transfer);

                            if (res_transfer.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                try
                                {
                                    var values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res_transfer.RawText);
                                    try { values.Remove("transferItems"); } catch { };
                                    try { values.Remove("removedTransferItems"); } catch { };
                                    try { values.Remove("removedTransferDetails"); } catch { };
                                    try { values.Remove("active"); } catch { };
                                    try { values.Remove("manOrderTransfer"); }catch { };
                                    try { values.Remove("returnedTransfer"); }catch { };

                                    string para_transfer_detail = " [ ";
                                    foreach (wh_transfer_details td in listTransferDetails)
                                    {
                                        para_transfer_detail += " { \"destLocationId\": " + td.dest_location_id + ", ";
                                        if (td.dest_package_number != null) para_transfer_detail += "\"destPackageNumber\": \"" + td.dest_package_number + "\", ";
                                        para_transfer_detail += "\"productName\": \"" + td.product_name + "\", ";
                                        para_transfer_detail += "\"doneQuantity\": " + td.done_quantity + ", ";
                                        para_transfer_detail += "\"manId\": " + td.man_id + ", ";
                                        if (td.man_pn != null) para_transfer_detail += "\"manPn\": \"" + td.man_pn + "\", ";
                                        para_transfer_detail += "\"productId\": " + td.product_id + ", ";
                                        para_transfer_detail += "\"reserved\": " + td.done_quantity + ", ";
                                        para_transfer_detail += "\"srcLocationId\": " + td.src_location_id + ", ";
                                        if(td.trace_number != null) para_transfer_detail += "\"traceNumber\": \"" + td.trace_number + "\", ";
                                        para_transfer_detail += "\"internalReference\": \"" + td.internal_reference + "\", ";
                                        para_transfer_detail += "\"reference\": \"" + td.reference + "\", ";
                                        para_transfer_detail += "\"transferId\": " + td.transfer_id + ", ";
                                        para_transfer_detail += "\"transferItemId\": " + td.transfer_item_id + " },";
                                    }
                                    string end_char = para_transfer_detail.Substring(para_transfer_detail.Length - 1, 1);
                                    if (end_char == ",") para_transfer_detail = para_transfer_detail.Substring(0, para_transfer_detail.Length - 1);
                                    para_transfer_detail += " ] ";

                                    values["transferDetails"] = (object)para_transfer_detail;

                                    string param_put = Common.DictionaryObjectToJson(values);

                                    //PUT DATA
                                    string url = "transfers/" + Convert.ToString(wtd.transfer_id);
                                    HttpResponse res = HTTP.Instance.Put(url, param_put);

                                    if (res.StatusCode == HttpStatusCode.OK)
                                    {
                                        //@TODO: Mark as sync for finished records
                                        foreach (wh_transfer_details td in listTransferDetails)
                                        {
                                            td.status = 1;
                                        }
                                        dbContext.SaveChanges();
                                        Console.WriteLine("Sync transfer details --> OK ");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Sync transfer details --> FAIL ");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Không load được dữ liệu !");
                                };
                            }
                            else
                            {
                                Console.WriteLine("Transfer data can not be retrieved !");
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Undated transfer receipts data !");
                    }
                    dbContext.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
