using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Business
{
    class ObjectDefine
    {

        public const string ClassPrint = "LabelPrint.Business.";
        public static string Printer = null;

        public static bool checkPrinter()
        {
            if (Printer == null) return false;
            return true;
        }

        //start char
        public static string packageID = "3S";
        public static string productNo = "P";

        public static char Separator  = '@';
        public static string Prefix = "[)>@06";
        public static string End = "@@";
    }

    class LabelPackage : ObjectDefine
    {
        #region Properties
        private string _product_no;
        private string _package_id;
        private string _product_no_old;

        public string ProductNo
        {
            get { return _product_no; }
            set { _product_no = value; }
        }

        public string PackageID
        {
            get { return _package_id; }
            set { _package_id = value; }
        }

        public string ProductNoOld
        {
            get { return _product_no_old; }
            set { _product_no_old = value; }
        }

        #endregion

        public LabelPackage(string labelContent)
        {
            ParseString(labelContent);
        }

        public LabelPackage Clone()
        {
            LabelPackage labelPackage = new LabelPackage();
            labelPackage.ProductNo = this.ProductNo;
            labelPackage.PackageID = this.PackageID;
            labelPackage.ProductNoOld = this.ProductNoOld;
            return labelPackage;
        }

        public LabelPackage() { }

        public LabelPackage(string productNo, string packageID, string productNoOld)
        {
            _product_no = productNo;
            _package_id = packageID;
            _product_no_old = productNoOld;
        }

        public bool ParseString(string labelContent)
        {
            if (labelContent.StartsWith(ObjectDefine.Prefix) && (labelContent.Substring(labelContent.Length - 3, 2) == ObjectDefine.End))
            {
                labelContent = labelContent.Substring(ObjectDefine.Prefix.Length - 1, labelContent.Length - 1);
                labelContent = labelContent.Substring(0, labelContent.Length - 3);
            }
            else return false;
            try
            {
                string[] tem_data = labelContent.Split(ObjectDefine.Separator);
                foreach (string item in tem_data)
                {
                    if (item.StartsWith(ObjectDefine.productNo))
                        _product_no = item.Substring(ObjectDefine.productNo.Length - 1, labelContent.Length - 1);
                    else if (item.StartsWith(ObjectDefine.packageID))
                        _package_id = item.Substring(ObjectDefine.packageID.Length - 1, labelContent.Length - 1);
                }
            }
            catch (Exception ex) { return false; }
            return true;
        }

        public string LabelToString()
        {
            //example: [)>@06@P80N45HGJ456YU768@3SPKG3676573658@@

            string qrString = "";
            qrString += ObjectDefine.Prefix + ObjectDefine.Separator + ObjectDefine.productNo + _product_no;
            qrString += ObjectDefine.Separator + ObjectDefine.packageID + _package_id + ObjectDefine.End;
            return qrString;
        }

        public List<KeyValuePair<string, string>> ToList()
        {
            var lstData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(ObjectDefine.productNo, _product_no),
                    new KeyValuePair<string, string>(ObjectDefine.packageID, _package_id)
                };

            return lstData;
        }

        public void Template()
        {
            if (checkPrinter())
            {

                //Format
                object[] _param = new object[] { LabelToString(), ProductNo, PackageID, ProductNoOld };
                Common.caller(ClassPrint + Printer, "Print_LabelPackage", _param);
            }
        }
    }

    #region not use

    class TemCuon : ObjectDefine
    {
        #region Properties
        private string _vnpt_pn;
        private string _so_phieu_nhap_kho;
        private string _so_don_hang;
        private string _ngay_nhap_kho;
        private string _id_cuon;
        private string _so_luong;
        private string _type;

        public string VnptPn
        {
            get { return _vnpt_pn; }
            set { _vnpt_pn = value; }
        }

        public string SoPhieuNhapKho
        {
            get { return _so_phieu_nhap_kho; }
            set { _so_phieu_nhap_kho = value; }
        }

        public string SoDonHang
        {
            get { return _so_don_hang; }
            set { _so_don_hang = value; }
        }

        public string NgayNhapKho
        {
            get { return _ngay_nhap_kho; }
            set { _ngay_nhap_kho = value; }
        }

        public string IdCuon
        {
            get { return _id_cuon; }
            set { _id_cuon = value; }
        }

        public string SoLuong
        {
            get { return _so_luong; }
            set { _so_luong = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion

        public TemCuon(string labelContent)
        {
            ParseString(labelContent);
        }

        public TemCuon Clone()
        {
            TemCuon newCuon = new TemCuon();
            newCuon.VnptPn = this.VnptPn;
            newCuon.SoPhieuNhapKho = this.SoPhieuNhapKho;
            newCuon.SoDonHang = this.SoDonHang;
            newCuon.NgayNhapKho = this.NgayNhapKho;
            newCuon.IdCuon = this.IdCuon;
            newCuon.SoLuong = this.SoLuong;
            newCuon.Type = this.Type;
            return newCuon;
        }

        public TemCuon() { }

        public TemCuon(string vnptPn, string soDonHang, string soPhieuNhapKho, string ngayNhapKho, string idCuon, string soLuong, string type)
        {
            _vnpt_pn = vnptPn;
            _so_don_hang = soDonHang;
            _so_phieu_nhap_kho = soPhieuNhapKho;
            _ngay_nhap_kho = ngayNhapKho;
            _id_cuon = idCuon;
            _so_luong = soLuong;
            _type = type;
        }

        public void ParseString(string labelContent)
        {
            try
            {
                char[] delimiterChars = { ':' };
                char[] StarChars = { '*' };

                string[] tem_data = labelContent.Split(StarChars);

                //using (StringReader reader = new StringReader(labelContent))
                //{
                //string line;
                //while ((line = reader.ReadLine()) != null)
                foreach (string line in tem_data)
                {
                    // Do something with the line
                    string[] datas = line.Split(delimiterChars);

                    string key = datas[0].ToLower();
                    switch (key)
                    {
                        case "pn":
                            {
                                _vnpt_pn = datas[1];
                            } break;
                        case "spnk":
                            {
                                _so_phieu_nhap_kho = datas[1];
                            } break;
                        case "sdh":
                            {
                                _so_don_hang = datas[1];
                            } break;
                        case "nnk":
                            {
                                _ngay_nhap_kho = datas[1];
                            } break;
                        case "id":
                            {
                                _id_cuon = datas[1];
                            } break;
                        case "sl":
                            {
                                _so_luong = datas[1];
                            } break;
                        case "type":
                            {
                                _type = datas[1];
                            } break;
                    }

                }
                //}
            }
            catch (Exception e)
            {

            }


        }

        public string TemToString()
        {
            string qrString = "";
            qrString += "pn:" + _vnpt_pn + "*";
            qrString += "spnk:" + _so_phieu_nhap_kho + "*";
            qrString += "sdh:" + _so_don_hang + "*";
            qrString += "nnk:" + _ngay_nhap_kho + "*";
            qrString += "id:" + _id_cuon + "*";
            qrString += "sl:" + _so_luong + "*";
            qrString += "type:" + _type + "*";

            return qrString;
        }

        public List<KeyValuePair<string, string>> ToList()
        {
            var lstData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("pn", _vnpt_pn),
                    new KeyValuePair<string, string>("spnk", _so_phieu_nhap_kho),
                    new KeyValuePair<string, string>("sdh", _so_don_hang),
                    new KeyValuePair<string, string>("nnk", _ngay_nhap_kho),
                    new KeyValuePair<string, string>("id", _id_cuon),
                    new KeyValuePair<string, string>("sl", _so_luong),
                    new KeyValuePair<string, string>("type", _type),
                };

            return lstData;
        }

        public void Template()
        {
            if (checkPrinter())
            {

                //Format
                string SoPhieuNhapKho_Print = SoPhieuNhapKho;
                string NgayNhapKho_Print = NgayNhapKho;
                try
                {
                    SoPhieuNhapKho_Print = SoPhieuNhapKho.Replace("RE-", "");
                    string[] s = NgayNhapKho.Split(' ');
                    NgayNhapKho_Print = s[0];
                }
                catch { };

                object[] _param = new object[] { TemToString(), SoDonHang, SoPhieuNhapKho_Print, VnptPn, NgayNhapKho_Print, IdCuon, SoLuong };
                Common.caller(ClassPrint + Printer, "Print_Cuon", _param);
            }
        }
    }

    class TemThanhPham : ObjectDefine
    {
        #region Properties
        private string _mac_imei;
        private string _seri_no;
        private string _product_type;
        private string _wps_pin;
        private string _gpon_sn;

        public string MAC_IMEI
        {
            get { return _mac_imei; }
            set { _mac_imei = value; }
        }
        public string SERI_NO
        {
            get { return _seri_no; }
            set { _seri_no = value; }
        }
        public string WPS
        {
            get { return _wps_pin; }
            set { _wps_pin = value; }
        }
        public string GPON
        {
            get { return _gpon_sn; }
            set { _gpon_sn = value; }
        }
        #endregion

        public TemThanhPham() { }

        public TemThanhPham(string mac_imei, string ma_sp, string noi_sx, string nam_sx, string ma_mau, string version, string product_type)
        {
            this._product_type = product_type;
            this._mac_imei = mac_imei;
            this._seri_no = Gen_SeriNo(ma_sp, noi_sx, nam_sx, ma_mau, version, mac_imei);
            this._wps_pin = Gen_WpsPin(mac_imei);
            this._gpon_sn = Gen_GponSN(mac_imei);
        }

        public void Template()
        {
            if (checkPrinter())
            {
                object[] _param = new object[] { this._mac_imei, this._seri_no, this._gpon_sn, this._wps_pin };
                Common.caller(ClassPrint + Printer, "Print_ThanhPham_" + this._product_type, _param);
            }
        }

        public string Gen_SeriNo(string _MaSP, string _NoiSX, string _NamSX, string _MaMau, string _Version, string _Mac_Imei)
        {
            return _MaSP + _NoiSX + _NamSX + Common.GetWeekOfYear().ToString() + _Version + _MaMau + MAC_IMEI.Substring(_Mac_Imei.Length - 6, 6);
        }

        public string Gen_WpsPin(string MAC_IMEI)
        {
            string strMD5 = Common.MD5(MAC_IMEI);
            string strCut6 = (strMD5.Substring(3, 1) + (strMD5.Substring(6, 1) + (strMD5.Substring(9, 1) + (strMD5.Substring(12, 1) + (strMD5.Substring(15, 1) + strMD5.Substring(18, 1))))));

            // vi du cho truong hop strCut cho chuoi WPS ngan hon 8 ky tu
            // strCut6 = "000001"
            long tempPIN = long.Parse(strCut6, System.Globalization.NumberStyles.HexNumber);
            long PinCodeDevice = (tempPIN % 9999999) * 10;
            long accum = 0;
            long digit;

            //accum = accum + 3 * ((PinCodeDevice \ 10000000) Mod 10)
            //accum = accum + 1 * ((PinCodeDevice \ 1000000) Mod 10)
            //accum = accum + 3 * ((PinCodeDevice \ 100000) Mod 10)
            //accum = accum + 1 * ((PinCodeDevice \ 10000) Mod 10)
            //accum = accum + 3 * ((PinCodeDevice \ 1000) Mod 10)
            //accum = accum + 1 * ((PinCodeDevice \ 100) Mod 10)
            //accum = accum + 3 * ((PinCodeDevice \ 10) Mod 10)

            accum = accum + 3 * ((PinCodeDevice / 10000000) % 10);
            accum = accum + 1 * ((PinCodeDevice / 1000000) % 10);
            accum = accum + 3 * ((PinCodeDevice / 100000) % 10);
            accum = accum + 1 * ((PinCodeDevice / 10000) % 10);
            accum = accum + 3 * ((PinCodeDevice / 1000) % 10);
            accum = accum + 1 * ((PinCodeDevice / 100) % 10);
            accum = accum + 3 * ((PinCodeDevice / 10) % 10);

            digit = (accum % 10);
            accum = ((10 - digit) % 10);
            PinCodeDevice = PinCodeDevice + accum;

            //wpspin = Format(PinCodeDevice, "00000000");
            //solution 1
            //wpspin = PinCodeDevice.ToString().PadLeft(8, '0');

            //solution 2
            return PinCodeDevice.ToString("D8");
        }

        public string Gen_GponSN(string MAC_IMEI)
        {
            string str;
            str = MAC_IMEI;
            str = str.Substring(6, 6);
            // Get 3 lower HEX values. We don't care 3 upper HEX values

            // Convert 3 lower HEX to binary
            str = Common.HexToBinary(str);

            // We create a new binary from original binary
            string strNewBin = str.Substring((str.Length - 23));
            // Remove the highest bit in binary
            string strHighest = str.Substring(0, 1);
            // Get the highest bit in binary
            strNewBin = (strNewBin + strHighest);
            // new binary = binary has removed highest bit  + the highest bit
            string strNewHex = Common.BinaryToHex(strNewBin);
            int remainder = strNewHex.Length;
            remainder = (remainder % 2);

            if (remainder != 0)
            {
                // The length of strNewHex is odd
                strNewHex = ("0" + strNewHex);
                // Insert '0' before strNewHex
            }

            return "VNPT00" + strNewHex;
            // 00: MAC range = A06518
        }
    }

    class TemThungCuon : ObjectDefine
    {
        #region Properties
        private string _vnpt_pn;
        private string _so_phieu_nhap_kho;
        private string _so_don_hang;
        private string _ngay_nhap_kho;
        private string _id_thung;
        private string _so_luong_thung;
        private string _type;

        public string VnptPn
        {
            get { return _vnpt_pn; }
            set { _vnpt_pn = value; }
        }

        public string SoPhieuNhapKho
        {
            get { return _so_phieu_nhap_kho; }
            set { _so_phieu_nhap_kho = value; }
        }

        public string SoDonHang
        {
            get { return _so_don_hang; }
            set { _so_don_hang = value; }
        }

        public string NgayNhapKho
        {
            get { return _ngay_nhap_kho; }
            set { _ngay_nhap_kho = value; }
        }

        public string IdThung
        {
            get { return _id_thung; }
            set { _id_thung = value; }
        }

        public string SoLuongThung
        {
            get { return _so_luong_thung; }
            set { _so_luong_thung = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion

        public TemThungCuon(string labelContent)
        {
            ParseString(labelContent);
        }

        public TemThungCuon Clone()
        {
            TemThungCuon newThungCuon = new TemThungCuon();
            newThungCuon.VnptPn = this.VnptPn;
            newThungCuon.SoPhieuNhapKho = this.SoPhieuNhapKho;
            newThungCuon.SoDonHang = this.SoDonHang;
            newThungCuon.NgayNhapKho = this.NgayNhapKho;
            newThungCuon.IdThung = this.IdThung;
            newThungCuon.SoLuongThung = this.SoLuongThung;
            newThungCuon.Type = this.Type;
            return newThungCuon;
        }

        public TemThungCuon() { }

        public TemThungCuon(string vnptPn, string soDonHang, string soPhieuNhapKho, string ngayNhapKho, string idThung, string soLuongThung, string type)
        {
            _vnpt_pn = vnptPn;
            _so_don_hang = soDonHang;
            _so_phieu_nhap_kho = soPhieuNhapKho;
            _ngay_nhap_kho = ngayNhapKho;
            _id_thung = idThung;
            _so_luong_thung = soLuongThung;
            _type = type;
        }

        public void ParseString(string labelContent)
        {
            try
            {
                char[] delimiterChars = { ':' };
                char[] StarChars = { '*' };

                string[] tem_data = labelContent.Split(StarChars);

                //using (StringReader reader = new StringReader(labelContent))
                //{
                //string line;
                //while ((line = reader.ReadLine()) != null)
                foreach (string line in tem_data)
                {
                    // Do something with the line
                    string[] datas = line.Split(delimiterChars);

                    string key = datas[0].ToLower();
                    switch (key.Trim())
                    {
                        case "pn":
                            {
                                _vnpt_pn = datas[1];
                            } break;
                        case "spnk":
                            {
                                _so_phieu_nhap_kho = datas[1];
                            } break;
                        case "sdh":
                            {
                                _so_don_hang = datas[1];
                            } break;
                        case "nnk":
                            {
                                _ngay_nhap_kho = datas[1];
                            } break;
                        case "id":
                            {
                                _id_thung = datas[1];
                            } break;
                        case "sl":
                            {
                                _so_luong_thung = datas[1];
                            } break;
                        case "type":
                            {
                                _type = datas[1];
                            } break;
                    }

                }
                //}
            }
            catch (Exception e)
            {

            }


        }

        public string TemToString()
        {
            string qrString = "";
            qrString += "pn:" + _vnpt_pn + "*";
            qrString += "spnk:" + _so_phieu_nhap_kho + "*";
            qrString += "sdh:" + _so_don_hang + "*";
            qrString += "nnk:" + _ngay_nhap_kho + "*";
            qrString += "id:" + _id_thung + "*";
            qrString += "sl:" + _so_luong_thung + "*";
            qrString += "type:" + _type + "*";

            return qrString;
        }

        public List<KeyValuePair<string, string>> ToList()
        {
            var lstData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("pn", _vnpt_pn),
                    new KeyValuePair<string, string>("spnk", _so_phieu_nhap_kho),
                    new KeyValuePair<string, string>("sdh", _so_don_hang),
                    new KeyValuePair<string, string>("nnk", _ngay_nhap_kho),
                    new KeyValuePair<string, string>("id", _id_thung),
                    new KeyValuePair<string, string>("sl", _so_luong_thung),
                    new KeyValuePair<string, string>("type", _type)
                };
            return lstData;
        }

        public void Template()
        {
            if (checkPrinter())
            {
                //Format
                string SoPhieuNhapKho_Print = SoPhieuNhapKho;
                string NgayNhapKho_Print = NgayNhapKho;
                try
                {
                    SoPhieuNhapKho_Print = SoPhieuNhapKho.Replace("RE-", "");
                    string[] s = NgayNhapKho.Split(' ');
                    NgayNhapKho_Print = s[0];
                }
                catch { };

                //Print
                object[] _param = new object[] { TemToString(), SoDonHang, SoPhieuNhapKho_Print, VnptPn, NgayNhapKho_Print, IdThung, SoLuongThung };
                Common.caller(ClassPrint + Printer, "Print_ThungCuon", _param);
            }
        }
    }

    class TemMACIMEI : ObjectDefine
    {
        #region Properties
        private string _data;
        //format
        //private string _horizontal;
        //private string _vertical;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        //public string Horizontal
        //{
        //    get { return _horizontal; }
        //    set { _horizontal = value; }
        //}

        //public string Vertical
        //{
        //    get { return _vertical; }
        //    set { _vertical = value; }
        //}

        #endregion

        public TemMACIMEI() { }

        public TemMACIMEI(string data)
        {
            _data = data;
        }

        public void Template()
        {
            if (checkPrinter())
            {
                object[] _param = new object[] { _data };
                Common.caller(ClassPrint + Printer, "Print_MAC_IMEI", _param);
            }
        }

    }

    class TemSerialNo : ObjectDefine
    {
        #region Properties
        private string _data;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        public TemSerialNo() { }

        public TemSerialNo(string _MaSP, string _NoiSX, string _NamSX, string _MaMau, string _Version, string MAC_IMEI)
        {
            _data = _MaSP + _NoiSX + _NamSX + Common.GetWeekOfYear().ToString() + _Version + _MaMau + MAC_IMEI.Substring(MAC_IMEI.Length - 6, 6);
        }

        public string Gen_SeriNo(string _MaSP, string _NoiSX, string _NamSX, string _MaMau, string _Version, string MAC_IMEI)
        {
            return _MaSP + _NoiSX + _NamSX + Common.GetWeekOfYear().ToString() + _Version + _MaMau + MAC_IMEI.Substring(MAC_IMEI.Length - 6, 6);
        }

        public void Template()
        {
            if (checkPrinter())
            {
                object[] _param = new object[] { _data };
                Common.caller(ClassPrint + Printer, "Print_SerialNo", _param);
            }
        }


    }

    class TemViTri : ObjectDefine
    {
        #region Properties
        private string _data;
        //format
        //private string _horizontal;
        //private string _vertical;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        //public string Horizontal
        //{
        //    get { return _horizontal; }
        //    set { _horizontal = value; }
        //}

        //public string Vertical
        //{
        //    get { return _vertical; }
        //    set { _vertical = value; }
        //}

        #endregion

        public TemViTri() { }

        public TemViTri(string data)
        {
            _data = data;
        }

        public void Template()
        {
            if (checkPrinter())
            {
                object[] _param = new object[] { _data };
                Common.caller(ClassPrint + Printer, "Print_ViTri", _param);
            }
        }

    }

    class TemThung : ObjectDefine
    {
        #region Properties

        private string _NMDT;
        private string _MaSP;
        private string _LoSX;
        private string _FirmWare;
        private List<string> _List_SeriNo;

        public string NMDT
        {
            get { return _NMDT; }
            set { _NMDT = value; }
        }
        public string MaSP
        {
            get { return _MaSP; }
            set { _MaSP = value; }
        }
        public string LoSX
        {
            get { return _LoSX; }
            set { _LoSX = value; }
        }
        public string FirmWare
        {
            get { return _FirmWare; }
            set { _FirmWare = value; }
        }
        public List<string> List_SeriNo
        {
            get { return _List_SeriNo; }
            set { _List_SeriNo = value; }
        }

        #endregion

        public TemThung() { }

        public TemThung(List<string> List_SeriNo, string MaSP, string LoSX, string NMDT = "1", string FirmWare = null)
        {
            this._List_SeriNo = List_SeriNo;
            this._MaSP = MaSP;
            this._LoSX = LoSX;
            this._NMDT = NMDT;
            this._FirmWare = FirmWare;
        }

        public void Template()
        {
            if (checkPrinter())
            {
                object[] _param = new object[] { _List_SeriNo, _MaSP, _LoSX, _NMDT, _FirmWare };
                Common.caller(ClassPrint + Printer, "Print_Thung", _param);
            }
        }

    }

    #endregion

}
