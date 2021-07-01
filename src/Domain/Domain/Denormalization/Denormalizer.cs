using System;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed record Denormalizer(Type Type, bool UseBackGround);
}