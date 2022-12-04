using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        
        }

        [HttpGet]
      
        public async Task <ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
           var users = await _userRepository.GetMembersAsync();

           return Ok(users);

          
        }
       
        [HttpGet("{username}")]
        public async Task <ActionResult<MemberDto>> GetUser(string username){
            //calls GetMemberAsync function which uses the mapper
            //to map the App User to the member Dto to return
            return await _userRepository.GetMemberAsync(username);
    
           
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if(user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);
            
            //NoContent returns a 204, which means the request was fine
            // but there is no content to return as the request was a put
            if(await _userRepository.SaveAllAsync()) return NoContent();
            //if the above line fails for what ever reason the badrequest below will return instead
            return BadRequest("Failed to update user");
        }
    }
}