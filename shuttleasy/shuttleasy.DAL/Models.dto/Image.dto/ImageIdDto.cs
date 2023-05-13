using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Image.dto
{
    public class ImageIdDto
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }

    }
}
