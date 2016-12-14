using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Sandtrap.Web.Validation
{

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfContainsAttribute : ValidationAttribute, IClientValidatable
    {

        #region .Declarations 

        private const string _DefaultErrorMessage = "The {0} field is required";
        private readonly string _DependentProperty;
        private IEnumerable<string> _TargetValues { get; set; }

        #endregion

        #region .Constructors 

        /// <summary>
        /// Initializes a new RequiredIfContainsAttribute.
        /// </summary>
        /// <param name="dependentProperty">
        /// 
        /// </param>
        /// <param name="targetValues">
        /// 
        /// </param>
        public RequiredIfContainsAttribute(string dependentProperty, string targetValues)
        {
            if (string.IsNullOrEmpty(dependentProperty))
            {
                throw new ArgumentNullException("dependentProperty");
            }
            _DependentProperty = dependentProperty;
            _TargetValues = targetValues.Split(',').Select(x => x.Trim().ToLower());
            ErrorMessage = _DefaultErrorMessage;
        }

        #endregion

        #region .Methods 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // Get the value of the dependant property
                var property = validationContext.ObjectInstance.GetType().GetProperty(_DependentProperty);
                var propertyValue = property.GetValue(validationContext.ObjectInstance, null);
                if (propertyValue != null && _TargetValues.Contains(propertyValue.ToString().ToLower()))
                {
                    return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "requiredifcontains",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };
            rule.ValidationParameters.Add("dependentproperty", _DependentProperty);
            rule.ValidationParameters.Add("targetvalues", string.Join(",", _TargetValues));
            yield return rule;
        }

        #endregion


    }
}
