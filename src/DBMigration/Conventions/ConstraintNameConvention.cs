using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;
using System.Text;


namespace DPMGallery.DBMigration
{
    public class DPMGalleryConstraintNameConvention : IConstraintConvention
    {
        public IConstraintExpression Apply(IConstraintExpression expression)
        {
            if (string.IsNullOrEmpty(expression.Constraint.ConstraintName))
            {
                expression.Constraint.ConstraintName = GetConstraintName(expression.Constraint);
            }

            return expression;
        }

        private static string GetConstraintName(ConstraintDefinition expression)
        {
            var sb = new StringBuilder();
            sb.Append(expression.IsPrimaryKeyConstraint ? "pk_" : "uc_");

            sb.Append(expression.TableName);
            foreach (var column in expression.Columns)
            {
                sb.Append('_').Append(column);
            }
            return sb.ToString().ToLower();
        }
    }
}
