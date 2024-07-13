using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Common
{
    internal class DataContextManager : IDisposable
    {
        List<DataContext> _createdContexts;

        public DataContextManager()
        {
            _createdContexts = new List<DataContext>();
        }

        public DataContext CreateEmptyDataContex()
        {
            DataContext newContext = DataContextFactory.GetTestDataContextOnMemmory();
            _createdContexts.Add(newContext);

            return newContext;
        }

        #region Memmory cleaning

        public void Dispose()
        {
            foreach (DataContext context in _createdContexts)
                DataContextFactory.DestroyTestDataContexOnMemmory(context);
        }

        #endregion

    }
}
