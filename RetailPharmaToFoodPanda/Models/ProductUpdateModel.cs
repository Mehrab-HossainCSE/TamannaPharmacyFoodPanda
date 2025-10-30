namespace RetailPharmaToFoodPanda.Models
{
    public class ProductUpdateModel
    {
        public string sBarcode { get; set; }
        public int BufferQty { get; set; }
        public bool ECProduct { get; set; }
        public string ImagePath { get; set; }
    }
}
