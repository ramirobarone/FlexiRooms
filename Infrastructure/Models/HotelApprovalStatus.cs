using System.Text.Json.Serialization;

namespace Infrastructure.Models
{
    /// <summary>
    /// Publication state of a hotel registered by its owner ("Sumá tu hotel").
    /// Only Approved hotels appear in search and can take payments.
    /// Pending is the CLR default on purpose: a hotel created by any path stays hidden until reviewed.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HotelApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
