using BE.FluentGuard;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    public sealed class CreateIndex<TDto> where TDto : class, new()
    {
        public IndexKeysDefinition<TDto> Definition { get; }

        public CreateIndexOptions Options { get; }

        private CreateIndex(IndexKeysDefinition<TDto> definition, CreateIndexOptions options)
        {
            Precondition.For(definition, nameof(definition)).NotNull();

            Definition = definition;
            Options = options;
        }

        public static CreateIndex<TDto> With(IndexKeysDefinition<TDto> definition, CreateIndexOptions options = null)
        {
            return new CreateIndex<TDto>(definition, options);
        }
    }
}