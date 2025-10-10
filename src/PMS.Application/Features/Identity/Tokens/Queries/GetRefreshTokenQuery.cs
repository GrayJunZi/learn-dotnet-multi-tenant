using MediatR;
using PMS.Core.Wrappers;
using System.Diagnostics.CodeAnalysis;

namespace PMS.Application.Features.Identity.Tokens.Queries;

public class GetRefreshTokenQuery : IRequest<IResponseWrapper>
{
    public RefreshTokenRequest RefreshTokenRequest { get; set; }
}

public class GetRefreshTokenQueryHandler(ITokenService tokenService) : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await tokenService.RefreshTokenAsync(request.RefreshTokenRequest);

        return await ResponseWrapper<TokenResponse>.SuccessAsync(refreshToken);
    }
}
