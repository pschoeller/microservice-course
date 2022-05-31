using PlatformService.DTOs;

namespace PlatformService.ASyncDataServices;

public interface IMessageBusClient{
    void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO);
}