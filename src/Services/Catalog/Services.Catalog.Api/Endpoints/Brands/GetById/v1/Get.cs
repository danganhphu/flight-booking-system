﻿using Ardalis.Result.AspNetCore;
using Services.Catalog.Features.Brands.GetBrandById.v1;

namespace Services.Catalog.Api.Endpoints.Brands.GetById.v1;

internal sealed class GetById(ISender sender) : Endpoint<GetBrandByIdRequest>
{
    public override void Configure()
    {
        Get(ApiRoutes.Brand.GetById);
        AllowAnonymous();

        Group<BrandGrouping>();
    }

    public override async Task HandleAsync(GetBrandByIdRequest request,
                                           CancellationToken ct)
    {
        var result = await sender.Send(new GetBrandById(request.Id), ct);

        await SendAsync(result, cancellation: ct);
    }
}
