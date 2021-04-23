using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hunted_Mobile.Model {
    public abstract class CustomModelErrorMessages<T> : IValidatableObject {
        private Dictionary<string, string> errorMessages = new Dictionary<string, string>();
        public Dictionary<string, string> ErrorMessages {
            get => errorMessages; set {
                if(value != null)
                    errorMessages = value;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if(ErrorMessages != null && ErrorMessages.Count > 0) {

                // Loop through the propeties of the given model
                foreach(var property in typeof(T).GetProperties()) {
                    string propertyLowercase = property.Name.ToLower().Trim('_');

                    // Check if error-messages contains key and if key has value
                    if(ErrorMessages.ContainsKey(propertyLowercase) && ErrorMessages.TryGetValue(propertyLowercase, out string value)) {
                        ErrorMessages.Remove(propertyLowercase);

                        yield return new ValidationResult(value, new string[1] { property.Name });
                    }
                }
            }
        }
    }
}
