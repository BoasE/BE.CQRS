using System.Collections.Generic;
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
            CustomAttributeData requires = null;
            foreach (var cad in method.CustomAttributes)
            {
                if (cad.AttributeType == typeof(RequiresAttribute))
                {
                    requires = cad;
                    break;
                }
            }

            if (requires == null)
                return true;

            if (requires.ConstructorArguments.Count == 1)
            {
                var arg = requires.ConstructorArguments[0];
                if (arg.ArgumentType.IsArray)
                {
                    var list = arg.Value as IList<CustomAttributeTypedArgument>;
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var type = list[i].Value as System.Type;
                            if (type != null && domainObject.Policy(type, cmd))
                                return true;
                        }
                    }
                }
                else
                {
                    var type = arg.Value as System.Type;
                    if (type != null && domainObject.Policy(type, cmd))
                        return true;
                }
            }

            if (requires.NamedArguments != null)
            {
                for (int i = 0; i < requires.NamedArguments.Count; i++)
                {
                    var na = requires.NamedArguments[i];
                    if (na.MemberName == nameof(RequiresAttribute.Polices))
                    {
                        var arr = na.TypedValue.Value as IList<CustomAttributeTypedArgument>;
                        if (arr != null)
                        {
                            for (int j = 0; j < arr.Count; j++)
                            {
                                var type = arr[j].Value as System.Type;
                                if (type != null && domainObject.Policy(type, cmd))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}