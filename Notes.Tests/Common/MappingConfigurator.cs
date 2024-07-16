using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Application.Interfaces;

namespace Notes.ApplicationTests.Common
{
    internal class MappingConfigurator
    {
        IMapper? _mapper;

        public IMapper GetMapper()
        {
            if (_mapper == null)
                _mapper = GetConfiguredMapper();

            return _mapper;
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
    }
}
