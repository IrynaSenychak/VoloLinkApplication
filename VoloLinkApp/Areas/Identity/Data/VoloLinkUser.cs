﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VoloLinkApp.Models;

namespace VoloLinkApp.Areas.Identity.Data;


public class VoloLinkUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(50)]
    public string LastName { get; set; }
    public ICollection<Event> CreatedEvents { get; set; }
    public ICollection<Event> ParticipatingEvents { get; set; }

}

