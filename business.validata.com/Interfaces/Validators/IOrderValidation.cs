﻿using business.validata.com.Validators.Models;
using model.validata.com.Entities;
using model.validata.com.Enumeration;


namespace business.validata.com.Interfaces.Validators
{
    public interface IOrderValidation
    {
        Task<OrderValidationResult> InvokeAsync(Order order, BusinessSetOperation businessSetOperation);
    }
}
