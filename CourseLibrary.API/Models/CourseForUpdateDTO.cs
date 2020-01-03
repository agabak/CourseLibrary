using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    public class CourseForUpdateDTO: CourseForManipulationDTO
    {
        [Required(ErrorMessage ="You should fill out a description.")]
        public override string Description{get => base.Description; set => base.Description = value;}
    }
}