﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDAGroupMVC.Models
{
    public class EmailSetting : BaseEntity
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpEmail { get; set; }
        public string SmtpPassword { get; set; }
    }
}
