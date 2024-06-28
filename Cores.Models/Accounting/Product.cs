    using System.ComponentModel.DataAnnotations;

    namespace Cores.Models.Accounting;

    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
    }