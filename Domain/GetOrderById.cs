using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnaWebDiningFin.Domain
{
    public class GetOrderById
    {
        public long ClientId { get; set; }
        public bool IsReady { get; set; }
        public long EstimatedWaitingTime { get; set; }
        public int Priority { get; set; }
        public float MaxWait { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime RegisteredTime { get; set; }
        public TimeSpan PreparedTime { get; set; }
        public List<CookingDetails> CookingDetails { get; set; }

        public GetOrderById()
        {

        }
    }
}
