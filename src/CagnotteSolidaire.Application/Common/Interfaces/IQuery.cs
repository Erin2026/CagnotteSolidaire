using MediatR;

namespace CagnotteSolidaire.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
