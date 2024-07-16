﻿using MediatR;
using Notes.Application.Categories.Dto;

namespace Notes.Application.Categories.Queries.GetAllCategories
{
    public record class GetAllCategoriesQuery : IRequest<CategoriesDto>
    {
        public int UserId { get; set; }
    }
}
