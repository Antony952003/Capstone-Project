using System.Runtime.Serialization;

namespace EmployeeGrievanceRedressal.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) { }
    }
}