namespace QuestPDF_Demo.Models
{
    public class DealInfo
    {
        public string Type { get; set; }
        public string Issuer { get; set; }
        public string Currency { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Duration { get; set; }
        public string NetRate { get; set; }
        public int Principal { get; set; }
        public int ExpectedInterest { get; set; }
        public int NetMaturityValue { get; set; }
    }
}
