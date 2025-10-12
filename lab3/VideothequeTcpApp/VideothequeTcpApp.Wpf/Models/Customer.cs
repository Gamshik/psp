﻿namespace VideothequeTcpApp.Wpf.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime RegisteredAt { get; set; }
    }
}
