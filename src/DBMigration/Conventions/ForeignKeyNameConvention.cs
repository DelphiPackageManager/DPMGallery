using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;
using System.Text;

namespace DPMGallery.DBMigration
{
    public class DPMGalleryForeignKeyNameConvention : IForeignKeyConvention
    {
        public IForeignKeyExpression Apply(IForeignKeyExpression expression)
        {
            if (string.IsNullOrEmpty(expression.ForeignKey.Name))
            {
                expression.ForeignKey.Name = GetForeignKeyName(expression.ForeignKey);
            }

            return expression;
        }

        private static string GetForeignKeyName(ForeignKeyDefinition foreignKey)
        {
            var sb = new StringBuilder();

            sb.Append("fk_");
            sb.Append(foreignKey.ForeignTable);

            foreach (string foreignColumn in foreignKey.ForeignColumns)
            {
                sb.Append("_");
                sb.Append(foreignColumn.ToLowerInvariant());
            }

            sb.Append("_");
            sb.Append(foreignKey.PrimaryTable.ToLowerInvariant());

            foreach (string primaryColumn in foreignKey.PrimaryColumns)
            {
                sb.Append("_");
                sb.Append(primaryColumn.ToLowerInvariant());
            }

            return sb.ToString();
        }
    }
}