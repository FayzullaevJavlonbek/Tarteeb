//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Text.Json.Serialization;
using Tarteeb.Api.Models.Foundations.Tickets;
using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.Models.Foundations.Times
{
    public class Time
    {
        public Guid Id { get; set; }
        public decimal HoursWorked { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DateWorked { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public Guid TicketId { get; set; }
        [JsonIgnore]
        public Ticket Ticket { get; set; }

        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
