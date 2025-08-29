using AutoMapper;
using HiringPipelineAPI.DTOs;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Candidate mappings
        CreateMap<Candidate, CandidateDto>();
        CreateMap<Candidate, CandidateDetailDto>()
            .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count))
            .ForMember(dest => dest.Applications, opt => opt.MapFrom(src => src.Applications));
        CreateMap<CreateCandidateDto, Candidate>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Applied"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<UpdateCandidateDto, Candidate>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateCandidateDto, Candidate>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Application mappings
        CreateMap<Application, ApplicationDto>();
        CreateMap<Application, ApplicationDetailDto>()
            .ForMember(dest => dest.Candidate, opt => opt.MapFrom(src => src.Candidate))
            .ForMember(dest => dest.Requisition, opt => opt.MapFrom(src => src.Requisition))
            .ForMember(dest => dest.StageHistory, opt => opt.MapFrom(src => src.StageHistories));
        CreateMap<CreateApplicationDto, Application>()
            .ForMember(dest => dest.CurrentStage, opt => opt.MapFrom(src => src.CurrentStage ?? "Applied"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<UpdateApplicationDto, Application>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateApplicationDto, Application>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Requisition mappings
        CreateMap<Requisition, RequisitionDto>();
        CreateMap<Requisition, RequisitionDetailDto>()
            .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count))
            .ForMember(dest => dest.Applications, opt => opt.MapFrom(src => src.Applications));
        CreateMap<CreateRequisitionDto, Requisition>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Open"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<UpdateRequisitionDto, Requisition>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateRequisitionDto, Requisition>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // StageHistory mappings
        CreateMap<StageHistory, StageHistoryDto>();
        CreateMap<StageHistory, StageHistoryDetailDto>()
            .ForMember(dest => dest.Application, opt => opt.MapFrom(src => src.Application));
        CreateMap<CreateStageHistoryDto, StageHistory>()
            .ForMember(dest => dest.MovedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Summary DTOs mappings
        CreateMap<Candidate, CandidateSummaryDto>();
        CreateMap<Requisition, RequisitionSummaryDto>();
        CreateMap<Application, ApplicationSummaryDto>();
        CreateMap<StageHistory, StageHistorySummaryDto>();
    }
}
