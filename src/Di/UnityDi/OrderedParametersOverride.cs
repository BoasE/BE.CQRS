﻿using System;
using System.Collections.Generic;
using Unity.Resolution;

namespace BE.CQRS.Di.Unity
{
    public class OrderedParametersOverride : ResolverOverride
    {
        private readonly Queue<InjectionParameterValue> parameterValues;

        public OrderedParametersOverride(params object[] parameterValues)
        {
            this.parameterValues = new Queue<InjectionParameterValue>();
            foreach (object parameterValue in parameterValues)
            {
                this.parameterValues.Enqueue(InjectionParameterValue.ToParameter(parameterValue));
            }
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            if (parameterValues.Count < 1)
                return null;

            InjectionParameterValue value = parameterValues.Dequeue();
            return value.GetResolverPolicy(dependencyType);
        }
    }
}