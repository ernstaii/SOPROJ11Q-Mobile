using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using Xamarin.Forms;

namespace Hunted_Mobile.Service {
    public static class ValidationHelper {
        public static bool IsFormValid(object model, Page page) {
            hideValidationFields(model, page);
            var errors = new List<ValidationResult>();
            var context = new ValidationContext(model);
            bool isValid = Validator.TryValidateObject(model, context, errors, true);
            if(!isValid) {
                showValidationFields(errors, model, page);
            }
            return errors.Count() == 0;
        }

        private static void hideValidationFields(object model, Page page, string validationLabelSuffix = "Error") {
            if(model == null) return;
            var properties = getValidatablePropertyNames(model);

            foreach(var propertyName in properties) {
                var errorControlName = $"{propertyName.Replace(".", "_")}{validationLabelSuffix}";
                var control = page.FindByName<Label>(errorControlName);
            }
        }

        private static void showValidationFields(List<ValidationResult> errors, object model, Page page, string validationLabelSuffix = "Error") {
            if(model == null) return;

            foreach(var error in errors) {
                var memberName = getMemberName(model, error);
                
                var errorControlName = $"{memberName}{validationLabelSuffix}";
                var control = page.FindByName<Label>(errorControlName);
                if(control != null) {
                    control.Text = $"{error.ErrorMessage}{System.Environment.NewLine}";
                    control.IsVisible = true;
                }
            }
        }

        private static IEnumerable<string> getValidatablePropertyNames(object model) {
            var validatableProperties = new List<string>();
            var properties = getValidatableProperties(model);
            foreach(var propertyInfo in properties) {
                var errorControlName = $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}";
                validatableProperties.Add(errorControlName);
            }
            return validatableProperties;
        }

        private static List<PropertyInfo> getValidatableProperties(object model) {
            var properties = model.GetType().GetProperties().Where(prop => prop.CanRead
                && prop.GetCustomAttributes(typeof(ValidationAttribute), true).Any()
                && prop.GetIndexParameters().Length == 0).ToList();
            return properties;
        }

        private static string getMemberName(object model, ValidationResult error) {
            string typeName = "";

            try {
                typeName = model
                    .GetType()
                    .GetProperties()
                    .FirstOrDefault(o => o.Name == "ValidationField")
                    .GetValue(model, null)
                    ?.ToString();
            }
            catch(Exception e) {
            }

            var memberName = $"{typeName ?? model.GetType().Name}_{error.MemberNames.FirstOrDefault()}";
            return memberName.Replace(".", "_");
        }
    }
}
