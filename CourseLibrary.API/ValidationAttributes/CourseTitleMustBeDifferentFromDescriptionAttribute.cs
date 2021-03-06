using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute: ValidationAttribute
    {
       protected override ValidationResult IsValid(object value, ValidationContext validationContext) 
       {
           var course = (CourseForManipulationDTO)validationContext.ObjectInstance;
           if(course.Title == course.Description)
           {
                return new ValidationResult(
                    "The provided description should be different from the title.", new [] {nameof(CourseForManipulationDTO)}
                );
           }
           return ValidationResult.Success;
       }   
    }
}