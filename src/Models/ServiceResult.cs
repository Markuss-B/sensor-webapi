namespace SensorWebApi.Models;
public class ServiceResult
{
    public ServiceResultStatus Status { get; set; }

    public static ServiceResult Create(ServiceResultStatus status) => new ServiceResult
    {
        Status = status,
    };

    public static ServiceResult Success => Create(ServiceResultStatus.Success);
    public static ServiceResult NotFound => Create(ServiceResultStatus.NotFound);
    public static ServiceResult BadRequest => Create(ServiceResultStatus.BadRequest);

    public static implicit operator bool(ServiceResult result) => result.Status == ServiceResultStatus.Success;
    public bool IsNotFound => Status == ServiceResultStatus.NotFound;
    public bool IsBadRequest => Status == ServiceResultStatus.BadRequest;
}

public class ServiceResult<T> where T : class
{
    public ServiceResultStatus Status { get; set; }
    public T Data { get; set; }

    public static ServiceResult<T> Create(ServiceResultStatus status, T data = null) => new ServiceResult<T>
    {
        Status = status,
        Data = data,
    };

    public static ServiceResult<T> Success(T data) => Create(ServiceResultStatus.Success, data);
    public static ServiceResult<T> NotFound() => Create(ServiceResultStatus.NotFound);
    public static ServiceResult<T> BadRequest() => Create(ServiceResultStatus.BadRequest);
}

public enum ServiceResultStatus
{
    Success,
    NotFound,
    BadRequest,
}