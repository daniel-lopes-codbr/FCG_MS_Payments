using AutoMapper;
using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Application.DTOs;

namespace FCG_MS_Payments.Application.Mapping;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentResponse>()
            .ForMember(dest => dest.ClientSecret, opt => opt.Ignore());

        CreateMap<CreatePaymentRequest, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StripePaymentIntentId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore())
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src.Metadata ?? new Dictionary<string, string>()));
    }
} 