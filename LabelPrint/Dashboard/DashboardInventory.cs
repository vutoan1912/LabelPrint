using LabelPrint.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrint.Dashboard
{
    public partial class DashboardInventory : Form
    {
        public DashboardInventory()
        {
            InitializeComponent();
        }

        private void tileItem1_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            TransferReceipts frmTransferReceipts = new TransferReceipts();
            frmTransferReceipts.Show();
        }
    }
}
