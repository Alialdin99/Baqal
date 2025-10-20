using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Application.DTOs.common
{
    public class ErrorResponse
    {
        /// User-friendly error message
        public string Message { get; set; } = string.Empty;

        /// Technical details (only in development)
        public string? Details { get; set; }

        /// Timestamp of when error occurred
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// Unique error ID for tracking
        public string ErrorId { get; set; } = Guid.NewGuid().ToString();
    }
}
