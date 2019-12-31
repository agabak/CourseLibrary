using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDTO>> GetCoursesForAuthor(Guid authorId)
        {
             if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();
              return Ok(_mapper.Map<IEnumerable<CourseDTO>>(_courseLibraryRepository.GetCourses(authorId)));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<IEnumerable<CourseDTO>> GetCourseForAuthor(Guid authorId,Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();
            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null) return NotFound(); 
            return Ok(_mapper.Map<IEnumerable<CourseDTO>>(course));
        }

        [HttpPost]
        public ActionResult<CourseDTO> CreateCourseForAuthor(Guid authorId, CourseForCreationDTO course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();

            var courseEntity = _mapper.Map<Entities.Course>(course);

            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CourseDTO>(courseEntity);

            return CreatedAtRoute("GetCourseForAuthor", new {authorId = authorId, courseId = courseToReturn.Id }, courseToReturn);
        }

    }
}