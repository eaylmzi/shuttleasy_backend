using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class CompanyDetailDto
    {
        public Company Company { get; set; }
        public CommentDetailsDto CommentDetails { get; set; }
    }
}
