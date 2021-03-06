﻿using FluentValidation;

namespace Esquio.UI.Api.Shared.Models.Features.Update
{
    public class UpdateFeatureRequestValidator
        : AbstractValidator<UpdateFeatureRequest>
    {
        public UpdateFeatureRequestValidator()
        {
            this.RuleFor(f => f.Name)
                .MaximumLength(200)
                .NotNull();

            this.RuleFor(f => f.Description)
                .MaximumLength(2000)
                .NotNull();
        }
    }
}
