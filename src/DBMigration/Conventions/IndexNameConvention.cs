using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;
using System.Text;


namespace DPMGallery.DBMigration
{
    public class DPMGalleryIndexNameConvention : IIndexConvention
    {
        public IIndexExpression Apply(IIndexExpression expression)
        {
            if (string.IsNullOrEmpty(expression.Index.Name))
            {
                expression.Index.Name = GetIndexName(expression.Index);
            }

            return expression;
        }

        private static string GetIndexName(IndexDefinition index)
        {
            var sb = new StringBuilder();

            sb.Append("ix_");
            sb.Append(index.TableName.ToLowerInvariant());

            foreach (IndexColumnDefinition column in index.Columns)
            {
                sb.Append("_");
                sb.Append(column.Name.ToLowerInvariant());
            }

            return sb.ToString();
        }
    }
}
