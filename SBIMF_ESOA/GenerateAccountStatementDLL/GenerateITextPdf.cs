using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Configuration;
using System.Web;

namespace GenerateAccountStatementDLL
{
    public class GenerateITextPdf
    {
        #region Declare Private Variables
        private bool _NoBorder = false;
        private bool _NeedIcon = true;
        private DataSet _ReportData = new DataSet();
        private int _FontSize = 8;
        private int _HeaderFontSize = 20;
        private int _AlignText = 0;
        private ArrayList _ColumnWidths;
        private int _PageType = 0;
        private bool _LandScape = false;
        private float[] _PaperSize = { 0, 0 };
        private PdfWriter _oPDFWriter;
        private string _ReportName = "PDF" + DateTime.Now.Ticks.ToString() + ".pdf";
        private int _TableGap = 1;
        private Hashtable _TableSizes = new Hashtable();
        private Hashtable _Totals = new Hashtable();
        private Hashtable _RunningTotal = new Hashtable();
        private Hashtable _Formatter = new Hashtable();
        private bool _ChildHeader = false;
        //private string _ReportPath = ConfigurationManager.AppSettings["ReportPathText"];
        private string _ReportPath = ""; //ConfigurationManager.AppSettings["ReportPathText"];
        private string _ReportSave = ""; //ConfigurationManager.AppSettings["ReportSaveText"];
        private bool _HighlightNegative = true;
        private string _Winpath = System.Environment.GetEnvironmentVariable("SystemRoot");
        #endregion

        public GenerateITextPdf(string ReportPath, string ReportSave)
        {
            _ReportPath = ReportPath;
            _ReportSave = ReportSave;
        }


        #region Public variables

        public bool HighlightNegative
        {
            get
            {
                return _HighlightNegative;
            }
            set
            {
                _HighlightNegative = value;
            }
        }

        public Hashtable Formatter
        {
            get
            {
                return _Formatter;
            }
            set
            {
                _Formatter = value;
            }
        }

        /// <summary>
        /// 0-A4,1-Legal,2-Letter,3-Ledger,4-User Defined (will require PaperSize to be set)
        /// </summary>
        public int PageType
        {
            get
            {
                return _PageType;
            }
            set
            {
                _PageType = value;
            }
        }

        public Hashtable Totals
        {
            get
            {
                return _Totals;
            }
            set
            {
                _Totals = value;
            }
        }

        public string ReportPath
        {
            get
            {
                return _ReportPath;
            }
            set
            {
                _ReportPath = value;
            }
        }

        public bool ChildHeader
        {
            set
            {
                _ChildHeader = value;
            }
            get
            {
                return _ChildHeader;
            }
        }

        /// <summary>
        /// Key = TableName,Value = float array
        /// ex. "Table1",float {10,10,50,20}
        /// </summary>
        public Hashtable TableSizes
        {
            set
            {
                _TableSizes = value;
            }
        }

        public bool NoBorder
        {
            get
            {
                return _NoBorder;
            }
            set
            {
                _NoBorder = value;
            }
        }

        public bool NeedIcon
        {
            get
            {
                return _NeedIcon;
            }
            set
            {
                _NeedIcon = value;
            }
        }

        public int FontSize
        {
            get
            {
                return _FontSize;
            }
            set
            {
                _FontSize = value;
            }
        }

        public int HeaderFontSize
        {
            get
            {
                return _HeaderFontSize;
            }
            set
            {
                _HeaderFontSize = value;
            }
        }

        public int AlignText
        {
            get
            {
                return _AlignText;
            }
            set
            {
                _AlignText = value;
            }
        }

        public ArrayList ColumnWidths
        {
            get
            {
                return _ColumnWidths;
            }
            set
            {
                _ColumnWidths = value;
            }
        }

        public bool LandScape
        {
            get
            {
                return _LandScape;
            }
            set
            {
                _LandScape = value;
            }
        }

        public float[] PaperSize
        {
            get
            {
                return _PaperSize;
            }
            set
            {
                _PaperSize = value;
            }
        }

        public string ReportName
        {
            get
            {
                //return _ReportPath + _ReportName;
                return _ReportName;
            }
            set
            {
                _ReportName = value + DateTime.Now.Ticks.ToString() + ".pdf";
            }
        }

        #endregion

        #region Create Cell Content

        private PdfPCell CreateContent(string sContent, Font oFont, bool Highlight)
        {
            PdfPCell oCell;

            try
            {
                Phrase oPhrase = new Phrase(sContent, oFont);
                oPhrase.Font.Size = _FontSize;
                oCell = new PdfPCell(oPhrase);
                return oCell;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PdfPCell CreateContent(string sContent, bool Highlight)
        {
            BaseFont arial = BaseFont.CreateFont(_Winpath + "\\fonts\\arial.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED);
            // Font oFont = new Font(arial, 12, Font.NORMAL);
            Font oFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9f, Font.BOLD);
            if (Highlight)
            {
                oFont.Color = BaseColor.RED;
            }
            return this.CreateContent(sContent, oFont, Highlight);
        }

        private PdfPCell CreateContent(string sContent)
        {
            return this.CreateContent(sContent, false);
        }


        #endregion

        #region Create Cell ContentNew

        private PdfPCell CreateContentSmall(string sContent, Font oFont, bool Highlight)
        {
            PdfPCell oCell;

            try
            {
                Phrase oPhrase = new Phrase(sContent, oFont);
                oPhrase.Font.Size = _FontSize;
                oCell = new PdfPCell(oPhrase);
                return oCell;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PdfPCell CreateContentSmall(string sContent, bool Highlight)
        {
            BaseFont arial = BaseFont.CreateFont(_Winpath + "\\fonts\\arial.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED);
            // Font oFont = new Font(arial, 12, Font.NORMAL);
            Font oFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9f);
            if (Highlight)
            {
                oFont.Color = BaseColor.RED;
            }
            return this.CreateContent(sContent, oFont, Highlight);
        }

        private PdfPCell CreateContentSmall(string sContent)
        {
            return this.CreateContentSmall(sContent, false);
        }


        #endregion

        #region Simply Creates a PDF table

        private PdfPTable BuildHead(DataTable oDT)
        {

            PdfPTable oTable = new PdfPTable(oDT.Columns.Count);
            string[] aRelations;
            if (oDT.ChildRelations.Count > 0)
            {
                aRelations = this.BuildNestedStructure(oDT.ChildRelations[0]);
            }
            else
            {
                aRelations = new string[0];
            }

            float[] HeaderWidths = this.GetColSize(oDT);
            oTable.WidthPercentage = 100;
            oTable.DefaultCell.Padding = 3;
            oTable.SetWidths(HeaderWidths);

            foreach (DataColumn oDC in oDT.Columns)
            {

                //Put new function for creating the headers
                if (aRelations.Length > 0 && oDC.ColumnName.ToUpper().Trim() == aRelations[4].ToString().ToUpper().Trim())
                {
                    DataTable onDT = this._ReportData.Tables[aRelations[2].ToString()];
                    PdfPTable oCTable = this.BuildHead(onDT);
                    PdfPCell oCell = new PdfPCell(oCTable);
                    oTable.AddCell(oCell);
                }
                else
                {

                    PdfPCell oCell = this.CreateContent(oDC.ColumnName.Trim());
                    //  oCell.BackgroundColor = new Color(51, 153, 153);
                    oCell.BackgroundColor = BaseColor.YELLOW;
                    if (this._NoBorder)
                    {
                        oCell.BorderWidth = 0;
                    }
                    oTable.AddCell(oCell);
                }

            }

            return oTable;
        }

        [Description("Simply returns a PDFTable Object"), Category("User Properties")]
        private PdfPTable BuildTable(DataTable oDT, bool NeedHeader)
        {
            DataView oDV = new DataView(oDT);
            return this.BuildTable(oDV, NeedHeader);
        }


        [Description("Simply returns a PDFTable Object"), Category("User Properties")]
        private PdfPTable BuildTable(DataView oDV, bool NeedHeader)
        {
            try
            {

                DataTable oDT = oDV.Table;
                float[] HeaderWidths = this.GetColSize(oDT);
                string[] aRelations;
                string ColType = "";
                string ColName = "";
                if (oDT.ChildRelations.Count > 0)
                {
                    aRelations = this.BuildNestedStructure(oDT.ChildRelations[0]);
                }
                else
                {
                    aRelations = new string[0];
                }
                PdfPTable oTable = new PdfPTable(oDT.Columns.Count);
                // oTable.SetTotalWidth(new float[] { 100, 75, 75, 75, 75 });
                oTable.WidthPercentage = 100;
                oTable.DefaultCell.Padding = 3;
                oTable.SetWidths(HeaderWidths);
                string sPrintVal = "";

                // Knock of the header if not required
                if (NeedHeader)
                {
                    foreach (DataColumn oDC in oDT.Columns)
                    {
                        //Put new function for creating the headers
                        if (aRelations.Length > 0 && oDC.ColumnName.ToUpper().Trim() == aRelations[3].ToString().ToUpper().Trim())
                        {
                            PdfPCell oCell = new PdfPCell(this.BuildHead(this._ReportData.Tables[aRelations[2].ToString()]));
                            //  oCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            oTable.AddCell(oCell);
                        }
                        else
                        {
                            PdfPCell oCell = this.CreateContent(oDC.ColumnName.Trim());
                            // oCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            // oCell.BackgroundColor = new Color(51, 153, 153);
                            // oCell.BackgroundColor = BaseColor.PINK;
                            if (this._NoBorder)
                            {
                                oCell.BorderWidth = 0;
                            }
                            oTable.AddCell(oCell);
                        }

                    }
                    oTable.HeaderRows = 1;  // this is the end of the table header
                }

                for (int i = 0; i < oDV.Count; i++)
                //foreach(DataRow oDR in oDV.Table.Rows)
                {
                    if (i % 2 == 1)
                    {
                        // Alternating colors
                        //oTable.DefaultCell.GrayFill = 0.9f;
                    }

                    //foreach(DataColumn oDC in oDV.Table.Columns)
                    for (int j = 0; j < oDV.Table.Columns.Count; j++)
                    {
                        ColType = oDV.Table.Columns[j].DataType.FullName.Replace("System.", "");
                        ColName = oDV.Table.Columns[j].ColumnName.Trim();

                        if (aRelations.Length > 0 && ColName.ToUpper() == aRelations[3].ToString().ToUpper().Trim())
                        {
                            DataTable onDT = this._ReportData.Tables[aRelations[2].ToString()];
                            DataView oDTV = new DataView(onDT);
                            oDTV.RowFilter = aRelations[4].ToString() + "= '" + oDV[i][j].ToString() + "'";
                            PdfPTable oCTable = this.BuildTable(oDTV, this._ChildHeader);
                            PdfPCell oCell = new PdfPCell(oCTable);
                            //  oCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            oTable.AddCell(oCell);
                        }
                        else
                        {
                            sPrintVal = oDV[i][j].ToString();
                            PdfPCell oCell = this.DataFormatter(ColType, ColName, sPrintVal);
                            // oCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            if (this._NoBorder)
                            {
                                oCell.Border = 0;
                            }
                            oTable.AddCell(oCell);
                        }

                    }
                    if (i % 2 == 1)
                    {
                        // oTable.DefaultCell.GrayFill = 0.0f;
                    }
                }

                //Need to Print Totals here

                return this.PrintTotals(oTable, oDT);
            }
            catch (Exception ex)
            {
                throw (ex);

            }
        }

        #endregion

        #region Printtotal columns
        private PdfPTable PrintTotals(PdfPTable oTable, DataTable oDT)
        {
            PdfContentByte cb = _oPDFWriter.DirectContent;
            try
            {
                bool NeedTotal = false;
                string sPrintValue = "";
                string ColType = "";

                foreach (DataColumn oDC in oDT.Columns)
                {
                    if (_Totals.ContainsKey(oDC.ColumnName))
                    {
                        NeedTotal = true;
                    }
                }
                if (NeedTotal)
                {
                    foreach (DataColumn oDC in oDT.Columns)
                    {
                        if (_Totals.ContainsKey(oDC.ColumnName))
                        {
                            sPrintValue = _Totals[oDC.ColumnName].ToString();
                            ColType = "Double";
                        }
                        else
                        {
                            sPrintValue = "";
                            ColType = "String";//oDC.DataType.FullName.Replace("System.", "");
                        }
                        PdfPCell oCell = this.DataFormatter(ColType, oDC.ColumnName, sPrintValue, true);
                        oCell.BackgroundColor = BaseColor.BLUE;
                        oTable.AddCell(oCell);
                    }
                }
                // oTable.WriteSelectedRows(0, -1, 0, 550, cb);
                // oTable.WriteSelectedRows(0, -1, 0, pageSize.GetTop(50), cb);
                return oTable;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        #region Column sizes
        private float[] FindColSize(DataTable oDT)
        {
            float[] aCols = new float[oDT.Columns.Count];
            string ColType = "";
            int i = 0;
            int iCtr = 0;
            string[] aRelations;
            try
            {
                if (oDT.ChildRelations.Count > 0)
                {
                    aRelations = this.BuildNestedStructure(oDT.ChildRelations[0]);
                }
                else
                {
                    aRelations = new string[0];
                }

                foreach (DataColumn oDC in oDT.Columns)
                {
                    if (oDT.ChildRelations.Count > 0 && oDC.ColumnName.ToUpper().Trim() == aRelations[4].ToString().ToUpper().Trim())
                    {
                        ColType = "Child";
                    }
                    else
                    {
                        ColType = oDC.DataType.FullName.Replace("System.", "");
                    }

                    switch (ColType)
                    {
                        case "Child":
                            i = oDT.Columns.Count > 5 ? 40 : 60;
                            //i = 40;
                            break;
                        case "Int16":
                            i = 10;
                            break;
                        case "Int32":
                            i = 10;
                            break;
                        case "Int64":
                            i = 10;
                            break;
                        case "Decimal":
                            i = 15;
                            break;
                        case "Double":
                            i = 15;
                            break;
                        case "Byte":
                            //i = 3;
                            i = 10;
                            break;
                        case "Boolean":
                            i = 5;
                            break;
                        case "DateTime":
                            i = 15;
                            break;
                        case "Guid":
                            i = 15;
                            break;
                        case "String":
                            i = 35;
                            break;
                        default:
                            i = 15;
                            break;
                    }
                    aCols[iCtr++] = i;
                }
            }


            catch (Exception ex)
            {
                throw (ex);
            }
            return aCols;
        }

        private float[] GetColSize(DataTable oDT)
        {
            float[] aCols;
            string sTableName = oDT.TableName;
            if (_TableSizes.ContainsKey(sTableName))
            {
                aCols = (float[])_TableSizes[sTableName];
            }
            else
            {
                aCols = this.FindColSize(oDT);
            }

            return aCols;
        }
        #endregion

        #region Print The PDF Report
        /// <summary>
        /// This function prints the report depending on the dataset 
        /// passed and the printsequence set
        /// </summary>
        [Description("Call Print Report for normal table report"), Category("User Properties")]
        public byte[] PrintPDFReport(DataSet ds)
        {
            string barcode = "";
            string userid = "";
            string stmtdate = "";
            string folio = "";
            string address1 = "";
            string address2 = "";
            string address3 = "";
            string address4 = "";
            string address5 = "";
            string name = "";
            string jointholder = "";
            string jointholder2 = "";
            string holding = "";
            string taxstatcode = "";
            string mobile_no = "";
            string email = "";
            string guardian = "";
            string nominee = "";
            string panno = "";
            string panno1 = "";
            string panno2 = "";
            string phoneres = "";
            string kycstatus = "";
            string EUIN = "";
            string DateFrom = "";
            string DateTo = "";
            string Tot_Inv_Amt = "";
            string Tot_Port_Val = "";
            string Tot_GL = "";
            string GL = "";

            if (ds.Tables.Contains("Statement_Period"))
            {
                DateFrom = ds.Tables["Statement_Period"].Rows[0]["DateFrom"].ToString();
                DateTo = ds.Tables["Statement_Period"].Rows[0]["DateTo"].ToString();
            }

            if (ds.Tables.Contains("Overall_Inv_Summary"))
            {
                Tot_Inv_Amt = ds.Tables["Overall_Inv_Summary"].Rows[0]["Tot_Inv_Amt"].ToString();
                Tot_Port_Val = ds.Tables["Overall_Inv_Summary"].Rows[0]["Tot_Port_Val"].ToString();
                Tot_GL = ds.Tables["Overall_Inv_Summary"].Rows[0]["Tot_GL"].ToString();
                GL = ds.Tables["Overall_Inv_Summary"].Rows[0]["GL"].ToString();
            }

            if (ds.Tables.Contains("stat_general"))
            {
                userid = ds.Tables["stat_general"].Rows[0]["userid"].ToString();
                stmtdate = ds.Tables["stat_general"].Rows[0]["stmtdate"].ToString();
            }
            if (ds.Tables.Contains("in_address"))
            {
                foreach (DataRow dr in ds.Tables["in_address"].Rows)
                {
                    barcode = Convert.ToString(dr["barcode"]);
                    folio = Convert.ToString(dr["no"]);
                    address1 = Convert.ToString(dr["addr1"]);
                    address2 = Convert.ToString(dr["addr2"]);
                    address3 = Convert.ToString(dr["addr3"]);
                    address4 = Convert.ToString(dr["addr4"]);
                    address5 = Convert.ToString(dr["addr5"]);
                    name = Convert.ToString(dr["name"]);
                    jointholder = ((Convert.ToString(dr["jname1"])) == "" ? "NA" : (Convert.ToString(dr["jname1"]))); //Convert.ToString(dr["jname1"]);
                    jointholder2 = ((Convert.ToString(dr["jname2"])) == "" ? "NA" : (Convert.ToString(dr["jname2"]))); // Convert.ToString(dr["jname2"]);
                    holding = Convert.ToString(dr["holding"]);
                    taxstatcode = Convert.ToString(dr["status"]);
                    mobile_no = Convert.ToString(dr["mobile_no"]);
                    email = Convert.ToString(dr["email"]).Replace("Email : ", "");
                    guardian = ((Convert.ToString(dr["guardian"])) == "" ? "NA" : (Convert.ToString(dr["guardian"]))); // Convert.ToString(dr["guardian"]);
                    nominee = Convert.ToString(dr["nominee"]).Replace("Nominee : ", "");
                    panno = Convert.ToString(dr["panno"]);
                    panno1 = ((Convert.ToString(dr["panno1"])) == "" ? "NA" : (Convert.ToString(dr["panno1"]))); // Convert.ToString(dr["panno1"]);
                    panno2 = ((Convert.ToString(dr["panno2"])) == "" ? "NA" : (Convert.ToString(dr["panno2"]))); // Convert.ToString(dr["panno2"]);
                    phoneres = Convert.ToString(dr["phoneres"]);
                    EUIN = ((Convert.ToString(dr["Last_EUIN"])) == "" ? "NA" : (Convert.ToString(dr["Last_EUIN"])));
                    kycstatus = (Convert.ToString(dr["Investor_KYCStatus"]) == "3") ? "Ok" : "Not ok";
                }
            }

            _ReportData = ds;
            byte[] Result = new byte[0];

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Document oPDF = this.CreateReportFormatPDF();
            _oPDFWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(oPDF, ms);
            _oPDFWriter.PageEvent = new PDFFooter();
            //Font content = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 6.5f);
            //Font contentBold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 6.5f, Font.BOLD);
            //Font contentBig = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10f);
            //Font contentBigBold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10f, Font.BOLD);
            //Font contentBig9 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9f);
            //Font contentBigBold9 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9f, Font.BOLD);
            //Font Headings = FontFactory.GetFont("Arial", 9f, Font.BOLD, BaseColor.WHITE);
            //Font Headings1 = FontFactory.GetFont("Arial", 12f, Font.BOLD, BaseColor.BLACK);
            //Font contentBigRed = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10f,new BaseColor(82,237, 73));
            //Font contentBigGreen = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10f, new BaseColor(75, 181, 67));

            Font content = FontFactory.GetFont(BaseFont.HELVETICA, 6.5f);
            Font contentBold = FontFactory.GetFont(BaseFont.HELVETICA, 6.5f, Font.BOLD);
            Font contentBig = FontFactory.GetFont(BaseFont.HELVETICA, 10f);
            Font contentBigBold = FontFactory.GetFont(BaseFont.HELVETICA, 10f, Font.BOLD);
            Font contentBig9 = FontFactory.GetFont(BaseFont.HELVETICA, 9f);
            Font contentBigBold9 = FontFactory.GetFont(BaseFont.HELVETICA, 9f, Font.BOLD);
            Font Headings = FontFactory.GetFont("Arial", 9f, Font.BOLD, BaseColor.WHITE);
            Font Headings1 = FontFactory.GetFont("Arial", 12f, Font.BOLD, BaseColor.BLACK);
            Font contentBigRed = FontFactory.GetFont(BaseFont.HELVETICA, 10f, new BaseColor(82, 237, 73));
            Font contentBigGreen = FontFactory.GetFont(BaseFont.HELVETICA, 10f, new BaseColor(75, 181, 67));

            oPDF.Open();




            PdfPCell _cell;

            if (ds.Tables.Contains("scheme"))
            {
                Phrase phrase = null;
                PdfPTable RegInfo = new PdfPTable(2);
                RegInfo.WidthPercentage = 100f;


                _cell = new PdfPCell(new Phrase(barcode, contentBig));
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthTop = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                //Barcode128 code128 = new Barcode128();
                //code128.CodeType = Barcode.UPCA;
                //code128.Code = barcode;
                //// Generate barcode image
                //iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(_oPDFWriter.DirectContent, null, null);
                //RegInfo.AddCell(image128);


                phrase = new Phrase();
                phrase.Add(new Chunk("Mode of Holding   :   ", contentBigBold));
                phrase.Add(new Chunk(holding, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthTop = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(name, contentBigBold));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Tax Status              :   ", contentBigBold));
                phrase.Add(new Chunk(taxstatcode, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);



                _cell = new PdfPCell(new Phrase(" ", contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Nominee                :   ", contentBigBold));
                phrase.Add(new Chunk(nominee, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(address1, contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);


                phrase = new Phrase();
                phrase.Add(new Chunk("Second Holder      :   ", contentBigBold));
                phrase.Add(new Chunk(jointholder, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(address2, contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);


                phrase = new Phrase();
                phrase.Add(new Chunk("Third Holder          :   ", contentBigBold));
                phrase.Add(new Chunk(jointholder2, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(address3, contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Guardian Name     :   ", contentBigBold));
                phrase.Add(new Chunk(guardian, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 9.1f;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(address4, contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase("  ", contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                _cell.BorderWidthBottom = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(address5, contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("PAN 1                     :   ", contentBigBold));
                phrase.Add(new Chunk(panno, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Tel-Res        :   ", contentBigBold));
                phrase.Add(new Chunk(phoneres, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                if (panno1.Contains("Please Provide") == false)
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("PAN 2                     :   ", contentBigBold));
                    phrase.Add(new Chunk(panno1, contentBig));
                    _cell = new PdfPCell(phrase);
                }
                else
                {
                    _cell = new PdfPCell(new Phrase(" ", contentBig));
                }
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Mobile No   :   ", contentBigBold));
                phrase.Add(new Chunk(mobile_no, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);


                if (panno2.Contains("Please Provide") == false)
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("PAN 3                     :   ", contentBigBold));
                    phrase.Add(new Chunk(panno2, contentBig));
                    _cell = new PdfPCell(phrase);
                }
                else
                {
                    _cell = new PdfPCell(new Phrase(" ", contentBig));
                }
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Email           :   ", contentBigBold));
                phrase.Add(new Chunk(email, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Statement Period   :   ", contentBigBold));
                phrase.Add(new Chunk(DateFrom, contentBig));
                phrase.Add(new Chunk(" To ", contentBigBold));
                phrase.Add(new Chunk(DateTo, contentBig));
                _cell = new PdfPCell(phrase);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.PaddingLeft = 10;
                _cell.PaddingBottom = 4;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(" ", contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.BorderWidthLeft = 1f;
                //_cell.BorderWidthRight = 1f;
                _cell.BorderWidthBottom = 1f;
                RegInfo.AddCell(_cell);

                _cell = new PdfPCell(new Phrase(" ", contentBig));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                _cell.BorderWidthLeft = 1f;
                _cell.BorderWidthRight = 1f;
                _cell.BorderWidthBottom = 1f;
                RegInfo.AddCell(_cell);

                oPDF.Add(RegInfo);

                foreach (DataRow dr in ds.Tables["scheme"].Rows)
                {
                    //oPDF.NewPage();

                    PdfPTable Foliotab = new PdfPTable(3);
                    Foliotab.WidthPercentage = 100f;

                    _cell = new PdfPCell(new Phrase(" ", contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 3;
                    Foliotab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Folio Number : ", contentBigBold));
                    phrase.Add(new Chunk(folio, contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.VerticalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    Foliotab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("ISIN : ", contentBigBold));
                    phrase.Add(new Chunk(Convert.ToString(dr["ISIN"]), contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.VerticalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    Foliotab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Statement Date : ", contentBigBold));
                    phrase.Add(new Chunk(DateTo, contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _cell.VerticalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    Foliotab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(" ", contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 3;
                    Foliotab.AddCell(_cell);

                    PdfPTable InvSummarytab = new PdfPTable(3);
                    InvSummarytab.WidthPercentage = 100f;

                    _cell = new PdfPCell(new Phrase("Overall Investment Summary", contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.Colspan = 3;
                    _cell.BorderWidthTop = 1f;
                    _cell.BorderWidthBottom = 1f;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.PaddingBottom = 4;
                    InvSummarytab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Investment Value : ", contentBigBold));
                    phrase.Add(new Chunk(Tot_Inv_Amt, contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.VerticalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    //_cell.BorderWidthTop = 1f;
                    _cell.BorderWidthBottom = 1f;
                    _cell.BorderWidthLeft = 1f;
                    //_cell.BorderWidthRight = 1f;
                    _cell.PaddingBottom = 4;
                    InvSummarytab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Portfolio Value : ", contentBigBold));
                    phrase.Add(new Chunk(Tot_Port_Val, contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.VerticalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.BorderWidthBottom = 1f;
                    _cell.PaddingBottom = 4;
                    InvSummarytab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Gain/Loss : ", contentBigBold));
                    phrase.Add(new Chunk(Tot_GL + " % ", contentBig));
                    //if (GL == "L") { phrase.Add(new Chunk(Tot_GL + " % ", contentBigRed)); }
                    //else if (GL == "G") { phrase.Add(new Chunk(Tot_GL + " % ", contentBigGreen)); }
                    //else { phrase.Add(new Chunk(Tot_GL + " % ", contentBig)); }

                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _cell.VerticalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    //_cell.BorderWidthTop = 1f;
                    _cell.BorderWidthBottom = 1f;
                    //_cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.PaddingBottom = 4;
                    InvSummarytab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(" ", contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 3;
                    InvSummarytab.AddCell(_cell);

                    PdfPTable SchemeNameTab = new PdfPTable(2);
                    SchemeNameTab.WidthPercentage = 100f;

                    _cell = new PdfPCell(new Phrase(Convert.ToString(dr["Lname"]), Headings));
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.BackgroundColor = new BaseColor(15, 94, 150);
                    SchemeNameTab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(dr["Nav"]), Headings));
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _cell.Border = 0;
                    _cell.BackgroundColor = new BaseColor(15, 94, 150);
                    SchemeNameTab.AddCell(_cell);

                    PdfPTable SchemeColumntab = new PdfPTable(7);
                    SchemeColumntab.SetWidths(new float[] { 1, 3, 1, 1.5f, 1.5f, 1.5f, 1 });

                    _cell = new PdfPCell(new Phrase("Opening Balance ", contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Colspan = 6;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(dr["Opbal"]), contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);


                    SchemeColumntab.WidthPercentage = 100f;
                    _cell = new PdfPCell(new Phrase("Date", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase("Transaction Type", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase("Amount", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase("NAV in INR ( Rs.)", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase("Price in INR ( Rs.)", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase("Number of Units", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase("Balance Unit", contentBigBold9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    SchemeColumntab.AddCell(_cell);



                    if (ds.Tables.Contains("trans"))
                    {
                        DataRow[] result = ds.Tables["trans"].Select("code = '" + Convert.ToString(dr["code"]) + "'");
                        foreach (DataRow row in result)
                        {
                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["date"]), contentBig));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);

                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["type"]), contentBig9));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);

                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["amount"]), contentBig));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);

                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["nav"]), contentBig));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);

                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["price"]), contentBig));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);

                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["n_units"]), contentBig));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);

                            _cell = new PdfPCell(new Phrase(Convert.ToString(row["b_units"]), contentBig));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _cell.Border = 0;
                            _cell.PaddingBottom = 4;
                            _cell.BorderWidthLeft = 1f;
                            _cell.BorderWidthRight = 1f;
                            _cell.BorderWidthBottom = 1f;
                            SchemeColumntab.AddCell(_cell);


                        }
                    }

                    PdfPTable Summarytab = new PdfPTable(2);
                    Summarytab.WidthPercentage = 100f;

                    _cell = new PdfPCell(new Phrase(" ", content));
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 2;
                    Summarytab.AddCell(_cell);

                    if (Convert.ToString(dr["reinvtag"]) == "")
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("Your Broker                              :   ", contentBigBold));
                        phrase.Add(new Chunk(((Convert.ToString(dr["brkname"])) == "" ? "NA" : (Convert.ToString(dr["brkname"]))), contentBig));
                        _cell = new PdfPCell(phrase);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        _cell.Colspan = 2;
                        _cell.BorderWidthTop = 1f;
                        _cell.BorderWidthLeft = 1f;
                        Summarytab.AddCell(_cell);
                    }
                    else
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("Your Broker                              :   ", contentBigBold));
                        phrase.Add(new Chunk(((Convert.ToString(dr["brkname"])) == "" ? "NA" : (Convert.ToString(dr["brkname"]))), contentBig));
                        _cell = new PdfPCell(phrase);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        _cell.BorderWidthTop = 1f;
                        _cell.BorderWidthLeft = 1f;
                        Summarytab.AddCell(_cell);

                        _cell = new PdfPCell(new Phrase(Convert.ToString(dr["reinvtag"]), contentBig));
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        _cell.BorderWidthTop = 1f;
                        _cell.BorderWidthRight = 1f;
                        Summarytab.AddCell(_cell);
                    }

                    phrase = new Phrase();
                    phrase.Add(new Chunk("EUIN                                          :   ", contentBigBold));
                    phrase.Add(new Chunk(EUIN, contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 2;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthLeft = 1f;
                    Summarytab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Total Lien Units                        :   ", contentBigBold));
                    phrase.Add(new Chunk(Convert.ToString(dr["lien_unit"]), contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 2;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthLeft = 1f;
                    Summarytab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Bank Details                              :   ", contentBigBold));
                    phrase.Add(new Chunk(Convert.ToString(dr["bankacct"]), contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 2;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthLeft = 1f;
                    Summarytab.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("KYC Complied Status              :   ", contentBigBold));
                    phrase.Add(new Chunk(kycstatus, contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.Colspan = 2;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthLeft = 1f;
                    Summarytab.AddCell(_cell);

                    PdfPTable Summarytab1 = new PdfPTable(2);
                    Summarytab1.WidthPercentage = 100f;
                    Summarytab1.SetWidths(new float[] { 1, 3 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Current Load Structure        :   ", contentBigBold));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    //_cell.Width = 200f;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthBottom = 1f;
                    _cell.PaddingBottom = 4;
                    Summarytab1.AddCell(_cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(Convert.ToString(dr["schremark"]), contentBig));
                    _cell = new PdfPCell(phrase);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    _cell.PaddingBottom = 4;
                    Summarytab1.AddCell(_cell);

                    PdfPTable TB_underline = new PdfPTable(1);
                    TB_underline.WidthPercentage = 100f;

                    _cell = new PdfPCell(new Phrase(" ", contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.BorderWidthBottom = 1f;
                    TB_underline.AddCell(_cell);



                    oPDF.Add(Foliotab);
                    oPDF.Add(InvSummarytab);
                    oPDF.Add(SchemeNameTab);
                    oPDF.Add(SchemeColumntab);
                    oPDF.Add(Summarytab);
                    oPDF.Add(Summarytab1);
                    oPDF.Add(TB_underline);


                }
            }

            // Code to generate portfolio summary
            //oPDF.NewPage();

            #region HEADER FOR SUMMERY PAGE COMMENTED
            //PdfPTable AccountSummarytab = new PdfPTable(1);
            //AccountSummarytab.WidthPercentage = 100f;
            //_cell = new PdfPCell(new Phrase("STATEMENT OF ACCOUNT", Headings1));
            //_cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_cell.Border = 0;
            //AccountSummarytab.AddCell(_cell);

            //PdfPTable RegSummaryInfo = new PdfPTable(2);

            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //RegSummaryInfo.AddCell(_cell);



            //RegSummaryInfo.WidthPercentage = 100f;
            //_cell = new PdfPCell(new Phrase(barcode, contentBig));
            //_cell.Border = 0;
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthTop = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Mode of Holding : " + holding, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthTop = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(userid, contentBigBold));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Tax Status : " + taxstatcode, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);



            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(nominee, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(address1, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Second Holder : " + jointholder, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(address2, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Third Holder : " + jointholder2, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(address3, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Guardian Name : " + guardian, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(address4, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("  ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(address5, contentBig));
            //// _cell.VerticalAlignment = Element.ALIGN_LEFT;
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //_cell.BorderWidthBottom = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Tel-Res : " + phoneres, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("PAN 1 : " + panno, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Mobile No : " + mobile_no, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Email : " + email, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase("Statement Period : " + DateFrom + " To " + DateTo, contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //_cell.BorderWidthBottom = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //_cell = new PdfPCell(new Phrase(" ", contentBig));
            //_cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_cell.Border = 0;
            //_cell.BorderWidthLeft = 1f;
            //_cell.BorderWidthRight = 1f;
            //_cell.BorderWidthBottom = 1f;
            //RegSummaryInfo.AddCell(_cell);

            //oPDF.Add(AccountSummarytab);
            //oPDF.Add(RegSummaryInfo); 
            #endregion


            //Summary starts
            PdfPTable FolioSummTab = new PdfPTable(1);
            FolioSummTab.WidthPercentage = 100f;

            _cell = new PdfPCell(new Phrase(" ", Headings1));
            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
            _cell.Border = 0;
            FolioSummTab.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("Portfolio Summary", Headings1));
            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
            _cell.Border = 0;
            FolioSummTab.AddCell(_cell);

            _cell = new PdfPCell(new Phrase(" ", Headings1));
            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
            _cell.Border = 0;
            _cell.BorderWidthBottom = 1f;
            FolioSummTab.AddCell(_cell);

            PdfPTable RegSummInfo = new PdfPTable(7);
            RegSummInfo.WidthPercentage = 100f;
            RegSummInfo.SetWidths(new float[] { 2, 1, 1, 1, 1, 1.5f, 1 });

            _cell = new PdfPCell(new Phrase("Scheme", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthLeft = 1f;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("Unit Balance", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("NAV as on", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthLeft = 1f;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("NAV", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("Current Value", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("Cost of Investment", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            _cell = new PdfPCell(new Phrase("Dividend Earn", contentBigBold9));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            _cell.PaddingBottom = 4;
            _cell.BorderWidthRight = 1f;
            _cell.BorderWidthBottom = 1f;
            RegSummInfo.AddCell(_cell);

            if (ds.Tables.Contains("trans1"))
            {
                foreach (DataRow row in ds.Tables["trans1"].Rows)
                {
                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["code"]), contentBig9));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["closbal"]), contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["navasondt"]), contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["nav"]), contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    string invamount = (row["assets"].ToString() == "") ? "0.00" : row["assets"].ToString();
                    _cell = new PdfPCell(new Phrase(invamount, contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["costvalue"]), contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["divearn"]), contentBig));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                }
            }
            if (ds.Tables.Contains("summarytotal"))
            {
                foreach (DataRow row in ds.Tables["summarytotal"].Rows)
                {
                    _cell = new PdfPCell(new Phrase("TOTAL : ", contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthLeft = 1f;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["totunits"]), contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(" ", contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(" ", contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["totassets"]), contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["totcostvalue"]), contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);

                    _cell = new PdfPCell(new Phrase(Convert.ToString(row["totdivearn"]), contentBigBold));
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    _cell.PaddingBottom = 4;
                    _cell.BorderWidthRight = 1f;
                    _cell.BorderWidthBottom = 1f;
                    RegSummInfo.AddCell(_cell);
                }
            }


            oPDF.Add(FolioSummTab);
            oPDF.Add(RegSummInfo);

            oPDF.Close();
            Result = ms.ToArray();

            return Result;
        }

        #endregion

        #region Document Creator
        private Document CreateReportFormatPDF()
        {

            string ReportPathName = this._ReportPath + "\\" + this._ReportName;
            Document oDocument;
            switch (_PageType)
            {
                case 0:
                    if (_LandScape)
                    {
                        oDocument = new Document(PageSize.A4.Rotate());
                    }
                    else
                    {
                        oDocument = new Document(PageSize.A4, 10f, 10f, 50f, 50f);
                    }
                    break;
                case 1:
                    if (_LandScape)
                    {
                        oDocument = new Document(PageSize.LEGAL.Rotate());
                    }
                    else
                    {
                        oDocument = new Document(PageSize.LEGAL);
                    }
                    break;
                case 2:
                    if (_LandScape)
                    {
                        oDocument = new Document(PageSize.LETTER.Rotate());
                    }
                    else
                    {
                        oDocument = new Document(PageSize.LETTER);
                    }
                    break;
                case 3:
                    if (_LandScape)
                    {
                        oDocument = new Document(PageSize.LEDGER);
                    }
                    else
                    {
                        oDocument = new Document(PageSize.LEDGER);
                    }
                    break;
                case 4:
                    if (_PaperSize[0] < 1 || _PaperSize[1] < 1)
                    {
                        oDocument = new Document(PageSize.A4);
                    }
                    else
                    {
                        Rectangle oPageSize = new Rectangle(_PaperSize[0], _PaperSize[1]);
                        oDocument = new Document(oPageSize);
                    }
                    break;
                default:
                    if (_LandScape)
                    {
                        oDocument = new Document(PageSize.A4.Rotate());
                    }
                    else
                    {
                        oDocument = new Document(PageSize.A4);
                    }
                    break;
            }
            if (_ReportPath.Trim() != "")
            {
                if (_ReportSave.ToUpper().Trim() == "Y")
                {
                    _oPDFWriter = PdfWriter.GetInstance(oDocument, new FileStream(ReportPathName, FileMode.Create));  // Commented by Namrata 
                    _oPDFWriter.PageEvent = new PDFFooter();
                }
            }
            oDocument.AddAuthor("Idealake Information Technology Pvt. Ltd.");
            oDocument.AddCreator("Enhanced Reporting Engine");
            return oDocument;
        }
        #endregion

        #region Put an Empty Space before each table
        [Description("Build empty Table/Space"), Category("User Properties")]
        private PdfPTable PuttheSpace()
        {
            try
            {
                PdfPTable oPT = new PdfPTable(1);
                for (int i = 0; i < this._TableGap; i++)
                {
                    Phrase oPhrase = new Phrase(" ");
                    PdfPCell oCell = new PdfPCell(oPhrase);
                    oCell.Border = 0;
                    oPT.AddCell(oCell);
                }
                return oPT;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        #endregion

        #region Build Parent Child Relation
        private string[] BuildNestedStructure(DataRelation oDR)
        {
            string[] aRelations = { oDR.RelationName, oDR.ParentTable.TableName, oDR.ChildTable.TableName, oDR.ParentColumns[0].ColumnName, oDR.ChildColumns[0].ColumnName };
            return aRelations;
        }
        #endregion

        #region Data Formatter

        private PdfPCell DataFormatter(string ColType, string ColName, string ColData)
        {
            return this.DataFormatter(ColType, ColName, ColData, false);
        }


        private PdfPCell DataFormatter(string ColType, string ColName, string ColData, bool PrintTotal)
        {
            try
            {
                string sFormat = "";
                string PrintValue = ColData;
                bool Highlight = false;
                PdfPCell oCell = this.CreateContent(PrintValue);
                oCell = this.CreateContentSmall(PrintValue, Highlight);

                if (_Formatter.ContainsKey(ColName))
                {
                    sFormat = _Formatter[ColName].ToString();
                    switch (ColType)
                    {
                        case "Int16":
                            int int16Value = Convert.ToInt16(ColData);
                            PrintValue = int16Value.ToString(sFormat);
                            Highlight = _HighlightNegative && int16Value < 0 ? true : false;
                            oCell = this.CreateContent(PrintValue, Highlight);
                            oCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            this.RunningTotal(ColType, ColName, ColData, PrintTotal);
                            break;
                        case "Int32":
                            int int32Value = Convert.ToInt32(ColData);
                            PrintValue = int32Value.ToString(sFormat);
                            Highlight = _HighlightNegative && int32Value < 0 ? true : false;
                            oCell = this.CreateContent(PrintValue, Highlight);
                            oCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            this.RunningTotal(ColType, ColName, ColData, PrintTotal);
                            break;
                        case "Int64":
                            long int64Value = Convert.ToInt64(ColData);
                            PrintValue = int64Value.ToString(sFormat);
                            Highlight = _HighlightNegative && int64Value < 0 ? true : false;
                            oCell = this.CreateContent(PrintValue, Highlight);
                            oCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            this.RunningTotal(ColType, ColName, ColData, PrintTotal);
                            break;
                        case "Decimal":
                            decimal DecValue = Convert.ToDecimal(ColData);
                            PrintValue = DecValue.ToString(sFormat);
                            Highlight = _HighlightNegative && DecValue < 0 ? true : false;
                            oCell = this.CreateContent(PrintValue, Highlight);
                            oCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            this.RunningTotal(ColType, ColName, ColData, PrintTotal);
                            break;
                        case "Double":
                            double dbValue = Convert.ToDouble(ColData);
                            PrintValue = dbValue.ToString(sFormat);
                            Highlight = _HighlightNegative && dbValue < 0 ? true : false;
                            oCell = this.CreateContent(PrintValue, Highlight);
                            oCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            this.RunningTotal(ColType, ColName, ColData, PrintTotal);
                            break;
                        case "Byte":
                            oCell = this.CreateContent(PrintValue);
                            oCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            break;
                        case "Boolean":
                            if (_NeedIcon)
                            {
                                PrintValue = (ColData == "True") ? "ü" : "";
                                string Winpath = System.Environment.GetEnvironmentVariable("SystemRoot");
                                BaseFont winding = BaseFont.CreateFont(Winpath + "\\fonts\\Wingding.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                Font oFont = new Font(winding, 12, Font.NORMAL);
                                oCell = this.CreateContent(PrintValue, oFont, false);
                                oCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                            }
                            break;
                        case "DateTime":
                            DateTime oDate = Convert.ToDateTime(ColData);
                            PrintValue = oDate.ToString(sFormat);
                            oCell = this.CreateContent(PrintValue);
                            oCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                            break;
                        case "Guid":
                            oCell = this.CreateContent(PrintValue);
                            oCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                            break;
                        case "String":
                            oCell = this.CreateContent(PrintValue);
                            oCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            break;
                        default:
                            oCell = this.CreateContent(PrintValue);
                            oCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            break;
                    }
                }

                return oCell;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        #endregion

        #region RunningTotal Builder
        private void RunningTotal(string ColType, string ColName, string ColData, bool TotalPrint)
        {
            try
            {
                if (_Totals.ContainsKey(ColName))
                {
                    double dbValue = Convert.ToDouble(_Totals[ColName]);
                    if (TotalPrint)
                    {
                        dbValue = 0.00;
                    }
                    else
                    {
                        dbValue += Convert.ToDouble(ColData);
                    }
                    _Totals[ColName] = dbValue.ToString();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        public class PDFFooter : PdfPageEventHelper
        {
            PdfContentByte cb;
            PdfTemplate template;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                cb = writer.DirectContent;
                template = cb.CreateTemplate(100, 100);

            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                try
                {
                    iTextSharp.text.Image imghead = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/images/logo.png"));
                    imghead.ScaleAbsolute(112f, 16f);
                    //imgfoot.SetAbsolutePosition(0, 0);
                    imghead.SetAbsolutePosition(0, 0);
                    //imghead.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    PdfContentByte cbhead = writer.DirectContent;
                    PdfTemplate tp = cbhead.CreateTemplate(250, 67); // units are in pixels 
                    tp.AddImage(imghead);
                    cbhead.AddTemplate(tp, 10, 810);
                }
                catch (Exception ex)
                {

                }

                int pageN = writer.PageNumber;
                String text = "Page " + pageN.ToString();
                iTextSharp.text.Rectangle pageSize = document.PageSize;
                cb.SetColorFill(BaseColor.BLACK);
                cb.BeginText();
                cb.SetFontAndSize(bf, 9);
                cb.SetTextMatrix(530, 820);
                cb.ShowText(text);
                cb.EndText();


                // write the text in the pdf content
                cb.BeginText();
                string HeadText = "STATEMENT OF ACCOUNT";
                cb.SetFontAndSize(bf, 12);
                cb.SetColorFill(BaseColor.BLACK);
                // put the alignment and coordinates here
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, HeadText, 300, 813, 0);
                cb.EndText();

                //cb.BeginText();
                //string FooterText1 = "Please verify your Name & Address, Application no, Broker details and other informationprinted in this statement. In case of any discrepancy,";
                //cb.SetFontAndSize(bf, 8);
                //cb.SetColorFill(BaseColor.BLACK);
                //// put the alignment and coordinates here
                //cb.ShowTextAligned(0, FooterText1, 5, 30, 0);
                //cb.EndText();

                //cb.BeginText();
                //string FooterText2 = "Please inform us by E - Mail to alert_l@camsonline.com or contact us at the number mention below.";
                //cb.SetFontAndSize(bf, 8);
                //cb.SetColorFill(BaseColor.BLACK);
                //// put the alignment and coordinates here
                //cb.ShowTextAligned(0, FooterText2, 5, 18, 0);
                //cb.EndText();

            }
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {

                BaseFont bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                template.BeginText();
                template.SetFontAndSize(bf1, 9);
                template.SetColorFill(BaseColor.BLACK);
                template.SetTextMatrix(10, 0);
                template.ShowText("" + (writer.PageNumber - 1));
                template.EndText();
            }
        }

    }
}
