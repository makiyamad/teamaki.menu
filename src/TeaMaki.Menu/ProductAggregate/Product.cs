namespace TeaMaki.Menu.ProductAggregate
{
    public class Product
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public decimal Price { get; protected set; }
        public decimal Tax { get; protected set; }
        public string ImagePath { get; protected set; }

        public Product(string name, decimal price, decimal tax, string imagePath = null, string productId = null)
        {
            Name = name;
            Price = price;
            Tax = tax;
            ImagePath = imagePath;
            Id = productId ?? string.Empty;
        }
    }


}
