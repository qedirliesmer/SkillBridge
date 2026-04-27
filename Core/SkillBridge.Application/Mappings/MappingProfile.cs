using AutoMapper;
using SkillBridge.Application.Commands.Bookings;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using SkillBridge.Application.DTOs.BookingDTOs;
using SkillBridge.Application.DTOs.CategoryDTOs;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.DTOs.StudentInterestDTOs;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
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
                ? src.MentorSkills.Where(ms => ms.Skill != null).Select(ms => ms.Skill.Name).Take(3).ToList()
                : new List<string>()));

        CreateMap<MentorProfile, MentorProfileDetailDto>()
            .ForMember(dest => dest.MentorFullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                src.MentorSkills != null
                ? src.MentorSkills.Where(ms => ms.Skill != null).Select(ms => ms.Skill.Name).ToList()
                : new List<string>()))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ReviewsReceived))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

        CreateMap<UserProfile, MyProfileDetailDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src =>
                src.StudentInterests != null
                ? src.StudentInterests.Where(si => si.Skill != null).Select(si => si.Skill.Name).ToList()
                : new List<string>()))
            .ForMember(dest => dest.IsMentor, opt => opt.MapFrom(src => src.MentorProfile != null))
            .ForMember(dest => dest.MentorInfo, opt => opt.MapFrom(src => src.MentorProfile));

        CreateMap<CreateUserProfileDto, UserProfile>()
            .ForMember(dest => dest.StudentInterests, opt => opt.Ignore());

        CreateMap<UpdateUserProfileDto, UserProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.StudentInterests, opt => opt.Ignore());

        CreateMap<UserProfile, PublicUserProfileDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src =>
                src.StudentInterests != null ? src.StudentInterests.Select(si => si.Skill.Name).ToList() : new List<string>()));

        CreateMap<Booking, BookingDetailDto>()
            .ForMember(dest => dest.MentorFullName, opt => opt.MapFrom(src =>
                src.Mentor != null && src.Mentor.User != null ? $"{src.Mentor.User.FirstName} {src.Mentor.User.LastName}" : "Naməlum Mentor"))
            .ForMember(dest => dest.MentorJobTitle, opt => opt.MapFrom(src => src.Mentor != null ? src.Mentor.CurrentJobTitle : string.Empty))
            .ForMember(dest => dest.MentorCompany, opt => opt.MapFrom(src => src.Mentor != null ? src.Mentor.Company : string.Empty))
            .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src =>
                src.Student != null && src.Student.User != null ? $"{src.Student.User.FirstName} {src.Student.User.LastName}" : "Naməlum Tələbə"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsReviewed, opt => opt.MapFrom(src => src.Review != null))
            .ForMember(dest => dest.ReviewRating, opt => opt.MapFrom(src => src.Review != null ? (double?)src.Review.Rating : null))
            .ForMember(dest => dest.ReviewComment, opt => opt.MapFrom(src => src.Review != null ? src.Review.Comment : null))
            .ForMember(dest => dest.IsCurrentUserMentor, opt => opt.Ignore()); 

        CreateMap<Booking, BookingListDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PartnerFullName, opt => opt.Ignore())
            .ForMember(dest => dest.PartnerJobTitleOrBio, opt => opt.Ignore());
        CreateMap<CreateBookingCommand, Booking>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => BookingStatus.Pending))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Availability, AvailabilityDto>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (int)src.DayOfWeek))
            .ForMember(dest => dest.DayOfWeekName, opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm")));

        CreateMap<CreateAvailabilityDto, Availability>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (Days)src.DayOfWeek))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
            .ForMember(dest => dest.Id, opt => opt.Ignore()); 

        CreateMap<UpdateAvailabilityDto, Availability>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (Days)src.DayOfWeek))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
            .ForMember(dest => dest.MentorId, opt => opt.Ignore());

        CreateMap<MentorProfileCreateDto, MentorProfile>().ForMember(d => d.UserId, o => o.Ignore());
        CreateMap<MentorProfileUpdateDto, MentorProfile>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore());

        CreateMap<Skill, SkillDto>()
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<Skill, SkillWithStatsDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.MentorCount, opt => opt.MapFrom(src => src.MentorSkills.Count))
            .ForMember(dest => dest.StudentInterestCount, opt => opt.MapFrom(src => src.StudentInterests.Count));

        CreateMap<CreateSkillDto, Skill>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.MentorSkills, opt => opt.Ignore())
            .ForMember(dest => dest.StudentInterests, opt => opt.Ignore());

        CreateMap<UpdateSkillDto, Skill>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.MentorSkills, opt => opt.Ignore())
            .ForMember(dest => dest.StudentInterests, opt => opt.Ignore());
        CreateMap<Skill, InterestDto>();

        CreateMap<Category, CategoryDto>();

        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Skills, opt => opt.Ignore());

        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Skills, opt => opt.Ignore());

        CreateMap<Category, CategoryWithSkillsDto>()
            .ForMember(dest => dest.TotalSkillsCount, opt => opt.MapFrom(src =>
                src.Skills != null ? src.Skills.Count() : 0))
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills));
    }
}


