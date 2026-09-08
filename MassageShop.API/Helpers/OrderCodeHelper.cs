namespace MassageShop.API.Helpers
{
    public static class OrderCodeHelper
    {
        public static string Generate(string prefix = "ORD")
        {
            var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}{timestamp}{random}";
        }
    }
}
