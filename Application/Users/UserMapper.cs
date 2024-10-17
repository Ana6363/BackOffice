using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Persistence.Models;

public static class UserMapper
{
    public static UserDto ToDto(User domainModel)
    {
        return new UserDto
        {
            Id = domainModel.Id.AsString(),
            Role = domainModel.Role,
            Active = domainModel.Active,
            ActivationToken = domainModel.ActivationToken,  
            TokenExpiration = domainModel.TokenExpiration   
        };
    }

    public static UserDataModel ToDataModel(User domainModel)
    {
        return new UserDataModel
        {
            Id = domainModel.Id.AsString(),
            Role = domainModel.Role,
            Active = domainModel.Active,
            ActivationToken = domainModel.ActivationToken,
            TokenExpiration = domainModel.TokenExpiration
        };
    }

    public static User ToDomain(UserDataModel dataModel)
    {
        var user = new User(dataModel.Id, dataModel.Role);
        if (dataModel.Active)
        {
            user.MarkAsActive();
        }
        else
        {
            user.MarkAsInactive();
        }

        user.ActivationToken = dataModel.ActivationToken;  
        user.TokenExpiration = dataModel.TokenExpiration;

        return user;
    }
}
