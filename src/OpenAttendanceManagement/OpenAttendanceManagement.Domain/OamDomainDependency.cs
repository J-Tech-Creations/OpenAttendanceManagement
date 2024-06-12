using Sekiban.Core.Dependency;
using System.Reflection;
namespace OpenAttendanceManagement.Domain;

public class OamDomainDependency : DomainDependencyDefinitionBase
{

    public override Assembly GetExecutingAssembly() => Assembly.GetExecutingAssembly();
    public override void Define()
    {
    }
}
