using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Users.Commands.UpsertUserNewsPagePreference;

public class UpsertUserNewsPagePreferenceCommand : IRequest<Result>
{
    public UpsertUserNewsPagePreferenceDto Body { get; set; } = new();
}
