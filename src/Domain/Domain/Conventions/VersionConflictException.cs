using System;
using System.Runtime.Serialization;

namespace BE.CQRS.Domain.Conventions
{
   [Serializable]
   public class VersionConflictException : Exception
   {
      //
      // For guidelines regarding the creation of new exception types, see
      //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
      // and
      //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
      //

      public VersionConflictException()
      {
      }

      public VersionConflictException(string message) : base(message)
      {
      }

      public VersionConflictException(string message, Exception inner) : base(message, inner)
      {
      }

      protected VersionConflictException(
         SerializationInfo info,
         StreamingContext context) : base(info, context)
      {
      }
   }
}