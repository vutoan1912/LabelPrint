using LabelPrint.Business;
using LabelPrint.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrint.Dashboard
{
    public partial class DashboardInventory : Form
    {
        public DashboardInventory()
        {
            InitializeComponent();
            syncInventoryLocalToErp ThreadSync = new syncInventoryLocalToErp();
        }

        private void tileItem1_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            TransferReceipts frmTransferReceipts = new TransferReceipts();
            frmTransferReceipts.Show();
        }

        private void printer_init()
        {
            string ClassPrint = "LabelPrint.Business.";

            string PrinterName = LabelPrint.Business.print.GetDefaultPrinter();
            string TypePrinter = null;

            //MessageBox.Show(PrinterName);

            if (PrinterName.ToLower().Trim().Contains("sato"))
            {
                TypePrinter = "SATO";

                var check_printer = Common.caller(ClassPrint + TypePrinter, "PrinterInit", new object[] { });
                //MessageBox.Show("check_printer: " + Convert.ToString(check_printer));

                if (check_printer == null || !(bool)check_printer)
                {
                    MessageBox.Show("Không tìm thấy kết nối với máy in!");
                    return;
                }

                if (PrinterName.ToLower().Trim().Contains("305")) ObjectDefine.Printer = "CL4NX305";
                else if (PrinterName.ToLower().Trim().Contains("609")) ObjectDefine.Printer = "CL4NX609";
                else MessageBox.Show("Chương trình in tem không hỗ trợ máy in đang được 'Set as Default Printer' ");
            }
            else if (PrinterName.ToLower().Trim().Contains("zebra"))
            {
                TypePrinter = "ZEBRA";
                var check_printer = Common.caller(ClassPrint + TypePrinter, "PrinterInit", new object[] { });
                //MessageBox.Show("check_printer: " + Convert.ToString(check_printer));

                if (check_printer == null || !(bool)check_printer)
                {
                    MessageBox.Show("Không tìm thấy kết nối với máy in!");
                    return;
                }
                if (PrinterName.ToLower().Trim().Contains("zm400")) ObjectDefine.Printer = "ZM400";
                else if (PrinterName.ToLower().Trim().Contains("110xi4")) ObjectDefine.Printer = "Z110Xi4";
                else MessageBox.Show("Chương trình in tem không hỗ trợ máy in đang được 'Set as Default Printer' ");
            }
            else
            {
                MessageBox.Show("Không tìm thấy kết nối với máy in!");
                return;
            }

        }

        private void DashboardInventory_Load(object sender, EventArgs e)
        {
            //Config Printer
            Thread thread_printer_init = new Thread(printer_init);
            thread_printer_init.Start();
        }
    }
}
