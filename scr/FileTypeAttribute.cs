using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sandtrap.Web.Validation
{

    /// <summary>
    /// Validation attribute to assert that an uploaded file (or files) must have
    /// have a specified file extension
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileTypeAttribute : ValidationAttribute, IClientValidatable
    {

        #region .Declarations 

        private const string _DefaultErrorMessage = "Only the following file types are allowed: {0}";
        private IEnumerable<string> _ValidTypes { get; set; }

        #endregion

        #region .Constructors 

        /// <summary>
        /// Initializes a new FileTypeAttribute.
        /// </summary>
        /// <param name="validTypes">
        /// A comma separated list of valid file extensions.
        /// </param>
        public FileTypeAttribute(string validTypes)
        {
            _ValidTypes = validTypes.Split(',').Select(s => s.Trim().ToLower());
            ErrorMessage = string.Format(_DefaultErrorMessage, string.Join(" or ", _ValidTypes));
        }

        #endregion

        #region .Methods 

        /// <summary>
        /// Override of <see cref="ValidationAttribute.IsValid()"/>
        /// </summary>
        /// <param name="value">
        /// The value to test.
        /// </param>
        /// <param name="validationContext">
        /// The context in which a validation check is performed
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the file extension is valid.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// is thrown if the property the attibute is applied to is not HttpPostedFileBase or
        /// assignable from IEnumerable<HttpPostedFileBase>
        /// </exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CheckPropertyType(GetType());
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null && !_ValidTypes.Any(x => file.FileName.EndsWith(x)))
                    {
                        return new ValidationResult(ErrorMessageString);
                    }
                }
            }
            else
            {
                HttpPostedFileBase file = value as HttpPostedFileBase;
                if (file != null && _ValidTypes.Any(x => file.FileName.EndsWith(x)))
                {
                    return new ValidationResult(ErrorMessageString);
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
            CheckPropertyType(metadata.ModelType);
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filetype",
                ErrorMessage = ErrorMessageString
            };
            rule.ValidationParameters.Add("validtypes", string.Join(",", _ValidTypes));
            yield return rule;
        }

        #endregion

        #region .Helper methods 

        private void CheckPropertyType(Type propertyType)
        {
            if (propertyType == typeof(HttpPostedFileBase))
            {
                return;
            }
            if (typeof(IEnumerable<HttpPostedFileBase>).IsAssignableFrom(propertyType))
            {
                return;
            }
            // TODO: Add to resource file
            string errMsg = "A FileTypeAttribute can only be applied to a property which is HttpPostedFileBase or is assignable from IEnumerable<HttpPostedFileBase>";
            throw new ArgumentException(errMsg);
        }

        #endregion

    }
}