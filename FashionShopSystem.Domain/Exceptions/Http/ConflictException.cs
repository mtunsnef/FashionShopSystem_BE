﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Domain.Exceptions.Http
{
    public class ConflictException : Exception
    {
        public ConflictException() : base("Conflict")
        {

        }
        public ConflictException(string message) : base(message)
        {

        }
        public ConflictException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
