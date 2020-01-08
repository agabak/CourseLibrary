using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

         [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId,CourseForUpdateDTO course)
        {
            if(!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();
            var courseForAuthorFromRep = _courseLibraryRepository.GetCourse(authorId,courseId);
            if(courseForAuthorFromRep == null)
            {
                var courseToAdd = _mapper.Map<Entities.Course>(course);
                    courseToAdd.Id = courseId;
                    _courseLibraryRepository.AddCourse(authorId,courseToAdd);
                    _courseLibraryRepository.Save();

                 var courseToReturn = _mapper.Map<CourseDTO>(courseToAdd);

                 return CreatedAtRoute("GetCourseForAuthor", new [] {authorId, courseId = courseToReturn.Id}, courseToReturn);
            }

            //map the entity to a CourseForUpdateDTO
            //apply the update field values to that dto
            //map the CourseForUpdateDTO back to an entity
            _mapper.Map(course, courseForAuthorFromRep);
            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRep);

            _courseLibraryRepository.Save();
            return NoContent();

        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDTO> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();
            var courseForAuthorFromRep = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseForAuthorFromRep == null)
            {
               var courseDTO = new CourseForUpdateDTO();
               patchDocument.ApplyTo(courseDTO);
               var courseToAdd = _mapper.Map<Entities.Course>(courseDTO);
               courseToAdd.Id = courseId;

               _courseLibraryRepository.AddCourse(authorId,courseToAdd);
               _courseLibraryRepository.Save();

               var courseToReturn = _mapper.Map<CourseDTO>(courseToAdd);
               return CreatedAtRoute("GetCourseForAuthor", new {authorId, courseId= courseToReturn.Id}, courseToReturn);
            }

            var courseToPatch = _mapper.Map<CourseForUpdateDTO>(courseForAuthorFromRep);

            //Add validation

            patchDocument.ApplyTo(courseToPatch, ModelState);
            if (!TryValidateModel(courseToPatch)) return ValidationProblem(ModelState);



            _mapper.Map(courseToPatch, courseForAuthorFromRep);
            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRep);
            _courseLibraryRepository.Save();

            return NoContent();

        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

    }
}