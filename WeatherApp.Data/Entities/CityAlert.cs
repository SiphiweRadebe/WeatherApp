using System;

namespace WeatherApp.Data.Entities
{
    public class CityAlert
    {
        public int CityId { get; set; }
        public int AlertId { get; set; }
        public DateTime AssociatedAt { get; set; }
        public bool NotificationSent { get; set; }

        public City City { get; set; } = null!;
        public Alert Alert { get; set; } = null!;
    }
}