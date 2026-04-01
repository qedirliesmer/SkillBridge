using AutoMapper;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Mappings;

public class MappingProfile:Profile
{
    public MappingProfile()
    {
        CreateMap<MentorProfile, MentorProfileListDto>();

        CreateMap<Review, MentorProfileReviewDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                $"{src.FromUserProfile.FirstName} {src.FromUserProfile.LastName}"))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<MentorProfile, MentorProfileDetailDto>()
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                src.MentorSkills.Select(ms => ms.Skill.Name).ToList()))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.ReviewsReceived.Any() ? (decimal)src.ReviewsReceived.Average(r => r.Rating) : 0))

            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ReviewsReceived));

        CreateMap<MentorProfileCreateDto, MentorProfile>();
        CreateMap<MentorProfileUpdateDto, MentorProfile>();
    }
}

