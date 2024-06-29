using AutoMapper;
using System.Reflection;

namespace Notes.Application.Common.Mapping
{
    public class AssemblyMappingProfile : Profile
    {
        public AssemblyMappingProfile(Assembly assembly) =>
            ApplyMappingsFromAssembly(assembly);

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            // регистрация мапперов проекта
            var types = assembly.GetExportedTypes()
                .Where(t => t.BaseType?.Name == typeof(MappingBase<>).Name)
                .ToArray();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var mappingMethod = type.GetMethod("Mapping");
                mappingMethod!.Invoke(instance, new[] { this });
            }
        }
    }
}
