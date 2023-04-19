﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Milestones;

namespace Tarteeb.Api.Services.Foundations.Milestones
{
    public interface IMilestoneService
    {
        ValueTask<Milestone> AddMilestoneAsync(Milestone milestone);
    }
}
