using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BackOffice.Domain.Shared;

namespace BackOffice.Infrastructure.Shared
{
    public class EntityIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, String>
        where TTypedIdValue : EntityId
    {
        public EntityIdValueConverter(ConverterMappingHints? mappingHints = null) 
            : base(id => id.Value, value => Create(value), mappingHints)
        {
        }

        private static TTypedIdValue Create(String id)// => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
        {
            var instance = Activator.CreateInstance(typeof(TTypedIdValue), id);
            if (instance is TTypedIdValue typedIdValue)
            {
                return typedIdValue; 
            }
            
            throw new InvalidOperationException($"Cannot create an instance of {typeof(TTypedIdValue)} from string '{id}'.");
        }
    }
}