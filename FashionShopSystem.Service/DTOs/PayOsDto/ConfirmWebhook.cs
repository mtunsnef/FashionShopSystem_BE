﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.DTOs.PayOsDto
{
    public record ConfirmWebhook(
        string webhook_url
    );
}
