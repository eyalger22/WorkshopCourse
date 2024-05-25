using Market.ORM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DomainLayer.Users
{
    // for future development
    public class Alert
    {
        [NotMapped]
        private static int Counter = -1;

        [Key]
        public int alertId { get; set; }

        public string User { get; set; }

        private string _message;

        public string Message
        {
            get => _message;
            private set => _message = value;
        }
        public Alert(string message, string user) 
        {
            UpdateCounter();
            _message = message;
            User = user;
            alertId = Counter++;
        }

        private static void UpdateCounter()
        {
            if (Counter == -1)
            {
                Counter = MarketContext.Instance.Alerts.Count();
                if (Counter > 0)
                {
                    Counter = MarketContext.Instance.Alerts.Max(x => x.alertId) + 1;
                }
                else
                {
                    Counter++;
                }
            }
        }
    }
}
