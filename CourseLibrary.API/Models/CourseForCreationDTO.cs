using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    public class CourseForCreationDTO: CourseForManipulationDTO //:IValidatableObject
    {
         // this fire last 
        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //  {
       //     if(Title == Description)
       //     {
       //          yield return new ValidationResult(
         //           "The provided description should be different from the title.", new [] {"CourseForCreationDTO"}
       //            );
         //   }
      //     }
    }
}
