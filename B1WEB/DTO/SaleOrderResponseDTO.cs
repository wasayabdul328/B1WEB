using Newtonsoft.Json;

namespace B1WEB.DTO
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class QueryResponseDTO
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public string SqlText { get; set; }
        public List<object> value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string odatanextLink { get; set; }
    }
       public class QueryResponseImagePathDTO
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public string SqlText { get; set; }
        public List<Valueforimage> value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string odatanextLink { get; set; }
    }

    public class Valueforimage
    {
        public string BitmapPath { get; set; }
       
    }
    
    public class Pricelistprice
    {
        public string Price { get; set; }
       
    }


    public class QueryResponseForContactPersonDTO
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public string SqlText { get; set; }
        public List<contactperson> value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string odatanextLink { get; set; }
    }
    public class QueryResponseForPrice
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public string SqlText { get; set; }
        public List<Pricelistprice> value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string odatanextLink { get; set; }
    }
    public class QueryResponseForLoginDTO
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public string SqlText { get; set; }
        public List<loginCustomer> value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string odatanextLink { get; set; }
    }

    public class loginCustomer
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string ListNum { get; set; }
        public string U_Password { get; set; }
    }
    public class contactperson
    {
        public string CntctCode { get; set; }
        public string Name { get; set; }
    }

    public class DocumentLine
    {
        public string ItemCode { get; set; }
        public string Quantity { get; set; }
        public string TaxCode { get; set; }
        public string UnitPrice { get; set; }
        public string DiscountPercent { get; set; }
    }

    public class SaleOrderDto
    {
        public string CardCode { get; set; }
        public string NumAtCard { get; set; }
        public string DocDueDate { get; set; }
        public string DocDate { get; set; }
        public string TaxDate { get; set; }
        public string SalesPersonCode { get; set; }
        public string Comments { get; set; }
        public string DiscountPercent { get; set; }
        public List<DocumentLine> DocumentLines { get; set; }
    }



}
