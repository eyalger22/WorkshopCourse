namespace Market.DomainLayer.ExternalServicesAdapters
{
    public class ExternalService
    {
        private static ExternalService _instance = null;
        public virtual PaymentService PaymentService { get; set; }
        public virtual DeliveryService DeliveryService { get; set; }
        private ExternalService() {
            PaymentService = new PaymentServiceAdapter(new PaymentServiceProxy(new PaymentServiceReal()));
            DeliveryService = new DeliveryServiceAdapter(new DeliveryServiceProxy(new DeliveryServiceReal()));                              
        }

        public static ExternalService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExternalService();
                }
                return _instance;
            }
        }
    }
}
