using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.DAL.EF
{
    internal static class ModelBuilderExtensions
    {
        internal static ModelBuilder UseLowerCaseTablesAndColumnsNames(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()!.ToLowerInvariant());

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.GetColumnName().ToLowerInvariant());
            }

            return modelBuilder;
        }
    }
}
