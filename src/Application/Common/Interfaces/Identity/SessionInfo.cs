using System.Runtime.Serialization;
using ActualLab.Fusion.Blazor;
using MemoryPack;
using MessagePack;
using KeyAttribute = MessagePack.KeyAttribute;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
[DataContract, MemoryPackable, MessagePackObject]
[ParameterComparer(typeof(ByValueParameterComparer))]
public sealed partial record SessionInfo(
    [property: DataMember, Key(0)] string UserId,
    [property: DataMember, Key(1)] string UserName,
    [property: DataMember, Key(2)] string DisplayName,
    [property: DataMember, Key(3)] string IPAddress,
    [property: DataMember, Key(4)] string TenantId,
    [property: DataMember, Key(5)] string ProfilePictureDataUrl,
    [property: DataMember, Key(6)] UserPresence Status

    )
{ }
public enum UserPresence
{
    Available,
    Busy,
    Donotdisturb,
    Away,
    Offline,
    Statusunknown
}
