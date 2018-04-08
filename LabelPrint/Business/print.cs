using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Drawing;
using System.IO;
using SATOPrinterAPI;
using System.IO;

namespace LabelPrint.Business
{
    class print
    {
        public static string GetDefaultPrinter()
        {
            string PrinterName = "";
            try
            {
                var dialog = new PrintDialog();
                PrinterName = dialog.PrinterSettings.PrinterName;
            }
            catch (Exception ex) { }

            return PrinterName;
        }
    }

    public class MyType
    {
        public MyType()
        {
            this.Values = new Dictionary<object, object>();
        }

        public Dictionary<object, object> Values
        {
            get;
            set;
        }
    }

    class SATO
    {
        public static Printer SATOPrinter = null;
        public static Driver SATODriver = null;

        public SATO() { }

        public SATO(string port = null)
        {
            PrinterInit();
        }

        public bool PrinterInitFull(string port = null)
        {
            SATOPrinter = new Printer();
            SATODriver = new Driver();
            Config Con = new Config();

            if (GetPortID(3) != null)
                Con.SetInterface(3, GetPortID(3));
            else if (GetPortID(2) != null)
                Con.SetInterface(2, GetPortID(2));
            else if (GetPortID(1) != null)
                Con.SetInterface(1, GetPortID(1));
            else if (GetTCPIP() != null)
                Con.SetInterface(0, null, port, GetTCPIP());
            else return false;
            return true;
        }

        public bool PrinterInit()
        {
            SATOPrinter = new Printer();
            SATODriver = new Driver();
            Config Con = new Config();

            if (GetPortID(3) != null)
            {
                Con.SetInterface(3, GetPortID(3));
                return true;
            }
            return false;
        }

        public void Print(string data)
        {
            byte[] cmddata = Encode.gen_command(data);
            SATOPrinter.Send(cmddata);
        }

        private string GetPortID(int type)
        {
            switch (type)
            {
                case 1: //COM
                    foreach (string comport in SATOPrinter.GetCOMList())
                    {
                        return comport;
                    }
                    break;
                case 2: //LPT
                    foreach (string lptport in SATOPrinter.GetLPTList())
                    {
                        return lptport;
                    }
                    break;
                case 3: //USB
                    foreach (Printer.USBInfo usbPorts in SATOPrinter.GetUSBList())
                    {
                        return usbPorts.PortID.ToString();
                    }
                    break;
            }
            return null;
        }

        private string GetTCPIP()
        {
            foreach (Printer.TCPIPInfo TCPInfo in SATOPrinter.GetTCPIPList())
            {
                return TCPInfo.IPAddress.ToString();
            }
            return null;
        }

        private string GetDriver()
        {
            foreach (Driver.Info DriverInfo in SATODriver.GetDriverList())
            {
                return DriverInfo.DriverName.ToString();
            }
            return null;
        }

        public class Encode
        {
            //Document: CL4NX Programming Reference.pdf

            public static Dictionary<string, int> Name2Idx = new Dictionary<string, int>();
            //BASE
            public static string ESC = "\x1B";
            public string Space = " ";
            public string Start = ESC + "A ";
            public string End = ESC + "Z ";
            public static string Quantity = ESC + "Q";
            //SYSTEM
            public static string Speed = ESC + "CS";
            public static string Darkness = ESC + "#F";
            public static string DarknessCompatible = ESC + "#E";
            public static string Size = ESC + "A1";
            public static string MultipleCut = ESC + "~";
            public static string NumberCut = ESC + "CT";
            public static string ClearMemory = ESC + "*";
            //FORMAT
            public static string Pitch = ESC + "P";
            public static string Enlargement = ESC + "L";
            public static string Horizontal = ESC + "H";
            public static string Vertical = ESC + "V";
            public static string Rotation = ESC + "%";

            #region DEFINE BARCODE & QRCODE FONT
            public static string font_Barcode_CODE39_B = ESC + "B1";
            public static string font_Barcode_CODE39_D = ESC + "D1";
            public static string font_Barcode_CODE93 = ESC + "BC";
            public static string font_Barcode_UPC = ESC + "BF";
            public static string font_Barcode_CODE128 = ESC + "BG";
            public static string font_Barcode_EAN128 = ESC + "BI";
            public static string font_Barcode_UPC_A = ESC + "BL";          //Without HRI
            public static string font_Barcode_UPC_A_HRI = ESC + "BM";      //With HRI
            public static string font_QRCode = ESC + "BQ";
            public static string font_Barcode_DataMatrix = ESC + "BX";
            public static string font_Barcode_DataMatrix_DataSpecify = ESC + "DC";
            #endregion

            #region DEFINE TEXT FONT
            public static string font_X20 = ESC + "X20";
            public static string font_X21 = ESC + "X21";
            public static string font_X22 = ESC + "X22";
            public static string font_X23 = ESC + "X23";
            public static string font_X24 = ESC + "X24";                   //very big
            public static string font_XU = ESC + "XU";                     //very small
            public static string font_XS = ESC + "XS";
            public static string font_XM = ESC + "XM";
            public static string font_XB = ESC + "XB";
            public static string font_XL = ESC + "XL";                     //very big
            public static string font_OA = ESC + "OA";
            public static string font_OB = ESC + "OB";
            public static string font_S = ESC + "S";
            public static string font_M = ESC + "M";
            #endregion

            public static void init()
            {
                int idx = 0;
                string[] ASCIIMAP = { "<NUL>", "<SOH>", "<STX>", "<ETX>", "<EOT>", "<ENQ>", "<ACK>", "<BEL>", "<BS>" };
                foreach (string s in ASCIIMAP)
                {
                    Name2Idx.Add(s, idx);
                    idx++;
                }
                Name2Idx.Add("<ESC>", 0x1b);
                Name2Idx.Add("<NAK>", 0x15);
                Name2Idx.Add("<DC2>", 0x12);
            }

            public static byte[] gen_command(string data)
            {
                return Utils.StringToByteArray(data);
            }

            //Base
            public string set_Quantity(string value = "1")
            {
                return Quantity + value + Space;
            }

            #region TEXT FONT

            public string gen_text_X20(string data)
            {
                return font_X20 + "," + data + Space;
            }

            public string gen_text_X21(string data)
            {
                return font_X21 + "," + data + Space;
            }

            public string gen_text_X22(string data)
            {
                return font_X22 + "," + data + Space;
            }

            public string gen_text_X23(string data, string Smoothing = "0")
            {
                //Smoothing    =    0 :  Smoothing disabled 
                //             =    1 :  Smoothing ON

                return font_X23 + "," + Smoothing + data + Space;
            }

            public string gen_text_X24(string data, string Smoothing = "0")
            {
                //Smoothing    =    0 :  Smoothing disabled 
                //             =    1 :  Smoothing ON

                return font_X24 + "," + Smoothing + data + Space;
            }

            public string gen_text_XU(string data)
            {
                return font_XU + data + Space;
            }

            public string gen_text_XS(string data)
            {
                return font_XS + data + Space;
            }

            public string gen_text_XM(string data)
            {
                return font_XM + data + Space;
            }

            public string gen_text_XB(string data, string Smoothing = "0")
            {
                //Smoothing  =  0:    Smoothing OFF 
                //              1:    Smoothing ON (Valid for expansion factors <L> between 3 and 9)

                return font_XB + Smoothing + data + Space;
            }

            public string gen_text_XL(string data, string Smoothing = "0")
            {
                //Smoothing  =  0:    Smoothing OFF 
                //              1:    Smoothing ON (Valid for expansion factors <L> between 3 and 9)

                return font_XL + Smoothing + data + Space;
            }

            public string gen_text_OA(string data)
            {
                return font_OA + data + Space;
            }

            public string gen_text_OB(string data)
            {
                return font_OB + data + Space;
            }

            public string gen_text_S(string data)
            {
                return font_S + data + Space;
            }

            public string gen_text_M(string data)
            {
                return font_M + data + Space;
            }

            #endregion

            #region BARCODE & QRCODE FONT

            public string gen_barcode_CODE39_B(string data, string NarrowBarWidth = "02", string Height = "080")
            {
                //<B>abbcccn
                //a  [Barcode type]      =  1
                //b  [Narrow bar width]  =  Valid range : 01 to 36 dots 
                //c  [Barcode height]    =  Valid range : 001 to 999 dots 
                //n  [Print data]        =  Data

                return font_Barcode_CODE39_B + NarrowBarWidth + Height + " *" + data + "* ";
            }

            public string gen_barcode_CODE39_D(string data, string NarrowBarWidth = "02", string Height = "080")
            {
                //<D>abbcccn
                //a  [Barcode type]      =  1
                //b  [Narrow bar width]  =  Valid range : 01 to 36 dots 
                //c  [Barcode height]    =  Valid range : 001 to 999 dots 
                //n  [Print data]        =  Data

                return font_Barcode_CODE39_D + NarrowBarWidth + Height + " *" + data + "* ";
            }

            public string gen_barcode_CODE93(string data, string NarrowBarWidth = "02", string Height = "080", string Digit = "12")
            {
                //<BC>aabbbccn
                //a  [Narrow bar]           =  Valid Range  :  01 ~ 36 dots
                //b  [Height of Barcode]    =  Valid Range  :  001 ~ 999 dots
                //c  [Digit No. of data]    =  Valid Range  :  01 ~ 99
                //n  [Print data]           =  Barcode data

                return font_Barcode_CODE93 + NarrowBarWidth + Height + Digit + Space + data + Space;
            }

            public string gen_barcode_UPC(string data, string NarrowBarWidth = "02", string Height = "080")
            {
                //<BF>aabbbn
                //a  [Narrow bar]           =  Valid Range  :  01 ~ 36 dots
                //b  [Height of Barcode]    =  Valid Range  :  001 ~ 999 dots
                //n  [Print data]           =  Barcode data

                return font_Barcode_UPC + NarrowBarWidth + Height + Space + data + Space;
            }

            public string gen_barcode_CODE128(string data, string NarrowBarWidth = "02", string Height = "080")
            {
                //<BG>aabbbn
                //a  [Narrow bar]           =  Valid Range  :  01 ~ 36 dots
                //b  [Height of Barcode]    =  Valid Range  :  001 ~ 999 dots
                //n  [Print data]           =  Barcode data

                return font_Barcode_CODE128 + NarrowBarWidth + Height + Space + data + Space;
            }

            public string gen_barcode_EAN128(string data, string NarrowBarWidth = "02", string Height = "080", string Expository = "2")
            {
                //<BI>aabbbcn
                //a  [Narrow bar]                             =  Valid Range  :  01 ~ 36 dots
                //b  [Height of Barcode]                      =  Valid Range  :  001 ~ 999 dots
                //c  [Barcode expository font specification]  =  0      :  No HRI
                //                                               1      :  HRI is available (Upper part of barcode)
                //                                               2      :  HRI is available (Under part of barcode)
                //n  [Print data]                             =  Barcode data

                return font_Barcode_EAN128 + NarrowBarWidth + Height + Expository + Space + data + Space;
            }

            public string gen_barcode_UPC_A(string data, string type = "H", string NarrowBarWidth = "02", string Height = "080")
            {
                //<BL>abbcccn
                //a[Barcode type]      =  H            ：  UPC-A(Fixed 'H')
                //b[Narrow bar]        =  Valid Range  ：  01~36 dots
                //c[Height of Barcode] =  Valid Range  ：  001~999 dots
                //n[Print data]        =  Data         ：  11 fixed digits

                return font_Barcode_UPC_A + type + NarrowBarWidth + Height + Space + data + Space;
            }

            public string gen_barcode_UPC_A_HRI(string data, string type = "H", string NarrowBarWidth = "02", string Height = "080")
            {
                //<BL>abbcccn
                //a[Barcode type]      =  H            ：  UPC-A(Fixed 'H')
                //b[Narrow bar]        =  Valid Range  ：  01~36 dots
                //c[Height of Barcode] =  Valid Range  ：  001~999 dots
                //n[Print data]        =  Data         ：  11 fixed digits

                return font_Barcode_UPC_A_HRI + type + NarrowBarWidth + Height + Space + data + Space;
            }

            public string gen_qrcode(string data, string CorrectionLevel = "3", string ConcatenationMode = "0", string Size = "06", string CharacterMode = "2")
            {
                //a  [Error correction level]    =  1  :  7%     High density level (L)   
                //                                  2  :  15%     Standard level (M) 
                //                                  3  :  30%     High reliability level (H) 
                //                                  4  :  15%     High reliability level (Q) 
                //b  [Concatenation mode]        =  0  :  Normal mode 
                //                                  1  :  Concatenation mode 
                //c  [Size of one side of cell]  =  Valid Range :  01 to 99 (dot)
                //g  [Character mode]            =  1  :  Number mode   
                //                                  2  :  Alphanumeric mode
                //                                  3  :  Binary mode 
                //                                  4  :  Kanji mode

                return font_QRCode + CorrectionLevel + ConcatenationMode + Size + "," + CharacterMode + Space + data;
            }

            public string gen_data_matrix_barcode(string data, string FormatID = "01", string ErrorCorrectionLevel = "20", string CellWidth = "16", string CellPitch = "16",
                        string NumberCellsPerLine = "000", string NumberCellsLine = "000", string MirrorImage = "0", string SizeGuideCell = "01")
            {
                //Format: <BX>aabbccddeeefffghh

                //a [Format ID] = Valid Range : 01 (Fixed)
                //b [Error correction level] = Valid Range : 20 (Fixed)
                //c [Cell width] = Valid Range : 01 to 16 (dot cell)
                //d [Cell pitch] = Valid Range : 01 to 16 (dot cell)
                //e [Number of cells per line] = Valid Range : 010 to 144 //000 : (Auto setup)
                //f [Number of cell lines] = Valid Range : 008 to 414 //000 : (Auto setup)
                //g [Mirror image] = Valid Range : 0 (Fixed)
                //h [Size of guide cell] = Valid Range : 01 (Fixed)

                return font_Barcode_DataMatrix + FormatID + ErrorCorrectionLevel + CellWidth + CellPitch + NumberCellsPerLine + NumberCellsLine + MirrorImage + SizeGuideCell + font_Barcode_DataMatrix_DataSpecify + data;
            }

            #endregion

            #region FORMAT

            public string set_Horizontal(string value)
            {
                return Horizontal + value;
            }

            public string set_Vertical(string value)
            {
                return Vertical + value;
            }

            public string set_Pitch(string value)
            {
                return Pitch + value;
            }

            public string set_Enlargement(string horizontal, string vertical)
            {
                //<L>aabb
                //aa[Horizontal enlargement ratio]  =  Valid range:    01 to 36
                //bb[Vertical enlargement ratio]    =  Valid range:    01 to 36

                return Enlargement + horizontal + vertical;
            }

            public string set_Rotation(string value)
            {
                //0:    Parallel 1 (0 degree)            
                //1:    Serial 1 (90-degree) 
                //2:    Parallel 2 (180-degree) 
                //3:    Serial 2 (270-degree) 

                return Rotation + value;
            }

            #endregion

            #region SYSTEM

            public string set_Speed(string value)
            {
                //Head density          Initial value [aa]      Parameter Valid Range   
                //8dots/mm(203dpi)      6                       2, 3, 4, 5, 6, 7, 8, 9, 10 
                //12dots/mm(305dpi)     6                       2, 3, 4, 5, 6, 7, 8 
                //24dots/mm(609dpi)     4                       2, 3, 4, 5, 6

                //Print speed corresponding to parameter
                //2:    2(inch/s)     50.8 (mm/s) 
                //3:    3(inch/s)     76.2 (mm/s) 
                //4:    4(inch/s)     101.6 (mm/s) 
                //5:    5(inch/s)     127.0 (mm/s) 
                //6:    6(inch/s)     152.4 (mm/s) 
                //7:    7(inch/s)     177.8 (mm/s) 
                //8:    8(inch/s)     203.2 (mm/s) 
                //9:    9(inch/s)     228.6 (mm/s) 
                //10:   10(inch/s)    254.0 (mm/s) 

                return Speed + value + Space;
            }

            public string set_Darkness(string level = "9", string specification = "A")
            {
                //<#F>aab
                //aa  [Print darkness level specification]   =  01 ~ 10
                //b   [Print darkness specification]         =  A to F (omissible) - This parameter is usually "A"

                return Darkness + level + specification + Space;
            }

            public string set_DarknessCompatible(string level = "5", string specification = "A")
            {
                //<#E>ab
                //a   [Print darkness level specification]   =  1 ~ 5
                //b   [Print darkness specification]         =  A to F (omissible) - This parameter is usually "A"

                return Darkness + level + specification + Space;
            }

            public string set_Size(string height, string width)
            {
                //<A1>aaaabbbb
                //a  [Height of label]   =    Valid range:      Refer to the table below 
                //b  [Width of label]    =    Valid range:      Refer to the table below

                //Head density            Width of label (dots)       Height of label (dots)
                //8dots/mm (203 dpi)      1 to 832                    1 to 20000 
                //12dots/mm (305 dpi)     1 to 1248                   1 to 18000 
                //24dots/mm (609 dpi)     1 to 2496                   1 to 9600 

                return Size + height + width + Space;
            }

            public string set_MultipleCut(string value = "1")
            {
                //value  [Number of prints before cutting]  =  Valid Range  :  0 to 9999
                return MultipleCut + value + Space;
            }

            public string set_NumberCut(string value = "1")
            {
                //value  [Number of labels between each cut]  =  Qty range  :  0 to 9999
                return NumberCut + value + Space;
            }

            public string MemoryClear(string item = "")
            {
                //a  [Item to be cleared]  =  Not specified:    Single item buffer, Receive buffer, Edit buffer (reprint is not possible)
                //                                              Multi item buffer, Receive buffer, Edit buffer (Clears job in parsing)
                //                            T  :  User defined characters
                //                            &  :  Form overlay
                //                            X  :  All clear (Receive buffer, Edit buffer, User defined characters, form overlay)

                return NumberCut + item + Space;
            }

            #endregion
        }

        class Config
        {
            public void SetInterface(int type_connect, string PortID, string PortIP = null, string IPAddress = null)
            {
                try
                {
                    //type_connect = 0: use PortIP, IPAddress
                    //type_connect = 1,2,3: use PortID
                    switch (type_connect)
                    {
                        case 0: //Socket
                            SATOPrinter.Interface = Printer.InterfaceType.TCPIP;
                            SATOPrinter.TCPIPAddress = IPAddress;
                            SATOPrinter.TCPIPPort = PortIP;
                            break;
                        case 1: //COM
                            SATOPrinter.Interface = Printer.InterfaceType.COM;
                            SATOPrinter.COMPort = PortID;
                            break;
                        case 2: //LPT
                            SATOPrinter.Interface = Printer.InterfaceType.LPT;
                            SATOPrinter.LPTPort = PortID;
                            break;
                        case 3: //USB
                            SATOPrinter.Interface = Printer.InterfaceType.USB;
                            SATOPrinter.USBPortID = PortID;
                            break;
                        //default:
                        //    MessageBox.Show("Error : Invalid Interface Selection!");
                        //    break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }

    class CL4NX305 : SATO
    {
        public string Space = " ";

        public CL4NX305() { }

        public void Print_MAC_IMEI(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Horizontal("30") + encode.set_Vertical("50") + encode.set_Pitch("1") + Space + encode.gen_text_XM("MAC");
            data += encode.set_Horizontal("100") + encode.set_Vertical("85") + encode.set_Pitch("15") + Space + encode.set_Enlargement("02", "02") + encode.gen_text_XS(value);
            data += encode.set_Horizontal("100") + encode.set_Vertical("0") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(value);
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ThungCuon(string qrdata, string sdh, string spnk, string pn, string nnk, string id, string sl)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            //<ESC>V10<ESC>H30<ESC>P1<ESC>XM So don hang: 
            //<ESC>V35<ESC>H30<ESC>P1<ESC>XM VNPT/TECH-BO-11/2016-18
            //<ESC>V65<ESC>H30<ESC>P1<ESC>XM So phieu nhap kho: 
            //<ESC>V90<ESC>H30<ESC>P1<ESC>XM PART-IMP-26
            //<ESC>V120<ESC>H30<ESC>P1<ESC>XM VNPT P/N: 
            //<ESC>V145<ESC>H30<ESC>P1<ESC>XM 73BJ1TOOGJAid
            //<ESC>V175<ESC>H30<ESC>P1<ESC>XM Ngay nhap kho: 
            //<ESC>V200<ESC>H30<ESC>P1<ESC>XM 2016-12-01
            //<ESC>V230<ESC>H30<ESC>P1<ESC>XM ID Thung: 
            //<ESC>V255<ESC>H30<ESC>P1<ESC>XM 0000007280
            //<ESC>V285<ESC>H30<ESC>P1<ESC>XM SL linh kien: 
            //<ESC>V310<ESC>H30<ESC>P1<ESC>XM 2000
            //<ESC>V85<ESC>H360<ESC>BQ3005,2

            data += encode.set_Vertical("10") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("So shipment:");
            data += encode.set_Vertical("35") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(sdh);
            data += encode.set_Vertical("65") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("So phieu nhap kho:");
            data += encode.set_Vertical("90") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(spnk);
            data += encode.set_Vertical("120") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("VNPT P/N:");
            data += encode.set_Vertical("145") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(pn);
            data += encode.set_Vertical("175") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("Ngay nhap kho:");
            data += encode.set_Vertical("200") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(nnk);
            data += encode.set_Vertical("230") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("ID Thung:");
            data += encode.set_Vertical("255") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(id);
            data += encode.set_Vertical("285") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("SL linh kien:");
            data += encode.set_Vertical("310") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(sl);
            data += encode.set_Vertical("85") + encode.set_Horizontal("370") + encode.gen_qrcode(qrdata, Size: "05");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_Cuon(string qrdata, string sdh, string spnk, string pn, string nnk, string id, string sl)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Vertical("10") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("So shipment:");
            data += encode.set_Vertical("35") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(sdh);
            data += encode.set_Vertical("65") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("So phieu nhap kho:");
            data += encode.set_Vertical("90") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(spnk);
            data += encode.set_Vertical("120") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("VNPT P/N:");
            data += encode.set_Vertical("145") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(pn);
            data += encode.set_Vertical("175") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("Ngay nhap kho:");
            data += encode.set_Vertical("200") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(nnk);
            data += encode.set_Vertical("230") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("ID Cuon:");
            data += encode.set_Vertical("255") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(id);
            data += encode.set_Vertical("285") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM("SL linh kien:");
            data += encode.set_Vertical("310") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XM(sl);
            data += encode.set_Vertical("85") + encode.set_Horizontal("370") + encode.gen_qrcode(qrdata, Size: "05");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ViTri(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Horizontal("40") + encode.set_Vertical("230") + encode.set_Pitch("15") + encode.set_Enlargement("02", "04") + Space + encode.gen_text_XB(value);
            data += encode.set_Horizontal("40") + encode.set_Vertical("50") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(value, "03", "150");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_LabelPackage(string barcode_data, string productNo, string packageID, string productNoOld, string TransferNo, string Supplier, string Project)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            //qrdata = "[)>@06@PHY5ND35N000001@3SPKG_00072@@";

            data += encode.set_Vertical("90") + encode.set_Horizontal("40") + encode.gen_data_matrix_barcode(barcode_data);

            data += encode.set_Vertical("20") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_X23("Product No.");
            data += encode.set_Vertical("50") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_XL(productNo);
            data += encode.set_Vertical("90") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_X23("Package-ID");
            data += encode.set_Vertical("120") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_XL(packageID);
            data += encode.set_Vertical("160") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_X23("Supplier");
            data += encode.set_Vertical("190") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_XL(Supplier);
            data += encode.set_Vertical("230") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_X23("Project");
            data += encode.set_Vertical("260") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_XL(Project);
            data += encode.set_Vertical("300") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_X23("Transfer No.");
            data += encode.set_Vertical("330") + encode.set_Horizontal("270") + encode.set_Pitch("1") + encode.gen_text_XL(TransferNo);
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }
    }

    class CL4NX609 : SATO
    {
        public string Space = " ";

        public CL4NX609() { }

        public void Print_MAC_IMEI(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Horizontal("10") + encode.set_Vertical("100") + encode.set_Pitch("1") + Space + encode.gen_text_X23("MAC");
            data += encode.set_Horizontal("130") + encode.set_Vertical("160") + encode.set_Pitch("36") + encode.set_Enlargement("02", "04") + Space + encode.gen_text_XB(value);
            data += encode.set_Horizontal("130") + encode.set_Vertical("30") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(value, "04", "120");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_SerialNo(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Horizontal("30") + encode.set_Vertical("100") + encode.set_Pitch("1") + Space + encode.gen_text_X23("SN");
            data += encode.set_Horizontal("110") + encode.set_Vertical("160") + encode.set_Pitch("21") + encode.set_Enlargement("02", "04") + Space + encode.gen_text_XB(value);
            data += encode.set_Horizontal("110") + encode.set_Vertical("30") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(value, "03", "120");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ViTri(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Horizontal("110") + encode.set_Vertical("160") + encode.set_Pitch("21") + encode.set_Enlargement("02", "04") + Space + encode.gen_text_XB(value);
            data += encode.set_Horizontal("110") + encode.set_Vertical("30") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(value, "03", "120");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ThungCuon(string qrdata, string sdh, string spnk, string pn, string nnk, string id, string sl)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Vertical("30") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("So shipment:");
            data += encode.set_Vertical("90") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(sdh);
            data += encode.set_Vertical("150") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("So phieu nhap kho:");
            data += encode.set_Vertical("210") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(spnk);
            data += encode.set_Vertical("270") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("VNPT P/N:");
            data += encode.set_Vertical("330") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(pn);
            data += encode.set_Vertical("390") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("Ngay nhap kho:");
            data += encode.set_Vertical("450") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(nnk);
            data += encode.set_Vertical("510") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("ID Thung:");
            data += encode.set_Vertical("570") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(id);
            data += encode.set_Vertical("635") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("SL linh kien:");
            data += encode.set_Vertical("630") + encode.set_Horizontal("300") + encode.set_Pitch("1") + encode.gen_text_XL(sl);
            data += encode.set_Vertical("230") + encode.set_Horizontal("680") + encode.gen_qrcode(qrdata, Size: "09");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_Cuon(string qrdata, string sdh, string spnk, string pn, string nnk, string id, string sl)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Vertical("30") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("So shipment:");
            data += encode.set_Vertical("90") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(sdh);
            data += encode.set_Vertical("150") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("So phieu nhap kho:");
            data += encode.set_Vertical("210") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(spnk);
            data += encode.set_Vertical("270") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("VNPT P/N:");
            data += encode.set_Vertical("330") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(pn);
            data += encode.set_Vertical("390") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("Ngay nhap kho:");
            data += encode.set_Vertical("450") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(nnk);
            data += encode.set_Vertical("510") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("ID Cuon:");
            data += encode.set_Vertical("570") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_XL(id);
            data += encode.set_Vertical("635") + encode.set_Horizontal("30") + encode.set_Pitch("1") + encode.gen_text_X23("SL linh kien:");
            data += encode.set_Vertical("630") + encode.set_Horizontal("300") + encode.set_Pitch("1") + encode.gen_text_XL(sl);
            data += encode.set_Vertical("230") + encode.set_Horizontal("680") + encode.gen_qrcode(qrdata, Size: "09");
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_Thung(List<string> List_SeriNo, string MaSP, string LoSX, string NMDT, string FirmWare)
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Vertical("50") + encode.set_Horizontal("100") + encode.set_Pitch("1") + encode.gen_text_X23("Ngay:");
            data += encode.set_Vertical("50") + encode.set_Horizontal("200") + encode.set_Pitch("1") + encode.gen_text_XL(DateTime.Now.ToString("dd/MM/yyyy"));
            data += encode.set_Vertical("50") + encode.set_Horizontal("850") + encode.set_Pitch("1") + encode.gen_text_X23("NMDT:");
            data += encode.set_Vertical("50") + encode.set_Horizontal("970") + encode.set_Pitch("1") + encode.gen_text_XL(NMDT);
            data += encode.set_Vertical("50") + encode.set_Horizontal("1350") + encode.set_Pitch("1") + encode.gen_text_X23("MaSP:");
            data += encode.set_Vertical("50") + encode.set_Horizontal("1500") + encode.set_Pitch("1") + encode.gen_text_XL(MaSP);

            data += encode.set_Vertical("200") + encode.set_Horizontal("100") + encode.set_Pitch("1") + encode.gen_text_X23("MAU:");
            data += encode.set_Vertical("190") + encode.set_Horizontal("210") + encode.set_Pitch("1") + encode.gen_text_X24("[ ]");
            data += encode.set_Vertical("200") + encode.set_Horizontal("310") + encode.set_Pitch("1") + encode.gen_text_X23("TRANG");
            data += encode.set_Vertical("190") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_X24("[ ]");
            data += encode.set_Vertical("200") + encode.set_Horizontal("560") + encode.set_Pitch("1") + encode.gen_text_X23("DEN");
            data += encode.set_Vertical("190") + encode.set_Horizontal("660") + encode.set_Pitch("1") + encode.gen_text_X24("[ ]");
            data += encode.set_Vertical("200") + encode.set_Horizontal("760") + encode.set_Pitch("1") + encode.gen_text_X23("HONG");
            data += encode.set_Vertical("190") + encode.set_Horizontal("880") + encode.set_Pitch("1") + encode.gen_text_X24("[ ]");
            data += encode.set_Vertical("200") + encode.set_Horizontal("960") + encode.set_Pitch("1") + encode.gen_text_X23("XANH");
            data += encode.set_Vertical("190") + encode.set_Horizontal("1080") + encode.set_Pitch("1") + encode.gen_text_X24("[ ]");
            data += encode.set_Vertical("200") + encode.set_Horizontal("1180") + encode.set_Pitch("1") + encode.gen_text_X23("VANG");

            data += encode.set_Vertical("200") + encode.set_Horizontal("1350") + encode.set_Pitch("1") + encode.gen_text_X23("LOSX:");
            data += encode.set_Vertical("140") + encode.set_Horizontal("1480") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(LoSX, "04", "080");
            data += encode.set_Vertical("222") + encode.set_Horizontal("1500") + encode.set_Pitch("25") + encode.gen_text_X23(LoSX);

            int h1 = 150, h2 = 1100, v_barcode = 300, v_text = 470;
            for (int i = 0; i < List_SeriNo.Count; i++)
            {
                if (i % 2 != 0)
                {
                    data += encode.set_Vertical(v_barcode.ToString()) + encode.set_Horizontal(h2.ToString()) + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(List_SeriNo[i].ToString(), "03", "150");
                    data += encode.set_Vertical(v_text.ToString()) + encode.set_Horizontal(h2.ToString()) + encode.set_Pitch("1") + encode.gen_text_X24(List_SeriNo[i].ToString());
                    v_text += 300; v_barcode += 300;
                }
                else
                {
                    data += encode.set_Vertical(v_barcode.ToString()) + encode.set_Horizontal(h1.ToString()) + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(List_SeriNo[i].ToString(), "03", "150");
                    data += encode.set_Vertical(v_text.ToString()) + encode.set_Horizontal(h1.ToString()) + encode.set_Pitch("1") + encode.gen_text_X24(List_SeriNo[i].ToString());
                }
            }

            data += encode.set_Vertical("3270") + encode.set_Horizontal("100") + encode.set_Pitch("1") + encode.gen_text_X23("Firmware:");
            data += encode.set_Vertical("3270") + encode.set_Horizontal("330") + encode.set_Pitch("1") + encode.gen_text_XL(FirmWare);
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ThanhPham_ONT(string mac_imei, string seriNo, string gpon, string wps) //65x40
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Rotation("2") + encode.set_Vertical("620") + encode.set_Horizontal("740") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(mac_imei, "03", "070");
            data += encode.set_Rotation("2") + encode.set_Vertical("540") + encode.set_Horizontal("740") + encode.set_Pitch("20") + encode.gen_text_X23(mac_imei);
            data += encode.set_Rotation("2") + encode.set_Vertical("480") + encode.set_Horizontal("740") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(seriNo, "03", "070");
            data += encode.set_Rotation("2") + encode.set_Vertical("400") + encode.set_Horizontal("740") + encode.set_Pitch("20") + encode.gen_text_X23(seriNo);
            data += encode.set_Rotation("2") + encode.set_Vertical("340") + encode.set_Horizontal("740") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(gpon, "03", "070");
            data += encode.set_Rotation("2") + encode.set_Vertical("260") + encode.set_Horizontal("740") + encode.set_Pitch("20") + encode.gen_text_X23(gpon);
            data += encode.set_Rotation("2") + encode.set_Vertical("167") + encode.set_Horizontal("740") + encode.set_Pitch("20") + encode.gen_text_X23(wps);
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ThanhPham_STB(string mac_imei, string seriNo, string gpon, string wps) //52x38
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Rotation("2") + encode.set_Vertical("430") + encode.set_Horizontal("850") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(seriNo, "03", "150");
            data += encode.set_Rotation("2") + encode.set_Vertical("260") + encode.set_Horizontal("850") + encode.set_Pitch("20") + encode.gen_text_X23(seriNo);
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_ThanhPham_ADSL(string mac_imei, string seriNo, string gpon, string wps) //74x28
        {
            Encode encode = new Encode();
            string data = encode.Start;
            data += encode.set_Rotation("2") + encode.set_Vertical("240") + encode.set_Horizontal("940") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(mac_imei, "03", "070");
            data += encode.set_Rotation("2") + encode.set_Vertical("260") + encode.set_Horizontal("940") + encode.set_Pitch("20") + encode.gen_text_X23(mac_imei);
            data += encode.set_Rotation("2") + encode.set_Vertical("200") + encode.set_Horizontal("940") + encode.set_Pitch("1") + Space + encode.gen_barcode_CODE128(seriNo, "03", "070");
            data += encode.set_Rotation("2") + encode.set_Vertical("120") + encode.set_Horizontal("940") + encode.set_Pitch("20") + encode.gen_text_X23(seriNo);
            data += encode.set_Rotation("2") + encode.set_Vertical("55") + encode.set_Horizontal("940") + encode.set_Pitch("20") + encode.gen_text_X23(wps);
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

        public void Print_LabelPackage(string barcode_data, string productNo, string packageID, string productNoOld, string TransferNo, string Supplier, string Project)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            //qrdata = "[)>@06@PHY5ND35N000001@3SPKG_00072@@";

            data += encode.set_Vertical("170") + encode.set_Horizontal("20") + encode.gen_data_matrix_barcode(barcode_data);

            data += encode.set_Vertical("20") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_X23("Product No.");
            data += encode.set_Vertical("80") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_XL(productNo);
            data += encode.set_Vertical("170") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_X23("Package-ID");
            data += encode.set_Vertical("230") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_XL(packageID);
            data += encode.set_Vertical("310") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_X23("Supplier");
            data += encode.set_Vertical("370") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_XL(Supplier);
            data += encode.set_Vertical("450") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_X23("Project");
            data += encode.set_Vertical("510") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_XL(Project);
            data += encode.set_Vertical("590") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_X23("Transfer No.");
            data += encode.set_Vertical("650") + encode.set_Horizontal("460") + encode.set_Pitch("1") + encode.gen_text_XL(TransferNo);   
            data += encode.set_Quantity() + encode.End;
            Print(data);
        }

    }

    class ZEBRA
    {
        public Zebra.Printing.UsbPrinterConnector ZebraPrinter;

        public ZEBRA()
        {
            //PrinterInit();
        }

        public bool PrinterInit()
        {
            var enumDevices = Zebra.Printing.UsbPrinterConnector.EnumDevices();
            if (enumDevices.Keys.Count > 0)
            {
                string key = enumDevices.Keys[0];
                ZebraPrinter = new Zebra.Printing.UsbPrinterConnector(key);
                return true;
            }
            return false;
        }

        public void Print(string data)
        {
            byte[] cmddata = Encode.gen_command(data);
            ZebraPrinter.IsConnected = true;
            ZebraPrinter.Send(cmddata);
        }

        public class Encode
        {
            public string Start = @"^XA";
            public string End = "^XZ";
            public string FS = "^FS ";
            //Format
            private static string FO = "^FO";
            private static string FD = "^FD";
            private static string FP = "^FP";
            private static string FB = "^FB";
            private static string BY = "^BY"; // BarCode Field Default
            //Font
            private static string Text = "^A";
            private static string Barcode_39 = "^B3";
            private static string Barcode_128 = "^BC";
            private static string QRCode = "^BQ";

            public static byte[] gen_command(string command)
            {
                return ASCIIEncoding.ASCII.GetBytes(command);
            }

            public string set_FieldOrigin(string x, string y)
            {
                return FO + x + "," + y;
            }

            public string set_FieldData(string value)
            {
                return FD + value;
            }

            public string set_FieldParameter(string g, string d = "H")
            {
                //d = direction  
                //    H = horizontal printing (left to right)
                //    V = vertical printing (top to bottom)
                //    R = reverse printing (right to left)
                //g = additional inter-character gap (in dots)

                return FP + d + "," + g;
            }

            public string set_FieldBlock(string a = "0", string b = "1", string c = "0", string d = "L", string e = "0")
            {
                //Format:  ^FBa,b,c,d,e
                //a = width of text block line (in dots)
                //b = maximum number of lines in text block
                //c = add or delete space between lines (in dots)
                //d = text justification
                //    L = left
                //    C = center
                //    R = right
                //    J = justified
                //e = hanging indent (in dots) of the second and remaining lines

                return FB + a + "," + b + "," + c + "," + d + "," + e;
            }

            public string set_BarCodeField(string w, string r, string h)
            {
                //Format:  ^BYw,r,h
                //w = module width (in dots)
                //r = wide bar to narrow bar width ratio
                //h = bar code height (in dots)

                return BY + w + "," + r + "," + h;
            }

            public string gen_Text(string data, string h, string w, string f = "0")
            {
                //^Afo,h,w
                //f = font name
                //    Values:  A through Z, and 0 to 9
                //o = field orientation
                //    Values:  
                //    N = normal
                //    R = rotated 90 degrees (clockwise)
                //    I = inverted 180 degrees
                //    B = read from bottom up, 270 degrees
                //h = Character Height (in dots)
                //w = width (in dots)

                return Text + f + "," + h + "," + w + set_FieldData(data) + FS;
            }

            #region Bar Code Field Commands
            //ZPL     Command Command Description 
            //^B0     Aztec Bar Code Parameters
            //^B1     Code 11 (USD-8)
            //^B2     Interleaved 2 of 5
            //^B3     Code 39 (USD-3 and 3 of 9)
            //^B4     Code 49 (*)
            //^B5     Planet Code Bar Code
            //^B7     PDF417 (*)
            //^B8     EAN-8 (*)
            //^B9     UPC-E
            //^BA     Code 93 (USS-93)(*)
            //^BB     CODABLOCK A, E, F (*)
            //^BC     Code 128 (USD-6) (*)
            //^BD     UPS MaxiCode (*)
            //^BE     EAN-13
            //^BF     Micro-PDF417
            //^BI     Industrial 2 of 5
            //^BJ     Standard 2 of 5
            //^BK     ANSI Codabar (USD-4 and 2 of 7)
            //^BL     LOGMARS
            //^BM     MSI
            //^BO     Aztec Bar Code Parameters
            //^BP     Plessey
            //^BQ     QR Code (*)
            //^BR     GS1 Databar (formerly RSS)
            //^BS     UPC/EAN Extensions (*)
            //^BU     UPC-A (*)
            //^BX     Data Matrix (*)
            //^BZ     PostNet (*), USPS Intelligent Mail, and Planet bar codes
            #endregion

            public string gen_Barcode_39(string data, string o = "N", string e = "N", string h = "60", string f = "N", string g = "N")
            {
                //Format:  ^B3o,e,h,f,g
                //o = orientation
                //    N = normal
                //    R = rotated 90 degrees (clockwise)
                //    I = inverted 180 degrees
                //    B = read from bottom up, 270 degrees
                //e = Mod-43 check digit
                //h = bar code height (in dots)
                //f = print interpretation line
                //g = print interpretation line above code

                return Barcode_39 + o + "," + e + "," + h + "," + f + "," + g + set_FieldData(data) + FS;
            }

            public string gen_Barcode_128(string data, string o = "N", string h = "60", string f = "N", string g = "N", string e = "N", string m = "N")
            {
                //Format:   ^BCo,h,f,g,e,m
                //o = orientation
                //    N = normal
                //    R = rotated 90 degrees (clockwise)
                //    I = inverted 180 degrees
                //    B = read from bottom up, 270 degrees
                //h = bar code height (in dots)
                //f = print interpretation line
                //g = print interpretation line above code
                //e = UCC check digit
                //m = mode
                //    N = no selected mode
                //    U = UCC Case Mode
                //    • More than 19 digits in ^FD or ^SN are eliminated.
                //    • Fewer than 19 digits in ^FD or ^SN add zeros to the right to bring the count to 19. This produces an invalid interpretation line.
                //    A = Automatic Mode
                //    This analyzes the data sent and automatically determines the best packing method. The full ASCII character set can be used in the ^FD statement — the printer determines when to shift subsets. A string of four or more numeric digits causes an automatic shift to Subset C.
                //    D = UCC/EAN Mode (x.11.x and newer firmware) 
                //    This allows dealing with UCC/EAN with and without chained application identifiers. The code starts in the appropriate subset followed by FNC1 to indicate a UCC/EAN 128 bar code. The printer automatically strips out parentheses and spaces for encoding, but prints them in the human-readable section. The printer automatically determines if a check digit is required, calculate it, and print it. Automatically sizes the human readable.

                return Barcode_128 + o + "," + h + "," + f + "," + g + "," + e + "," + m + set_FieldData(data) + FS;
            }

            public string gen_QRCode(string data, string a = "N", string b = "2", string c = "6", string d = "Q", string e = "7")
            {
                //Format:  ^BQa,b,c,d,e
                //a = field orientation
                //b = model
                //c = magnification factor
                //d = error correction
                //H = ultra-high reliability level
                //Q = high reliability level
                //M = standard level 
                //L = high density level
                //e = mask value

                return QRCode + a + "," + b + "," + c + "," + d + "," + e + set_FieldData(data) + FS;
            }
        }
    }

    class ZM400 : ZEBRA
    {
        public void Print_MAC_IMEI(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            data += encode.End;
            Print(data);
        }

        public void Print_SerialNo(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            data += encode.End;
            Print(data);
        }
    }

    class Z110Xi4 : ZEBRA
    {
        public void Print_MAC_IMEI(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            data += encode.End;
            Print(data);
        }

        public void Print_SerialNo(string value)
        {
            Encode encode = new Encode();
            string data = encode.Start;

            data += encode.End;
            Print(data);
        }
    }
}
