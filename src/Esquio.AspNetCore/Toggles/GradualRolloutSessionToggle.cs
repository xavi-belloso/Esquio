﻿using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.AspNetCore.Toggles
{
    /// <summary>
    /// A binary <see cref="IToggle"/> that is active depending on current session id  and how this value is assigned to a specific partition using the 
    /// configured <see cref="IValuePartitioner"/>. This <see cref="IToggle"/> create 100 buckets for partitioner and assign the session id into a specific
    /// bucket. If assigned bucket is less or equal that Percentage property value this toggle is active.
    /// </summary>
    [DesignType(Description = "The session identifier falls within percentage created by Esquio Partitioner.", FriendlyName = "Partial rollout by Http Session Id")]
    [DesignTypeParameter(ParameterName = Percentage, ParameterType = EsquioConstants.PERCENTAGE_PARAMETER_TYPE, ParameterDescription = "The percentage of sessions that activates this toggle. Percentage from 0 to 100.")]
    public class GradualRolloutSessionToggle
        : IToggle
    {
        internal const string Percentage = nameof(Percentage);

        private readonly IValuePartitioner _partitioner;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="partitioner">The <see cref="IValuePartitioner"/> service to be used.</param>
        /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> service to be used.</param>
        public GradualRolloutSessionToggle(
            IValuePartitioner partitioner,
            IHttpContextAccessor httpContextAccessor)
        {
            _partitioner = partitioner ?? throw new ArgumentNullException(nameof(partitioner));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        ///<inheritdoc/>
        public ValueTask<bool> IsActiveAsync(ToggleExecutionContext context, CancellationToken cancellationToken = default)
        {
            if (Double.TryParse(context.Data[Percentage].ToString(), out double percentage))
            {
                if (percentage > 0d)
                {
                    var sessionId = _httpContextAccessor
                        .HttpContext
                        .Session
                        .Id;

                    // we apply also some entropy to sessionid value.
                    // adding this entropy ensure that not all features with gradual rollout for claim value are enabled/disable at the same time for the same user.

                    var assignedPartition = _partitioner.ResolvePartition(context.FeatureName + sessionId);
                    var active = assignedPartition <= percentage;

                    return new ValueTask<bool>(active);
                }
            }

            return new ValueTask<bool>(false);
        }
    }
}
