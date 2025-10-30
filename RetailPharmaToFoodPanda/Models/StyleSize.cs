namespace RetailPharmaToFoodPanda.Models
{
    public class StyleSize
    {
        public string? CMPIDX { get; set; }

        // This appears to be the Primary Key
        public string sBarcode { get; set; }

        public string Barcode { get; set; }
        public string CSSID { get; set; }
        public string SSID { get; set; }
        public string SSName { get; set; }
        public string PrdID { get; set; }
        public string PrdName { get; set; }
        public string CBTID { get; set; }
        public string BTID { get; set; }
        public string BTName { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string? FloorID { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal? VATPrcnt { get; set; }
        public decimal? PrdComm { get; set; }
        public decimal? CPU { get; set; }
        public decimal? RPU { get; set; }
        public decimal? RPP { get; set; }
        public string? DisContinued { get; set; }
        public string SupID { get; set; }
        public string? SupName { get; set; }
        public string? UserID { get; set; }
        public DateTime? ENTRYDT { get; set; }
        public string? ZoneID { get; set; }
        public decimal? Point { get; set; }
        public decimal? Reorder { get; set; }
        public decimal? MaxOrder { get; set; }
        public string? PcsInbox { get; set; }
        public string? COO { get; set; }
        public decimal? WSP { get; set; }
        public decimal? WSPQTY { get; set; }
        public decimal? AVGCPU { get; set; }
        public decimal? OpeningQty { get; set; }
        public decimal? ClosingQty { get; set; }
        public decimal? VatPrcnt_Chln { get; set; }
        public bool? ECProduct { get; set; }
        public string? ImagePath { get; set; }
        public decimal? BufferQty { get; set; }
    }
   

    public class StyleSizeViewModel
    {
        public string? CMPIDX { get; set; }

        // This appears to be the Primary Key
        public string sBarcode { get; set; }

        public string Barcode { get; set; }
        public string CSSID { get; set; }
        public string SSID { get; set; }
        public string SSName { get; set; }
        public string PrdID { get; set; }
        public string PrdName { get; set; }
        public string CBTID { get; set; }
        public string BTID { get; set; }
        public string BTName { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string? FloorID { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal? VATPrcnt { get; set; }
        public decimal? PrdComm { get; set; }
        public decimal? CPU { get; set; }
        public decimal? RPU { get; set; }
        public decimal? RPP { get; set; }
        public string? DisContinued { get; set; }
        public string SupID { get; set; }
        public string? SupName { get; set; }
        public string? UserID { get; set; }
        public DateTime? ENTRYDT { get; set; }
        public string? ZoneID { get; set; }
        public decimal? Point { get; set; }
        public decimal? Reorder { get; set; }
    }

    public class StyleSizeSearchResult
    {
        public List<StyleSize> StyleSizes { get; set; } = new();
        public int TotalProducts { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
    }
}
