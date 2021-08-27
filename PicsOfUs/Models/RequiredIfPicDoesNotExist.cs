using Microsoft.Ajax.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace PicsOfUs.Models
{
    public class RequiredIfPicDoesNotExist : ValidationAttribute
    {
        private string _propertyName;
        public RequiredIfPicDoesNotExist(string propertyName)
        {
            _propertyName = propertyName;

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_propertyName);
            if (property == null)
            {
                throw new ValidationException($"Unknown property {_propertyName}");
            }

            try
            {
                Pic pic = (Pic) property.GetValue(validationContext.ObjectInstance, null);
                if (pic.Url.IsNullOrWhiteSpace())
                {
                    return new ValidationResult("Please selected a file to upload.");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            catch (Exception e)
            {
                throw new ValidationException($"Property: {_propertyName} is not of type Pic");
            }

        }
    }
}