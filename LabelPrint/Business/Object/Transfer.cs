using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Business
{
    class TransferItem
    {
        //private string _manPn = null;

        public object created { get; set; }
        public object updated { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public bool active { get; set; }
        public int id { get; set; }
        public int transferId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public int? manId { get; set; }
        public double doneQuantity { get; set; }
        public int? srcLocationId { get; set; }
        public int? destLocationId { get; set; }
        public string state { get; set; }
        public double reserved { get; set; }
        public string internalReference { get; set; }
        public string productDescription { get; set; }
        public double initialQuantity { get; set; }
        public bool selected { get; set; }
        public double foc { get; set; }
        public string manPn { get; set; }
        //public string manPn
        //{
        //    get { return _manPn; }
        //    set { _manPn = value ?? _manPn; }
        //}
    }

    class TransferDetail
    {
        public object created { get; set; }
        public object updated { get; set; }
        public string createdBy { get; set; }
        public bool active { get; set; }
        public int id { get; set; }
        public int transferId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public int? manId { get; set; }
        public double doneQuantity { get; set; }
        public int? srcLocationId { get; set; }
        public int? destLocationId { get; set; }
        public string state { get; set; }
        public double reserved { get; set; }
        public string internalReference { get; set; }
        public string productDescription { get; set; }
        public int transferItemId { get; set; }
        public int? destPackageId { get; set; }
        public string destPackageNumber { get; set; }
        public string reference { get; set; }
        public string srcLocationName { get; set; }
        public string destLocationName { get; set; }
        public int? available { get; set; }
        public string adjustmentType { get; set; }
        public string traceNumber { get; set; }
        public int? lotId { get; set; }
    }
}
