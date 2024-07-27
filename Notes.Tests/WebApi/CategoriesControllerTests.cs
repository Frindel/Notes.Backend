using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Exceptions;
using Notes.Domain;
using Notes.Persistence.Data;
using Notes.Tests.Common;
using Notes.WebApi.Controllers;
using Notes.WebApi.Models.Categories;

namespace Notes.Tests.WebApi
{
    [TestFixture]
    internal class CategoriesControllerTests : ControllerTestsBase<CategoriesController>
    {
        DataContext _context;
        CategoriesController _controller;
        User _notSavedUser;
        User _savedUser;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _controller = CreateController(_context);
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _notSavedUser = Helper.CreateUserOfNumber(2);
        }

        [Test]
        public async Task GetAllCategories_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Helper.AddCategoriesWithNumbers(_context, _savedUser, 1);

            // Act
            ObjectResult response = (ObjectResult)await _controller.GetAll();
            List<CategoryDto> categories = (List<CategoryDto>)response.Value!;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(categories, "result is null");
                Assert.IsNotEmpty(categories, "categories not geted");
            });
        }

        [Test]
        public void GetAllCategories_NegativeUserId_ThrowsValudationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, -1);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetAll());
        }

        [Test]
        public void GetAllCategories_NegativePageNumber_ThrowsValidationException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetAll(pageNumber: negativeValue));
        }

        [Test]
        public void GetAllCategories_NegativePageSize_ThrowsValidationException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetAll(pageSize: negativeValue));
        }

        [Test]
        public async Task GetCategory_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Category savedCategory = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1).First();

            // Act
            ObjectResult response = (ObjectResult)await _controller.GetById(savedCategory.PersonalId);
            CategoryDto? getedCategory = response.Value as CategoryDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(getedCategory, "result is null");
                Assert.IsTrue(getedCategory!.Id == savedCategory.PersonalId);
            });
        }

        [Test]
        public void GetCategory_NegativeUserIdOrCategoryId_ThrowsValidationException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, negativeValue);

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetById(negativeValue));
        }

        [Test]
        public void GetCategory_NonExistentUser_ThrowsUserNotFoundException()
        {
            var a = _context.Users.ToList();

            // Arange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _notSavedUser.Id);
            Category category = Helper.CreateCategoryOfNumber(1, _notSavedUser);

            // Act / assert
            Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(category.PersonalId));
        }

        [Test]
        public void GetCategory_NonExistentCategory_ThrowsUserNotFoundException()
        {
            // Arange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Category notSavedCategory = Helper.CreateCategoryOfNumber(1, _savedUser);

            // Act / assert
            Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(notSavedCategory.PersonalId));
        }

        [Test]
        public async Task CreateCategory_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Category notSavedCategory = Helper.CreateCategoryOfNumber(1, _savedUser);
            var request = new CreateCategoryRequest()
            {
                Name = notSavedCategory.Name
            };

            // Act
            ObjectResult response = (ObjectResult)await _controller.Create(request);
            CategoryDto? addedCategory = response.Value as CategoryDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(addedCategory, "result is null");
                Assert.IsNotNull(addedCategory!.Name, "created category name is null");
            });
        }

        // имя не задано
        [Test]
        public void CreateCategory_MissingCategoryName_ThrowsValidationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            var request = new CreateCategoryRequest();

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Create(request));
        }

        // id пользователя < 0
        [Test]
        public void CreateCategory_NegativeUserIds_ThrowsValudationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, -1);
            Category category = Helper.CreateCategoryOfNumber(1, _notSavedUser);
            var request = new CreateCategoryRequest()
            {
                Name = category.Name
            };

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Create(request));
        }

        // пользователь не найден
        [Test]
        public void CreateCategory_NonExistentUser_ThrowsUserNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _notSavedUser.Id);
            Category category = Helper.CreateCategoryOfNumber(1, _notSavedUser);
            var request = new CreateCategoryRequest()
            {
                Name = category.Name
            };

            // Act / assert
            Assert.ThrowsAsync<NotFoundException>(() => _controller.Create(request));
        }
    }
}