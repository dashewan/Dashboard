using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Dashboard.Core.ModelValidation;

namespace Dashboard.Core.ModelValidation
{
    public class Validation
    {
        public static void ValidateAttributes<TEntity>(TEntity entity)
        {
            var validationInstance = new Validation();
            validationInstance.ValidateAttributesInternal(entity);
        }

        public virtual void ValidateAttributesInternal<TEntity>(TEntity entity)
        {
            var validationIssues = new List<ValidationIssue>();

            // Get list of properties from validationModel
            var props = typeof(TEntity).GetProperties();
            var metatype = typeof(TEntity).GetCustomAttributes(typeof(MetadataTypeAttribute), false).FirstOrDefault();

            Type type = null;
            PropertyInfo[] s = null;
            if (metatype != null)
            {
                type = ((System.ComponentModel.DataAnnotations.MetadataTypeAttribute)(metatype)).MetadataClassType;
                s = type.GetProperties();
            }

            var customAttrs = typeof(TEntity).GetCustomAttributes(true).Where(t => t.GetType().Namespace.Contains("ValidationMeta"));
            foreach (var attr in customAttrs)
            {
                var validate = (ValidationAttribute)attr;
                bool valid = validate.IsValid(entity);
                if (!valid)
                {
                    validationIssues.Add(new ValidationIssue(null, null, validate.ErrorMessage));
                }
            }

            // Perform validation on each property
            foreach (var prop in s)
                ValidateProperty(validationIssues, entity, prop);

            // throw exception?
            if (validationIssues.Count > 0)
                throw new ValidationIssueException(validationIssues);
        }

        protected virtual void ValidateProperty<TEntity>(List<ValidationIssue> validationIssues, TEntity entity, PropertyInfo property)
        {
            // Get list of validator attributes
            var validators = property.GetCustomAttributes(typeof(ValidationAttribute), false);


            //var validators = property.Attributes(typeof(Attribute), false);
            foreach (ValidationAttribute validator in validators)
                ValidateValidator(validationIssues, entity, property, validator);
        }

        protected virtual void ValidateValidator<TEntity>(List<ValidationIssue> validationIssues, TEntity entity, PropertyInfo property, ValidationAttribute validator)
        {
            var dataEntityProperty = typeof(TEntity).GetProperties().FirstOrDefault(p => p.Name == property.Name);
            var value = dataEntityProperty.GetValue(entity, null);

            //var value = property.GetValue(entity, null);
            if (!validator.IsValid(value))
            {
                validationIssues.Add(new ValidationIssue(property.Name, value, validator.ErrorMessage));
            }
        }
    }
}
