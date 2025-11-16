using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopcornMarket.ServiceBus.Models;
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int Attempts { get; set; }
    public string? Key { get; set; }  // optional Kafka key
}
