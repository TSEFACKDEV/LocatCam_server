namespace LocatCam.Core.Services;

using AutoMapper;
using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Enums;

/// <summary>
/// Profils de mapping AutoMapper entre entités et DTOs.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>Constructeur configurant tous les mappings.</summary>
    public MappingProfile()
    {
        // User <-> UserDto
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
            .ForMember(d => d.DateCreation, o => o.MapFrom(s => s.DateCreation));

        // Annonce <-> AnnonceDto
        CreateMap<Annonce, AnnonceDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Statut, o => o.MapFrom(s => s.Statut.ToString()))
            .ForMember(d => d.NomBailleur, o => o.MapFrom(s => s.User != null ? s.User.Nom : null))
            .ForMember(d => d.Photos, o => o.MapFrom(s => s.Photos.OrderBy(p => p.Ordre)));

        // AnnoncePhoto <-> AnnoncePhotoDto
        CreateMap<AnnoncePhoto, AnnoncePhotoDto>();

        // AnnonceCreateDto -> Annonce
        CreateMap<AnnonceCreateDto, Annonce>()
            .ForMember(d => d.Type, o => o.MapFrom(s => Enum.Parse<TypeAnnonce>(s.Type)))
            .ForMember(d => d.Statut, o => o.MapFrom(s => StatutAnnonce.Brouillon))
            .ForMember(d => d.DateCreation, o => o.MapFrom(s => DateTime.UtcNow));

        // AnnonceUpdateDto -> Annonce
        CreateMap<AnnonceUpdateDto, Annonce>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        // Favori <-> FavoriDto
        CreateMap<Favori, FavoriDto>();

        // Message <-> MessageDto
        CreateMap<Message, MessageDto>();

        // MessageCreateDto -> Message
        CreateMap<MessageCreateDto, Message>()
            .ForMember(d => d.DateEnvoi, o => o.MapFrom(s => DateTime.UtcNow))
            .ForMember(d => d.Lu, o => o.MapFrom(s => false));

        // Signalement <-> SignalementDto
        CreateMap<Signalement, SignalementDto>();
    }
}
