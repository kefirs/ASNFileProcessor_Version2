namespace ASNFileProcessor
{
    public class ASNLine
    {
        public int? Id { get; set; }
        public string? ProductId { get; set; } //Product ID i.e "P000001661"
        public string? ISBNCode { get; set; } // Product barcode i.e "9781473663800"
        public int? Quantity { get; set; }
        public int? ASNHeaderId { get; set; }
        public ASNHeader? ASNHeader { get; set; }
    }
}
