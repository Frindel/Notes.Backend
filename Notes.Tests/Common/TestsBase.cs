using AutoMapper;

namespace Notes.ApplicationTests.Common
{
    internal abstract class TestsBase : IDisposable
    {
        protected DataContextManager ContextManager { get; }

        protected TestsHelper Helper { get; }

        protected IMapper Mapper { get; }


        public TestsBase()
        {
            ContextManager = new DataContextManager();
            Helper = new TestsHelper();

            var mappingConfigurator = new MappingConfigurator();
            Mapper = mappingConfigurator.GetMapper();
        }

        public void Dispose()
        {
            ContextManager.Dispose();
        }
    }
}
