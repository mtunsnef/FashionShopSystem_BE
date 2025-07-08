using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service
{
    public class FavouriteResponseDto
    {
        public int FavoriteId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
