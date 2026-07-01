namespace Invento.Application.Common.Caching
{
    public static class CacheKeys
    {

        public static string Dashboard()
            => "dashboard";

        public static string Company()
            => "company";

        public static string TenantSettings()
            => "tenant-settings";

        public static string Users(
            string identifier)
            => $"users:{identifier}";

        public static string User(
            Guid userId)
            => $"user:{userId}";

        public static string Categories(
            string identifier)
            => $"categories:{identifier}";

        public static string Category(
            Guid categoryId)
            => $"category:{categoryId}";

        public static string Products(
            string identifier)
            => $"products:{identifier}";

        public static string Product(
            Guid productId)
            => $"product:{productId}";

        public static string Customers(
            string identifier)
            => $"customers:{identifier}";

        public static string Customer(
            Guid customerId)
            => $"customer:{customerId}";

        public static string Suppliers(
            string identifier)
            => $"suppliers:{identifier}";

        public static string Supplier(
            Guid supplierId)
            => $"supplier:{supplierId}";
        public static string Sales(
            string identifier)
            => $"sales:{identifier}";

        public static string Sale(
            Guid saleId)
            => $"sale:{saleId}";

        public static string Purchases(
            string identifier)
            => $"purchases:{identifier}";

        public static string Purchase(
            Guid purchaseId)
            => $"purchase:{purchaseId}";

        public static string Receivables(
            string identifier)
            => $"receivables:{identifier}";

        public static string Payables(
            string identifier)
            => $"payables:{identifier}";

        public static string Balance(
            string identifier)
            => $"balance:{identifier}";

        public static string Profit(
            string identifier)
            => $"profit:{identifier}";

        public static string Targets(
            string identifier)
            => $"targets:{identifier}";

        public static string Reports(
            string identifier)
            => $"reports:{identifier}";
    }
}