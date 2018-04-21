using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Business
{
    public class StockQuantity
    {
        public object created { get; set; }
        public object updated { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public bool active { get; set; }
        public int id { get; set; }
        public int? locationId { get; set; }
        public int? packageId { get; set; }
        public int? lotId { get; set; }
        public int? productId { get; set; }
        public int? manId { get; set; }
        public int? reserved { get; set; }
        public int? onHand { get; set; }
        public int? productVersionId { get; set; }
    }
}
