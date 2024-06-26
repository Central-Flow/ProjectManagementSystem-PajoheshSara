﻿using Domain.Constants.Notification;
using Domain.Entities.HumanResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [MaxLength(700)]
        public string Description { get; set; }

        [Required]
        public NotificationTypes Type { get; set; }

        [Required]
        public string Issuer { get; set; }
        //UserRel
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
