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
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionController: ControllerBase
    {
         private readonly ICourseLibraryRepository _courseLibraryRepository;
         private readonly IMapper _mapper;

         public AuthorCollectionController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
         {
                _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));    
         }

         [HttpGet("({ids})", Name="GetAuthorCollection")]
         public IActionResult GetAuthorCollection([FromRoute] [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
         {
             if(ids == null) return BadRequest();

             var authorEntities = _courseLibraryRepository.GetAuthors(ids);
             if(ids.Count() != authorEntities.Count()) return NotFound();

             var authorToReturn = _mapper.Map<IEnumerable<AuthorDTO>>(authorEntities);
             return Ok(authorToReturn);

         }

        [HttpPost]
         public ActionResult<IEnumerable<AuthorDTO>> CreateAuthorCollection( 
             IEnumerable<AuthorForCreationDTO> authorCollection)
         {
            var authorEntities = _mapper.Map<IEnumerable<Entities.Author>>(authorCollection); 

            authorEntities.ToList().ForEach(author => {
                _courseLibraryRepository.AddAuthor(author);
            });

            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<IEnumerable<AuthorDTO>>(authorEntities);
            var idsAsString = string.Join(",", authorToReturn.Select(s => s.Id));

             return CreatedAtRoute("GetAuthorCollection", new {ids = idsAsString},authorToReturn);
         }

    }
}