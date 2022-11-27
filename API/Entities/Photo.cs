using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    //creates a seperate table for photos that is still related to the app user
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        //photo entity must include an AppUserId and AppUser
        //so that every photo uploaded must have an assosciated user
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}