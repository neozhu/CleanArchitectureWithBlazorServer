using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Notification;
public class UpdateUserProfileCommand:INotification
{
    public UserProfile UserProfile { get; set; } = null!;
}
public class UpdateUserProfileEventArgs : EventArgs
{
    public UserProfile UserProfile { get; set; } = null!;
}
