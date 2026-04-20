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
        CreateMap<Review, MentorProfileReviewDto>()
        .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
        (src.FromUserProfile != null && src.FromUserProfile.User != null)
        ? $"{src.FromUserProfile.User.FirstName} {src.FromUserProfile.User.LastName}"
        : "Anonymous Student"))
         .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
          .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
          .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<MentorProfile, MentorProfileListDto>()
            .ForMember(dest => dest.MentorFullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.TopSkills, opt => opt.MapFrom(src =>
                src.MentorSkills != null
                ? src.MentorSkills.Select(ms => ms.Skill.Name).Take(3).ToList()
                : new List<string>()));

        CreateMap<MentorProfile, MentorProfileDetailDto>()
            .ForMember(dest => dest.MentorFullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                src.MentorSkills != null
                ? src.MentorSkills.Select(ms => ms.Skill.Name).ToList()
                : new List<string>()))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ReviewsReceived))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

        CreateMap<MentorProfileCreateDto, MentorProfile>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<MentorProfileUpdateDto, MentorProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}

