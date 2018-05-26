using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Business;

namespace LabelPrint.Dashboard
{
    public partial class ConfigApiUrl : Form
    {
        public ConfigApiUrl()
        {
            InitializeComponent();
        }

        private void ConfigApiUrl_Load(object sender, EventArgs e)
        {
            this.txtApiUrl.Text = Config.API_URL;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.API_URL = txtApiUrl.Text.Trim();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
