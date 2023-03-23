﻿using System.ComponentModel.DataAnnotations;

using static PersonalFinancer.Data.Constants.TransactionConstants;

namespace PersonalFinancer.Services.Transactions.Models
{
    public class CategoryInputModel
    {
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
            ErrorMessage = "Category name must be between {2} and {1} characters long.")]
        public string Name { get; init; } = null!;

        public string OwnerId { get; set; } = null!;
    }
}
