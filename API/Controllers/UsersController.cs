using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;

namespace API.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
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
           
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);
            
            //NoContent returns a 204, which means the request was fine
            // but there is no content to return as the request was a put
            if(await _userRepository.SaveAllAsync()) return NoContent();
            //if the above line fails for what ever reason the badrequest below will return instead
            return BadRequest("Failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0 ) photo.IsMain = true;

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                //this is so adding a photo results in a 201 response
                return CreatedAtAction(nameof(GetUser),
                 new {username = user.UserName},
                 _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            //get the current user
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            
            //protection if the user isnt found
            if(user == null) return NotFound();
            //gets the photo that matches the ID that has been passed in
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            //if no photo to return is found then response NotFound
            if (photo == null) return NotFound();
            //if the photo is already main return bad request
            if(photo.IsMain) return BadRequest("this is already your main photo");
            //get what the current main photo is
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            //if we do have a photo that is currently the main, set it to false 
            if (currentMain != null) currentMain.IsMain = false;
            //set the new photo to true
            photo.IsMain = true;

            //if the photo can be updated to main then return a 201 (no response because its an update)
            if(await _userRepository.SaveAllAsync()) return NoContent();
            //if all the above fails just respond with bad request
            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your main photo");

            //ones with a public ID need to be delete from Cloudinary ALSO
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting photo");

        }
    }
}