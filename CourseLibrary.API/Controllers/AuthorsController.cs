using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<AuthorDTO>> GetAuthors([FromQuery] AuthorsResourcesParameters authorsResourcesParameters)
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourcesParameters);

            return Ok(_mapper.Map<IEnumerable<AuthorDTO>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        public ActionResult<CourseDTO> GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo != null) return Ok(_mapper.Map<AuthorDTO>(authorFromRepo));
            return NotFound();
        }

        [HttpPost]
        public ActionResult<AuthorDTO> CreateAuthor(AuthorForCreationDTO author)
        {
            if (author == null) return BadRequest();

            var authorEntity = _mapper.Map<Entities.Author>(author);

            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorReturned = _mapper.Map<AuthorDTO>(authorEntity);

            return CreatedAtRoute("GetAuthor",new { authorId = authorReturned.Id }, authorReturned);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS,POST");
            return Ok();
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId,CourseForUpdateDTO course)
        {
            if(!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();
            var courseForAuthorFromRep = _courseLibraryRepository.GetCourse(authorId,courseId);
            if(courseForAuthorFromRep == null) return NotFound();

            //map the entity to a CourseForUpdateDTO
            //apply the update field values to that dto
            //map the CourseForUpdateDTO back to an entity
            _mapper.Map(course, courseForAuthorFromRep);
            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRep);

            _courseLibraryRepository.Save();
            return NoContent();

        }
    }
}