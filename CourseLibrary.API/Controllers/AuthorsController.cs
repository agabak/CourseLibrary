using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Helps;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
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
    }
}