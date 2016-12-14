using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sandtrap.Web.Validation
{

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {

        #region .Declarations 

        private const string _DefaultErrorMessage = "Please enter the {0}.";
        private readonly string _PropertyName;
        private readonly object _Value;

        #endregion

        #region .Constructors 

        public RequiredIfAttribute(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            _PropertyName = propertyName;
            _Value = value;
            ErrorMessage = _DefaultErrorMessage;
        }

        #endregion

        #region .Methods 

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                var property = validationContext.ObjectInstance.GetType().GetProperty(_PropertyName);
                var propertyValue = property.GetValue(validationContext.ObjectInstance, null);
                if (propertyValue != null && propertyValue.Equals(_Value))
                {
                    return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };
            rule.ValidationParameters.Add("dependentproperty", _PropertyName);
            rule.ValidationParameters.Add("targetvalue", _Value);
            yield return rule;
        }

        #endregion

    }

}