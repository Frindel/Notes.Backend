using Notes.Persistence.Data;
using Notes.Tests.Common;
using Notes.WebApi.Controllers;

namespace Notes.Tests.WebApi
{
    [TestFixture]
    internal class UsersControllerTests : ControllerTestsBase<UsersController>
    {
        DataContext _context;
        UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _controller = CreateController(_context);
        }

        [Test]
        public void SuccessfulRegister()
        {

        }
    }
}
