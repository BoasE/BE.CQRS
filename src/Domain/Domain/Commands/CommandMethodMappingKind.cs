﻿namespace BE.CQRS.Domain.Commands
{
    public enum CommandMethodMappingKind
    {
        Create,
        Update,
        UpdateWithoutHistory
    }
}