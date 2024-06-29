using AutoMapper;

namespace Notes.Application.Common.Mapping
{
    public abstract class MappingBase<T>
    {
        public virtual void Mapping(Profile profile) =>
            profile.CreateMap(typeof(T), GetType());
    }
}
