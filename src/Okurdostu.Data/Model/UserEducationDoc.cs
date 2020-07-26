﻿using System;
using System.Collections.Generic;

namespace Okurdostu.Data
{
    public partial class UserEducationDoc
    {
        public UserEducationDoc()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid UserEducationId { get; set; }
        public string PathAfterRoot { get; set; }
        public string FullPath { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual UserEducation UserEducation { get; set; }
    }
}
