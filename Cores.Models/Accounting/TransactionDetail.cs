﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Cores.Models.Accounting;

public class TransactionDetail
{
    public int Id { get; set; }
    
    public int TransactionId { get; set; }
    [ForeignKey("TransactionId")]
    [ValidateNever]
    public Transaction Transaction { get; set; }

    
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    [ValidateNever]
    public Account Account { get; set; }    
    
    public int CurrencyId { get; set; }
    [ForeignKey("CurrencyId")]
    [ValidateNever]
    public Currency Currency { get; set; }    
    
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}