using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Domain.Policies
{
    public sealed class RequiresAttributeValidator
    {
        public bool CheckPolicies(IDomainObject domainObject, ICommand cmd, IEnumerable<MethodInfo> methods)
        {
            foreach (MethodInfo method in methods)
            {
                if (!ArePoliciesFullfilled(domainObject, cmd, method))
                    return false;
            }
            return true;
        }

        private static bool ArePoliciesFullfilled(IDomainObject domainObject, ICommand cmd, MethodInfo method)
        {
            var annotation = method.GetCustomAttribute<RequiresAttribute>(true);

            if (annotation == null)
                return true;

            return annotation.Polices.Any(policy => domainObject.Policy(policy, cmd));
        }
    }
}