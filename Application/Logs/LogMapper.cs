using BackOffice.Domain.Logs;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Persistence.Models;

namespace BackOffice.Application.Logs
{
    public static class LogMapper
    {
        public static LogDataModel ToDataModel(Log domainModel)
        {
            return new LogDataModel
            {
                LogId = domainModel.Id.Value.ToString(),
                Timestamp = domainModel.Timestamp,
                ActionType = domainModel.ActionType.Value.ToString(),
                AffectedUserEmail = domainModel.AffectedUserEmail.Value,
                Details = domainModel.Details.Value
            };
        }

        public static LogDto ToDto(Log domainModel)
        {
            return new LogDto
            {
                LogId = domainModel.Id.Value.ToString(),
                Timestamp = domainModel.Timestamp,
                ActionType = domainModel.ActionType.Value.ToString(),
                AffectedUserEmail = domainModel.AffectedUserEmail.Value,
                Details = domainModel.Details.Value
            };
        }

        public static Log ToDomain(LogDataModel dataModel)
        {
            return new Log(
                new LogId(dataModel.LogId),
                new ActionType(Enum.Parse<ActionTypeEnum>(dataModel.ActionType)),
                new Email(dataModel.AffectedUserEmail),
                new Text(dataModel.Details)
            );
        }

        public static Log ToDomain(LogDto dto)
        {
            return new Log(
                new LogId(dto.LogId),
                new ActionType(Enum.Parse<ActionTypeEnum>(dto.ActionType)),
                new Email(dto.AffectedUserEmail),
                new Text(dto.Details)
            );
        }
    }
}
