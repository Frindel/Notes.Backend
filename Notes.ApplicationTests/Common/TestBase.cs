using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Application.Interfaces;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Common
{
    internal abstract class TestBase : IDisposable
    {
        List<DataContext> _createdContexts;

        protected IMapper Mapper { get; }

        public TestBase()
        {
            Mapper = GetConfiguredMapper();
            _createdContexts = new List<DataContext>();
        }

        IMapper GetConfiguredMapper()
        {
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(
                    typeof(INotesContext).Assembly));
            });
            return configurationProvider.CreateMapper();
        }

        protected DataContext CreateEmptyDataContex()
        {
            DataContext newContext = DataContextFactory.GetTestDataContextOnMemmory();
            _createdContexts.Add(newContext);

            return newContext;
        }

        #region Memmory cleaning

        public void Dispose()
        {
            foreach(DataContext context in _createdContexts)
                DataContextFactory.DestroyTestDataContexOnMemmory(context);
        }

        #endregion

    }
}
