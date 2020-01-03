using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class AuthorForCreationDTO
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        public string MainCategory { get; set; }

        public ICollection<CourseForCreationDTO> Courses { get; set; }
               = new List<CourseForCreationDTO>();
    }
}

// Note: try to use separate dto for every action 
