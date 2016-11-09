﻿using System;
using Coolector.Common.Commands.Remarks.Models;

namespace Coolector.Common.Commands.Remarks
{
    public class ResolveRemark : IAuthenticatedCommand
    {
        public Request Request { get; set; }
        public string UserId { get; set; }
        public Guid RemarkId { get; set; }
        public RemarkFile Photo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool ValidatePhoto { get; set; }
        public bool ValidateLocation { get; set; }
    }
}