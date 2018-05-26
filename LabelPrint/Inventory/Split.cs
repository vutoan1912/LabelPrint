using LabelPrint.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrint.Inventory
{
    public partial class Split : Form
    {
        #region Properties

        private string packageId;
        private double quantity;
        public double quantityNew = 0;
        private bool check = false;

        public string PackageId { get { return packageId; } set { packageId = value; } }
        public double Quantity { get { return quantity; } set { quantity = value; } }

        #endregion

        public Split()
        {
            InitializeComponent();
        }

        private void Split_Load(object sender, EventArgs e)
        {
            lblReport.Text = "Split " + packageId;
            lblAttention.Text = "The quantity to be split must be less than " + quantity.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (check)
            {
                this.quantityNew = Common.ConvertDouble(txtQuantity.Text);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            quantityNew = 0;
            this.Close();
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            if (txtQuantity.Text.Trim().Length == 0)
            {
                check = false; txtQuantity.Text = null;
                ttError.ShowHint("The need to enter the number to split!"); return;
            }
            double _SoLuongNew = Common.ConvertDouble(txtQuantity.Text.Trim());
            if (_SoLuongNew <= 0)
            {
                check = false; txtQuantity.Text = null;
                ttError.ShowHint("The number to be split must be greater than 0"); return;
            }
            if (_SoLuongNew >= this.quantity)
            {
                check = false; txtQuantity.Text = null;
                ttError.ShowHint("The quantity to be split must be less than " + quantity.ToString()); return;
            }

            check = true;
        }
    }
}
